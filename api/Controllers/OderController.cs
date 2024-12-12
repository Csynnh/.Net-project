using api.Filters;
using api.Request;
using api.TransferModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service;
using Newtonsoft.Json;

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
    [Route("/api/orders/account/{account_id}")]
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
    [Route("/api/orders/status/{status}")]
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
    [HttpGet]
    [Route("/api/orders/account/{accountId}/status/{status}")]
    public async Task<ResponseDto> ListOrderStatusByAccountId([FromRoute] string status, [FromRoute] Guid accountId)
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = await _oderService.ListOrderStatusByAccountId(accountId, status)
        };
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet("/api/orders/total-grouped-by-status")]
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
    [Route("/api/orders")]
    public async Task<ResponseDto> Post([FromBody] CreateOderRequest dto)
    {
        try
        {
            HttpContext.Response.StatusCode = StatusCodes.Status201Created;
            _logger.LogInformation($"Creating order with data: {dto.account_id}");

            var response = await _oderService.CreateNewOder(
                    accountId: dto.account_id,
                    total: dto.price,
                    paymentMethod: dto.paymentMethod,
                    shippingMethod: dto.shippingMethod,
                    userInfo: dto.userInfo,
                    products: dto.products
                );
            var results = new ResponseDto()
            {
                MessageToClient = "Successfully created an oder",
                ResponseData = response
            };

            return results;
        }
        catch (Exception ex)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return new ResponseDto()
            {
                MessageToClient = $"An error occurred while creating the oder {ex.Message}",
                ResponseData = null
            };
        }
    }

    // [Authorize(Roles = "User,Admin")]
    [HttpPut("/api/orders/next-status/{id}")]
    public async Task<IActionResult> UpdateToNextOrderStatus([FromRoute] Guid id)
    {
        var result = await _oderService.UpdateToNextOrderStatus(id);
        if (result)
            return Ok(new { message = "Order status updated to the next step successfully" });
        return BadRequest(new { message = "Failed to update order status" });
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("/api/orders/retrieve-chart-data")]
    public async Task<ResponseDto> RetrieveChartData([FromBody] RetrieveChartDataRequest dto)
    {
        try
        {
            HttpContext.Response.StatusCode = StatusCodes.Status200OK;
            return new ResponseDto()
            {
                MessageToClient = "Successfully Retrieve Chart Data",
                ResponseData = await _oderService.RetrieveChartData(dto.ChartType)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while Retrieving Chart Data");
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return new ResponseDto()
            {
                MessageToClient = "An error occurred while Retrieving Chart Data",
                ResponseData = null
            };
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet]
    [Route("/api/orders/{id}")]
    public async Task<ResponseDto> Get([FromRoute] Guid id)
    {
        try
        {
            HttpContext.Response.StatusCode = 200;
            return new ResponseDto()
            {
                MessageToClient = "Successfully fetched",
                ResponseData = await _oderService.GetOrderById(id)
            };
        }
        catch (System.Exception)
        {

            HttpContext.Response.StatusCode = 404;
            return new ResponseDto()
            {
                MessageToClient = "Oder not found",
                ResponseData = null
            };
        }
    }
}
