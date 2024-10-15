using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using api.Filters;
using api.TransferModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using service;

namespace library.Controllers;

public class CartController : ControllerBase
{
    private readonly ILogger<CartController> _logger;
    private readonly CartService _invoiceService;

    public CartController(ILogger<CartController> logger, CartService invoiceService)
    {
        _logger = logger;
        _invoiceService = invoiceService;
    }

    [HttpGet]
    [Route("/api/invoices")]
    public ResponseDto Get()
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = _invoiceService.GetInvoices()
        };
    }

    [HttpPost]
    [ValidateModel]
    [Route("/api/invoices")]
    public ResponseDto Post([FromBody] CreateInvoiceRequestDto dto)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return new ResponseDto()
        {
            MessageToClient = "Successfully created an invoice",
            ResponseData = _invoiceService.CreateInvoice(dto.account_id , dto.created_date, dto.price, dto.status, dto.checkout_method, dto.shipping_method)
        };
    }

    [HttpPut]
    [ValidateModel]
    [Route("/api/invoices/{id}")]
    public ResponseDto Put([FromRoute] Guid id, [FromBody] UpdateInvoiceRequestDto dto)
    {
        HttpContext.Response.StatusCode = 201;
        return new ResponseDto()
        {
            MessageToClient = "Successfully updated",
            ResponseData = _invoiceService.UpdateInvoice(id, dto.account_id , dto.created_date, dto.price, dto.status, dto.checkout_method, dto.shipping_method)
        };
    }

    [HttpDelete]
    [Route("/api/invoices/{id}")]
    public ResponseDto Delete([FromRoute] Guid id)
    {
        _invoiceService.DeleteInvoice(id);
        return new ResponseDto()
        {
            MessageToClient = "Successfully deleted"
        };
    }
}
