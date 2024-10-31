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
    public ResponseDto Get([FromRoute] Guid account_id)
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = _oderService.ListOderByAccountId(account_id)
        };
    }

    // [HttpPost]
    // [ValidateModel]
    // [Route("/api/oder")]
    // public ResponseDto Post([FromBody] CreateInvoiceRequestDto dto)
    // {
    //     HttpContext.Response.StatusCode = StatusCodes.Status201Created;
    //     return new ResponseDto()
    //     {
    //         MessageToClient = "Successfully created an invoice",
    //         ResponseData = _oderService.CreateInvoice(dto.account_id, dto.price, dto.status, dto.checkout_method, dto.shipping_method)
    //     };
    // }

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
