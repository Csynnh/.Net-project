using api.Filters;
using api.Request;
using api.TransferModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service;

namespace library.Controllers;

public class OderController : ControllerBase
{
    private readonly ILogger<OderController> _logger;
    private readonly OderService _oderService;

    public OderController(ILogger<OderController> logger, OderService oderService)
    {
        _logger = logger;
        _oderService = oderService;
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet]
    [Route("/api/oder/{account_id}")]
    public async Task<ResponseDto> Get([FromRoute] Guid account_id, [FromQuery] string status)
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = await _oderService.ListOderByAccountId(account_id, status)
        };
    }
    [Authorize(Roles = "User,Admin")]
    [HttpGet]
    [Route("/api/oder/status/{status}")]
    public async Task<ResponseDto> GetWithStatus([FromRoute] string status)
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = await _oderService.ListOrderByStatus(status)
        };
    }
    [Authorize(Roles = "User,Admin")]
    [HttpGet("orders/total-grouped-by-status")]
    public async Task<IActionResult> GetTotalOrders()
    {
        try
        {
            var result = await _oderService.GetTotalOrders();
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }


    [Authorize(Roles = "User,Admin")]
    [HttpPost]
    [ValidateModel]
    [Route("/api/oder")]
    // [TypeFilter(typeof(OderFilter))]
    public async Task<ResponseDto> Post([FromBody] CreateOderRequest dto)
    {
        try
        {
            HttpContext.Response.StatusCode = StatusCodes.Status201Created;
            return new ResponseDto()
            {
                MessageToClient = "Successfully created an oder",
                ResponseData = await _oderService.CreateNewOder(
                    accountId: dto.account_id,
                    total: dto.price,
                    paymentMethod: dto.paymentMethod,
                    shippingMethod: dto.shippingMethod,
                    userInfo: dto.userInfo,
                    products: dto.products
                )
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating an oder");
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return new ResponseDto()
            {
                MessageToClient = "An error occurred while creating the oder",
                ResponseData = null
            };
        }
    }

    // [Authorize(Roles = "User,Admin")]
    [HttpPut("/api/oder/next-status/{id}")]
    public async Task<IActionResult> UpdateToNextOrderStatus([FromRoute] Guid id)
    {
         var result = await _oderService.UpdateToNextOrderStatus(id);
         if (result)
            return Ok(new { message = "Order status updated to the next step successfully" });
        return BadRequest(new { message = "Failed to update order status" });
    }

    // http://192.168.1.8/admin/api/oder/next-status/14565dcb-7fbc-4b6b-bce2-2e2f019668e
    // http://localhost:api/oder/next-status/14565dcb-7fbc-4b6b-bce2-2e2f019668e8
    // [HttpPut]
    // [ValidateModel]
    // [Route("/api/oder/{id}")]
    // public ResponseDto Put([FromRoute] Guid id, [FromBody] UpdateInvoiceRequestDto dto)
    // {
    //     HttpContext.Response.StatusCode = 201;
    //     return new ResponseDto()
    //     {
    //         MessageToClient = "Successfully updated",
    //         ResponseData = _oderService.UpdateInvoice(id, dto.price, dto.status, dto.checkout_method, dto.shipping_method)
    //     };
    // }

    // [HttpDelete]
    // [Route("/api/oder/{id}")]
    // public ResponseDto Delete([FromRoute] Guid id)
    // {
    //     _oderService.DeleteInvoice(id);
    //     return new ResponseDto()
    //     {
    //         MessageToClient = "Successfully deleted"
    //     };
    // }
}
