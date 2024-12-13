using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security;

namespace service
{
  public class UserService
  {
    private readonly UserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<UserService> _logger;
    private readonly OtpService _otpService;


    public UserService(UserRepository userRepository, OtpService otpService, IConfiguration configuration, ILogger<UserService> logger)
    {
      _userRepository = userRepository;
      _configuration = configuration;
      _logger = logger;
      _otpService = otpService;
    }

    public string HashPassword(string password)
    {
      return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
      return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    public async Task<LoginResponse?> ValidateUserAsync(string username, string password)
    {
      // Retrieve the user from the database
      var user = await GetUserByUsernameAsync(username);
      if (user == null)
      {
        return null;
      }
      // Verify the password
      bool IsCorrectPassword = VerifyPassword(password, user.PasswordHash);
      if (IsCorrectPassword)
      {
        var token = GenerateJwtToken(user, password);
        return new LoginResponse
        {
          Name = user.Name,
          Token = token.Token,
          AccountId = user.Id,
          ExpiredTime = token.ExpiredTime,
          Email = user.Email,
          PhoneNumber = user.PhoneNumber
        };
      }
      return null;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
      User? user = await _userRepository.GetUserByUsernameAsync(username);
      if (user == null)
      {
        _logger.LogInformation($"User with username {username} not found");
        return null;
      }

      return user;
    }

    public async Task<string> CreateAccount(string username, string password, string name, string email, string phone_number, string role="User")
    {
      try
      {
        // check current user role with token role
        var roleClaim = ClaimTypes.Role.ToString();
        if (roleClaim != "Admin" && role != "User")
        {
          throw new SecurityException("CreateAccount::You do not have permission to create an account with this role");
        }

        var account = new Account
        {
          username = username,
          password = HashPassword(password),
          role = role,
          name = name,
          email = email,
          phone_number = phone_number
        };

        await _userRepository.CreateAccountAsync(account: account);
        _logger.LogInformation($"Successfully created an account for {name}");
        return $"Successfully created an account for {name}";
      }
      catch (Exception ex)
      {
        throw new Exception($"CreateAccount::Failed to create an account for {username}: {ex.Message}");
      }
    }

    public async Task<string> UpdateAccount(Guid id, string name, string email, string phone_number)
    {
      try
      {
        var account = new Account
        {
          id = id,
          name = name,
          email = email,
          phone_number = phone_number
        };

        await _userRepository.UpdateAccountAsync(account: account);
        _logger.LogInformation($"Successfully updated account for {name}");
        return $"Successfully updated account for {name}";
      }
      catch (Exception ex)
      {
        throw new Exception($"UpdateAccount::Failed to update account for {name}: {ex.Message}");
      }
    }

    public async Task<bool> ChangePassword(PasswordChangeRequest request)
    {
      if (await _otpService.ValidateOtp(request.Email, request.Otp))
      {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user == null)
        {
          throw new Exception("ChangePassword::User not found");
        }
        var newPasswordHash = HashPassword(request.NewPassword);
        user.PasswordHash = newPasswordHash;
        await _userRepository.UpdatePasswordAsync(user);
        return true;
      }

      throw new Exception("ChangePassword::OTP validation failed");
    }

    public TokenModel GenerateJwtToken(User user, string password)
    {
      Console.WriteLine($"GenerateJwtToken::Generating JWT token {user.Username} - {user.Id.ToString()}");
      var claims = new[]
      {
          new Claim(JwtRegisteredClaimNames.Sub, user.Username),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          new Claim(ClaimTypes.Name, user.Username),
          new Claim(ClaimTypes.Role, user.Role),
          new Claim(ClaimTypes.Hash, password),
          new Claim(ClaimTypes.Sid, user.Id.ToString())
      };
      string jwtKey = _configuration["Jwt:Key"] ?? throw new Exception("GenerateJwtToken::Jwt:Key is not set in the configuration");
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: _configuration["Jwt:Issuer"],
          audience: _configuration["Jwt:Audience"],
          claims: claims,
          expires: DateTime.Now.AddDays(1), // Token expiration time
          signingCredentials: creds);

      return new TokenModel
      {
        Token = new JwtSecurityTokenHandler().WriteToken(token),
        ExpiredTime = token.ValidTo
      };
    }
  }
}
