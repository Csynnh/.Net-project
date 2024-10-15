using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using api.Filters;
using api.TransferModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using service;

namespace library.Controllers;

public class InvoiceDetailController : ControllerBase
{
    private readonly ILogger<InvoiceDetailController> _logger;
    private readonly InvoiceDetailService _invoiceDetailService;

    public InvoiceDetailController(ILogger<InvoiceDetailController> logger, InvoiceDetailService invoiceDetailService)
    {
        _logger = logger;
        _invoiceDetailService = invoiceDetailService;
    }

    [HttpGet]
    [Route("/api/invoice_details")]
    public ResponseDto Get()
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = _invoiceDetailService.GetInvoiceDetailForFeed()
        };
    }

    [HttpPost]
    [ValidateModel]
    [Route("/api/invoice_details")]
    public ResponseDto Post([FromBody] CreateInvoiceDetailRequestDto dto)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return new ResponseDto()
        {
            MessageToClient = "Successfully created an invoice detail",
            ResponseData = _invoiceDetailService.CreateInvoiceDetail(dto.invoices_id , dto.product_id , dto.amount, dto.price)
        };
    }

    [HttpPut]
    [ValidateModel]
    [Route("/api/invoice_details")]
    public ResponseDto Put([FromBody] UpdateInvoiceDetailRequestDto dto)
    {
        HttpContext.Response.StatusCode = 201;
        return new ResponseDto()
        {
            MessageToClient = "Successfully updated",
            ResponseData = _invoiceDetailService.UpdateInvoiceDetail(dto.invoices_id , dto.product_id , dto.amount, dto.price)
        };
    }

    [HttpDelete]
    [Route("/api/invoice_details/{invoices_id }/{product_id}")]
    public ResponseDto Delete([FromRoute] Guid invoices_id , [FromRoute] Guid product_id)
    {
        _invoiceDetailService.DeleteInvoiceDetail(invoices_id , product_id);
        return new ResponseDto()
        {
            MessageToClient = "Successfully deleted"
        };
    }
}
