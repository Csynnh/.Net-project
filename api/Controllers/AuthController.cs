using api.TransferModels;
using infrastructure.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service;
using api.Filters;

namespace api.Controllers
{
  [Route("api/auth")]
  [ApiController]
  public class AuthController : ControllerBase
  {
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
      _userService = userService;
    }

    [HttpPost("login")]
    public async Task<ResponseDto> Login(LoginRequest loginRequest)
    {
      string? token = await _userService.ValidateUserAsync(loginRequest.Username, loginRequest.Password);
        HttpContext.Response.StatusCode = 200;
      if (token != null)
      {
        return new ResponseDto()
        {
            MessageToClient = "Successfully logged in",
            ResponseData = new { Token = token }
        };
      }
      HttpContext.Response.StatusCode = 401;
      return new ResponseDto()
      {
          MessageToClient = "Invalid username or password",
          ResponseData = null
      };
    }

    [HttpPost]
    [ValidateModel]
    [Route("accounts")]
    public async Task<ResponseDto> CreateAccount([FromBody] CreateAccountRequestDto dto)
    {
        try
        {
          var responseData = await _userService.CreateAccount(dto.username, dto.password, dto.name, dto.email, dto.phone_number, dto.role);
          HttpContext.Response.StatusCode = StatusCodes.Status201Created;
          return new ResponseDto()
          {
            MessageToClient = "Successfully created new account",
            ResponseData = responseData
          };
        }
        catch (Exception ex)
        {
          HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
          return new ResponseDto()
          {
            MessageToClient = $"CreateAccount::An error occurred while creating the account",
            ResponseData = ex.Message
          };
        }
    }
  }
}
