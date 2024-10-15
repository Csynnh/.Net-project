using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using api.Filters;
using api.TransferModels;
using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using service;

namespace library.Controllers;

public class UserAddressController : ControllerBase
{
    private readonly ILogger<UserAddressController> _logger;
    private readonly UserAddressService _userAddressService;

    public UserAddressController(ILogger<UserAddressController> logger, UserAddressService userAddressService)
    {
        _logger = logger;
        _userAddressService = userAddressService;
    }

    [HttpGet]
    [Route("/api/user_addresses")]
    public ResponseDto Get()
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = _userAddressService.GetUserAddressForFeed()
        };
    }

    [HttpPost]
    [ValidateModel]
    [Route("/api/user_addresses")]
    public ResponseDto Post([FromBody] CreateUserAddressRequestDto dto)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return new ResponseDto()
        {
            MessageToClient = "Successfully created an invoice",
            ResponseData = _userAddressService.CreateUserAddress(dto.account_id, dto.address)
        };
    }

    [HttpPut]
    [ValidateModel]
    [Route("/api/user_addresses/{id}")]
    public ResponseDto Put([FromRoute] Guid id, [FromBody] UpdateUserAddressRequestDto dto)
    {
        HttpContext.Response.StatusCode = 201;
        return new ResponseDto()
        {
            MessageToClient = "Successfully updated",
            ResponseData = _userAddressService.UpdateUserAddress(id, dto.address)
        };
    }

    [HttpDelete]
    [Route("/api/user_addresses/{id}")]
    public ResponseDto Delete([FromRoute] Guid id)
    {
        _userAddressService.DeleteUserAddress(id);
        return new ResponseDto()
        {
            MessageToClient = "Successfully deleted"
        };
    }
}
