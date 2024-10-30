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


    public UserService(UserRepository userRepository, IConfiguration configuration, ILogger<UserService> logger)
    {
      _userRepository = userRepository;
      _configuration = configuration;
      _logger = logger;
    }

    public string HashPassword(string password)
    {
      return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
      return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }

    public async Task<string?> ValidateUserAsync(string username, string password)
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
        return GenerateJwtToken(user);
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

    public async Task<string> CreateAccount(string username, string password, string name, string email, string phone_number, string role)
    {
      try
      {
        // check current user role with token role
        var roleClaim = ClaimTypes.Role.ToString();
        if(roleClaim != "Admin" && role != "User")
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

    public string GenerateJwtToken(User user)
    {
      Console.WriteLine($"GenerateJwtToken::Generating JWT token {user.Username}");
      var claims = new[]
      {
          new Claim(JwtRegisteredClaimNames.Sub, user.Username),
          new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
          new Claim(ClaimTypes.Name, user.Username),
          new Claim(ClaimTypes.Role, user.Role)
      };
      string jwtKey = _configuration["Jwt:Key"] ?? throw new Exception("GenerateJwtToken::Jwt:Key is not set in the configuration");
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

      var token = new JwtSecurityToken(
          issuer: _configuration["Jwt:Issuer"],
          audience: _configuration["Jwt:Audience"],
          claims: claims,
          expires: DateTime.Now.AddMinutes(30), // Token expiration time
          signingCredentials: creds);

      return new JwtSecurityTokenHandler().WriteToken(token);
    }
  }
}
