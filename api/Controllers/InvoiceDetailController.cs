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
    [Route("/api/invoicedetails")]
    public ResponseDto Get()
    {
        HttpContext.Response.StatusCode = 314;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = _invoiceDetailService.GetInvoiceDetails()
        };
    }

    [HttpPost]
    [ValidateModel]
    [Route("/api/invoicedetails")]
    public ResponseDto Post([FromBody] CreateInvoiceDetailRequestDto dto)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return new ResponseDto()
        {
            MessageToClient = "Successfully created an invoice detail",
            ResponseData = _invoiceDetailService.CreateInvoiceDetail(dto.ID_HoaDon, dto.ID_HangHoa, dto.SoLuong, dto.Gia)
        };
    }

    [HttpPut]
    [ValidateModel]
    [Route("/api/invoicedetails")]
    public ResponseDto Put([FromBody] UpdateInvoiceDetailRequestDto dto)
    {
        HttpContext.Response.StatusCode = 201;
        return new ResponseDto()
        {
            MessageToClient = "Successfully updated",
            ResponseData = _invoiceDetailService.UpdateInvoiceDetail(dto.ID_HoaDon, dto.ID_HangHoa, dto.SoLuong, dto.Gia)
        };
    }

    [HttpDelete]
    [Route("/api/invoicedetails/{idHoaDon}/{idHangHoa}")]
    public ResponseDto Delete([FromRoute] int idHoaDon, [FromRoute] int idHangHoa)
    {
        _invoiceDetailService.DeleteInvoiceDetail(idHoaDon, idHangHoa);
        return new ResponseDto()
        {
            MessageToClient = "Successfully deleted"
        };
    }
}
