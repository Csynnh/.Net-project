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
    private readonly OtpService _otpService;

    public AuthController(UserService userService, OtpService otpService)
    {
      _userService = userService;
      _otpService = otpService;
    }

    [HttpPost("login")]
    public async Task<ResponseDto> Login(LoginRequest loginRequest)
    {
      LoginResponse? token = await _userService.ValidateUserAsync(loginRequest.Username, loginRequest.Password);
      HttpContext.Response.StatusCode = 200;
      if (token != null)
      {
        return new ResponseDto()
        {
          MessageToClient = "Successfully logged in!",
          ResponseData = token
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

    [HttpPut]
    [ValidateModel]
    [Route("accounts/{id}")]
    public async Task<ResponseDto> UpdateAccount([FromBody] UpdateAccountRequestDto dto, [FromRoute] Guid id)
    {
      try
      {
        var responseData = await _userService.UpdateAccount(id, dto.name, dto.email, dto.phone_number);
        HttpContext.Response.StatusCode = StatusCodes.Status200OK;
        return new ResponseDto()
        {
          MessageToClient = "Successfully updated account",
          ResponseData = responseData
        };
      }
      catch (Exception ex)
      {
        HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return new ResponseDto()
        {
          MessageToClient = $"UpdateAccount::An error occurred while updating the account",
          ResponseData = ex.Message
        };
      }
    }

    [HttpPost("request-otp")]
    public async Task<ResponseDto> RequestOtp([FromBody] OtpRequest request)
    {
      try
      {
        await _otpService.GenerateAndSendOtpAsync(request.Email);
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return new ResponseDto()
        {
          MessageToClient = "OTP sent successfully",
          ResponseData = true
        };
      }
      catch (Exception ex)
      {
        HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return new ResponseDto()
        {
          MessageToClient = "Failed to send OTP",
          ResponseData = ex.Message
        };
      }
    }


    [HttpPost("confirm-otp")]
    public async Task<ResponseDto> ConfirmOtp([FromBody] OtpConfirmRequest request)
    {
      try
      {
        bool isValidOtp = await _otpService.ValidateOtp(request.Email, request.Otp);
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return new ResponseDto()
        {
          MessageToClient = "OTP confirmed successfully",
          ResponseData = isValidOtp
        };
      }
      catch (Exception ex)
      {
        HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return new ResponseDto()
        {
          MessageToClient = "Failed to confim OTP",
          ResponseData = ex.Message
        };
      }
    }

    [HttpPost("change-password")]
    public async Task<ResponseDto> ChangePassword([FromBody] PasswordChangeRequest request)
    {
      try
      {
        if (await _userService.ChangePassword(request))
        {
          HttpContext.Response.StatusCode = StatusCodes.Status201Created;
          return new ResponseDto()
          {
            MessageToClient = "Password changed successfully",
            ResponseData = true
          };
        }
        HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
        return new ResponseDto()
        {
          MessageToClient = "Failed to change password",
          ResponseData = false
        };
      }
      catch (Exception ex)
      {
        HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        return new ResponseDto()
        {
          MessageToClient = "Failed to change password",
          ResponseData = ex.Message
        };
      }
    }
  }
}
