using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using api.Filters;
using api.TransferModels;
using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using service;

namespace library.Controllers;

public class UserStoredInformationController : ControllerBase
{
    private readonly ILogger<UserStoredInformationController> _logger;
    private readonly UserStoredInformationService _userAddressService;

    public UserStoredInformationController(ILogger<UserStoredInformationController> logger, UserStoredInformationService userAddressService)
    {
        _logger = logger;
        _userAddressService = userAddressService;
    }

    [Authorize(Roles = "User,Admin")]
    [HttpGet]
    [Route("/api/user-stored-information/{account_id}")]
    public async Task<ResponseDto> Get([FromRoute] Guid account_id)
    {
        try
        {
            HttpContext.Response.StatusCode = 200;
            return new ResponseDto()
            {
                MessageToClient = "Successfully fetched",
                ResponseData = await _userAddressService.ListUserStoredInformation(account_id)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while fetching user addresses.");
            HttpContext.Response.StatusCode = 500;
            return new ResponseDto()
            {
                MessageToClient = "An error occurred while fetching user addresses.",
                ResponseData = ex.Message
            };
        }
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPost]
    [ValidateModel]
    [Route("/api/user-stored-information")]
    public async Task<ResponseDto> Post([FromBody] CreateUserAddressRequestDto dto)
    {
        try
        {
            HttpContext.Response.StatusCode = StatusCodes.Status201Created;
            return new ResponseDto()
            {
                MessageToClient = "Successfully created an user address",
                ResponseData = await _userAddressService.CreateUserStoredInformation(dto.account_id, dto.address)
            };
        }
        catch (Exception ex)
        {
            HttpContext.Response.StatusCode = 500;
            return new ResponseDto()
            {
                MessageToClient = "An error occurred while creating user address.",
                ResponseData = ex.Message
            };
        }
    }

    [Authorize(Roles = "User,Admin")]
    [HttpPut]
    [ValidateModel]
    [Route("/api/user-stored-information/{accountId}/{id}")]
    public async Task<ResponseDto> Put([FromRoute] Guid id, [FromRoute] Guid accountId, [FromBody] UserInformationRequest address)
    {
        try
        {
            HttpContext.Response.StatusCode = 201;
            return new ResponseDto()
            {
                MessageToClient = "Successfully updated",
                ResponseData = await _userAddressService.UpdateUserStoredInformation(id, accountId, address)
            };
        }
        catch (Exception ex)
        {
            HttpContext.Response.StatusCode = 500;
            return new ResponseDto()
            {
                MessageToClient = "An error occurred while updating user address.",
                ResponseData = ex.Message
            };
        }
    }

    [Authorize(Roles = "User,Admin")]
    [HttpDelete]
    [Route("/api/user-stored-information/{id}")]
    public async Task<ResponseDto> Delete([FromRoute] Guid id, [FromRoute] Guid account_id)
    {
        await _userAddressService.DeleteUserStoredInformation(id, account_id);
        return new ResponseDto()
        {
            MessageToClient = "Successfully deleted"
        };
    }
}
