
using api.TransferModels;
using Microsoft.AspNetCore.Mvc;
using service;

namespace api.Controllers;

[Route("api/shipping-method")]
[ApiController]
public class ShippingMethodController : ControllerBase
{
  private ShippingMethodService _shippingMethodService;
  public ShippingMethodController(ShippingMethodService shippingMethodService)
  {
    _shippingMethodService = shippingMethodService;
  }

  [HttpGet]
  public async Task<ResponseDto> GetShippingMethods()
  {
    try
    {
      var result = await _shippingMethodService.GetShippingMethods();
      HttpContext.Response.StatusCode = StatusCodes.Status200OK;
      return new ResponseDto()
      {
        MessageToClient = "Shipping methods retrieved successfully",
        ResponseData = result
      };
    }
    catch (Exception ex)
    {
      HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
      return new ResponseDto()
      {
        MessageToClient = "Error retrieving shipping methods",
        ResponseData = ex.Message
      };
    }
  }

  [HttpPost]
  public async Task<ResponseDto> CreateShippingMethod([FromBody] ShippingMethodRequest shippingMethod)
  {
    try
    {
      var result = await _shippingMethodService.CreateShippingMethod(shippingMethod);
      HttpContext.Response.StatusCode = StatusCodes.Status201Created;
      return new ResponseDto()
      {
        MessageToClient = "Shipping method created successfully",
        ResponseData = result
      };
    }
    catch (Exception ex)
    {
      HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
      return new ResponseDto()
      {
        MessageToClient = "Error creating shipping method",
        ResponseData = ex.Message
      };
    }
  }

  [HttpDelete("{shippingMethodId}")]
  public async Task<ResponseDto> DeleteShippingMethod(Guid shippingMethodId)
  {
    try
    {
      await _shippingMethodService.DeleteShippingMethod(shippingMethodId);
      HttpContext.Response.StatusCode = StatusCodes.Status200OK;
      return new ResponseDto()
      {
        MessageToClient = "Shipping method deleted successfully",
        ResponseData = null
      };
    }
    catch (Exception ex)
    {
      HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
      return new ResponseDto()
      {
        MessageToClient = "Error deleting shipping method",
        ResponseData = ex.Message
      };
    }
  }
}
