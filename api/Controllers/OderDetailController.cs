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

[Route("api/oder-detail")]
[ApiController]
public class OderDetailController : ControllerBase
{
    private readonly ILogger<OderDetailController> _logger;
    private readonly OderDetailService _oderDetailService;

    public OderDetailController(ILogger<OderDetailController> logger, OderDetailService oderDetailService)
    {
        _logger = logger;
        _oderDetailService = oderDetailService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin, User")]
    public async Task<ResponseDto> CreateOderDetail([FromBody] OderDetailRequest oderDetail)
    {
        try
        {
            OderDetailResponse response = await _oderDetailService.CreateOderDetail(oderDetail);
            HttpContext.Response.StatusCode = 201;
            return new ResponseDto
            {
                MessageToClient = "Oder detail created successfully",
                ResponseData = response
            };
        }
        catch (Exception ex)
        {
            HttpContext.Response.StatusCode = 400;
            return new ResponseDto
            {
                MessageToClient = "Failed to create oder detail",
                ResponseData = new { Error = ex.Message }
            };
        }
    }
}
