using api.Filters;
using api.Request;
using api.TransferModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using service;

namespace library.Controllers;

public class OderController : ControllerBase
{
    private readonly ILogger<OderController> _logger;
    private readonly OderService _oderService;
    private readonly IHubContext<NotificationHub> _hubContext;


    public OderController(ILogger<OderController> logger, OderService oderService, IHubContext<NotificationHub> hubContext)
    {
        _logger = logger;
        _oderService = oderService;
        _hubContext = hubContext;
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
            await _hubContext.Clients.All.SendAsync("ReceiveOrderNotification", (object)response);

            return results;
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

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("/api/oder/retrieve-chart-data")]
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
}
