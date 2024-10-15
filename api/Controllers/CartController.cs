using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using api.Filters;
using api.TransferModels;
using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using service;

namespace library.Controllers;

public class CartController : ControllerBase
{
    private readonly ILogger<CartController> _logger;
    private readonly CartService _cartService;

    public CartController(ILogger<CartController> logger, CartService cartService)
    {
        _logger = logger;
        _cartService = cartService;
    }

    [HttpGet]
    [Route("/api/invoices")]
    public ResponseDto Get()
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = _cartService.GetCartForFeed()
        };
    }

    [HttpPost]
    [ValidateModel]
    [Route("/api/invoices")]
    public ResponseDto Post([FromBody] CreateCartsRequestDto dto)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return new ResponseDto()
        {
            MessageToClient = "Successfully created an invoice",
            ResponseData = _cartService.CreateCart(dto.account_id, dto.product_id, dto.quantity)
        };
    }

    [HttpPut]
    [ValidateModel]
    [Route("/api/invoices/{id}")]
    public ResponseDto Put([FromRoute] Guid id, [FromBody] CreateCartsRequestDto dto)
    {
        HttpContext.Response.StatusCode = 201;
        return new ResponseDto()
        {
            MessageToClient = "Successfully updated",
            ResponseData = _cartService.UpdateCart(id, dto.quantity)
        };
    }

    [HttpDelete]
    [Route("/api/invoices/{id}")]
    public ResponseDto Delete([FromRoute] Guid id)
    {
        _cartService.DeleteCart(id);
        return new ResponseDto()
        {
            MessageToClient = "Successfully deleted"
        };
    }
}
