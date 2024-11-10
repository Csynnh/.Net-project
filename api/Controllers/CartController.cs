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

public class CartController : ControllerBase
{
    private readonly ILogger<CartController> _logger;
    private readonly CartService _cartService;

    public CartController(ILogger<CartController> logger, CartService cartService)
    {
        _logger = logger;
        _cartService = cartService;
    }

    // Create Cart
    [HttpPost]
    [ValidateModel]
    [Route("/api/carts")] // phần đuôi của API trên swagger
    public ResponseDto Post([FromBody] Cart dto)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;


        try
        {
            _cartService.CreateCart(dto.account_id, dto.variant_product_id, dto.quantity);
            return new ResponseDto()
            {
                MessageToClient = "Successfully created an invoice",
                ResponseData = "Created cart successfully!"
            };
        }
        catch (Exception ex) // Catch other general exceptions
        {
            return new ResponseDto()
            {
                MessageToClient = "Error",
                ResponseData = ex.Message
            };
        }
    }

    //Get List Cart
    [HttpGet]
    [Route("/api/carts/{account_id}")]
    // [Authorize(Roles = "User")]
    public ResponseDto Get([FromRoute] Guid account_id)
    {
        HttpContext.Response.StatusCode = 200;

        try
        {
            return new ResponseDto()
            {
                MessageToClient = "Successfully fetched",
                ResponseData = _cartService.GetCartForFeed(account_id)
            };
        }
        catch (Exception ex)
        {
            return new ResponseDto()
            {
                MessageToClient = "Error",
                ResponseData = ex.Message
            };
        }
    }


    //Update Cart
    [HttpPut]
    [ValidateModel]
    [Route("/api/carts/{id}/{quantity}")]
    public ResponseDto Put([FromRoute] Guid id, [FromRoute] int quantity)
    {
        HttpContext.Response.StatusCode = 201;

        try
        {
            _cartService.UpdateCart(id, quantity);
            return new ResponseDto()
            {
                MessageToClient = "Successfully updated",
                ResponseData = "Successfully updated"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDto()
            {
                MessageToClient = "Error",
                ResponseData = ex.Message
            };
        }
    }

    [HttpDelete]
    [Route("/api/carts/{id}")]
    public ResponseDto Delete([FromRoute] Guid id)
    {
        HttpContext.Response.StatusCode = 200;
        try
        {
            _cartService.DeleteCart(id);

            return new ResponseDto()
            {
                MessageToClient = "Successfully deleted",
                ResponseData = "Successfully deleted"
            };
        }
        catch (Exception ex)
        {
            return new ResponseDto()
            {
                MessageToClient = "Error",
                ResponseData = ex.Message
            };
        }
    }
}
