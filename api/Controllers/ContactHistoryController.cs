using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using api.Filters;
using api.TransferModels;
using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using service;

namespace library.Controllers;

public class ContactHistoryController : ControllerBase
{
    private readonly ILogger<ContactHistoryController> _logger;
    private readonly ContactService _contactService;

    public ContactHistoryController(ILogger<ContactHistoryController> logger, ContactService contactService)
    {
        _logger = logger;
        _contactService = contactService;
    }

    [HttpGet]
    [Route("/api/invoices")]
    public ResponseDto Get()
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = _contactService.GetContactHistoryForFeed()
        };
    }

    [HttpPost]
    [ValidateModel]
    [Route("/api/invoices")]
    public ResponseDto Post([FromBody] CreateContactHistoryRequestDto dto)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return new ResponseDto()
        {
            MessageToClient = "Successfully created an invoice",
            ResponseData = _contactService.CreateContactHistory(dto.account_id, dto.contact_details)
        };
    }

    [HttpPut]
    [ValidateModel]
    [Route("/api/invoices/{id}")]
    public ResponseDto Put([FromRoute] Guid id, [FromBody] UpdateContactHistoryRequestDto dto)
    {
        HttpContext.Response.StatusCode = 201;
        return new ResponseDto()
        {
            MessageToClient = "Successfully updated",
            ResponseData = _contactService.UpdateContactHistory(dto.id, dto.contact_details)
        };
    }

    [HttpDelete]
    [Route("/api/invoices/{id}")]
    public ResponseDto Delete([FromRoute] Guid id)
    {
        _contactService.DeleteContactHistory(id);
        return new ResponseDto()
        {
            MessageToClient = "Successfully deleted"
        };
    }
}
