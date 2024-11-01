using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using api.Filters;
using api.TransferModels;
using infrastructure.Repositories;
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
    public async Task<ResponseDto> Get([FromRoute] Guid account_id)
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = await _oderService.ListOderByAccountId(account_id)
        };
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPost]
    [ValidateModel]
    [Route("/api/oder")]
    public async Task<ResponseDto> Post([FromBody] CreateOderRequestDto dto)
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
                    status: dto.status,
                    paymentMethodId: dto.payment_method_id,
                    shippingMethodId: dto.shipping_method_id,
                    storedInformationId: dto.user_stored_info_id
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
