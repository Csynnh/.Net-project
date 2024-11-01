using System.Security.Claims;
using api.Filters;
using api.TransferModels;
using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using service;

namespace api.Controllers;

[Route("api/payment-method")]
[ApiController]
public class PaymentMethodController : ControllerBase
{
  private PaymentMethodService _paymentMethodService;
  public PaymentMethodController(PaymentMethodService paymentMethodService)
  {
    _paymentMethodService = paymentMethodService;
  }

  [HttpGet("{accountId}")]
  public async Task<ResponseDto> GetPaymentMethodByAccountId(Guid accountId)
  {
    try
    {
      var result = await _paymentMethodService.GetPaymentMethodByAccountId(accountId);
      HttpContext.Response.StatusCode = StatusCodes.Status200OK;
      return new ResponseDto()
      {
        MessageToClient = "Payment method found",
        ResponseData = result
      };
    }
    catch (Exception ex)
    {
      HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
      return new ResponseDto()
      {
        MessageToClient = "Error getting payment method",
        ResponseData = ex.Message
      };
    }
  }

  [HttpPost]
  public async Task<ResponseDto> AddPaymentMethod([FromBody] PaymentMethodRequest paymentMethod)
  {
    try
    {
      var result = await _paymentMethodService.AddPaymentMethod(paymentMethod);
      HttpContext.Response.StatusCode = StatusCodes.Status201Created;
      return new ResponseDto()
      {
        MessageToClient = "Payment method added successfully",
        ResponseData = result
      };
    }
    catch (Exception ex)
    {
      HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
      return new ResponseDto()
      {
        MessageToClient = "Error adding payment method",
        ResponseData = ex.Message
      };
    }
  }

  [HttpDelete("{paymentMethodId}")]
  public async Task<ResponseDto> DeletePaymentMethod(Guid paymentMethodId)
  {
    try
    {
      var result = await _paymentMethodService.DeletePaymentMethod(paymentMethodId);
      HttpContext.Response.StatusCode = StatusCodes.Status200OK;
      return new ResponseDto()
      {
        MessageToClient = "Payment method deleted successfully",
        ResponseData = result
      };
    }
    catch (Exception ex)
    {
      HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
      return new ResponseDto()
      {
        MessageToClient = "Error deleting payment method",
        ResponseData = ex.Message
      };
    }
  }
}