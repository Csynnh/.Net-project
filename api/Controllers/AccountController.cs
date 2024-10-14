using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using api.Filters;
using api.TransferModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using service;

namespace library.Controllers;

public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly AccountService _accountService;

    public AccountController(ILogger<AccountController> logger, AccountService accountService)
    {
        _logger = logger;
        _accountService = accountService;
    }

    // [HttpGet]
    // [Route("/api/accounts")]
    // public ResponseDto Get()
    // {
    //     HttpContext.Response.StatusCode = 200;
    //     return new ResponseDto()
    //     {
    //         MessageToClient = "Successfully fetched",
    //         ResponseData = _accountService.GetAccounts()
    //     };
    // }

    [HttpPost]
    [ValidateModel]
    [Route("/api/accounts")]
    public ResponseDto Post([FromBody] CreateAccountRequestDto dto)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return new ResponseDto()
        {
            MessageToClient = "Successfully created an account",
            ResponseData = _accountService.CreateAccount(dto.username , dto.password , dto.name, dto.email, dto.phone_number , dto.role )
        };
    }

    // [HttpPut]
    // [ValidateModel]
    // [Route("/api/accounts/{id}")]
    // public ResponseDto Put([FromRoute] int id, [FromBody] UpdateAccountRequestDto dto)
    // {
    //     HttpContext.Response.StatusCode = 201;
    //     return new ResponseDto()
    //     {
    //         MessageToClient = "Successfully updated",
    //         ResponseData = _accountService.UpdateAccount(id, dto.username , dto.password , dto.name, dto.email, dto.phone_number, dto.address , dto.role )
    //     };
    // }

    [HttpDelete]
    [Route("/api/accounts/{id}")]
    public ResponseDto Delete([FromRoute] int id)
    {
        _accountService.DeleteAccount(id);
        return new ResponseDto()
        {
            MessageToClient = "Successfully deleted"
        };
    }
}
