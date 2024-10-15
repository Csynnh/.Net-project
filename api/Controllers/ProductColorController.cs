using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using api.Filters;
using api.TransferModels;
using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using service;

namespace library.Controllers;

public class ProductColorController : ControllerBase
{
    private readonly ILogger<ProductColorController> _logger;
    private readonly ProductColorService _productColorService;

    public ProductColorController(ILogger<ProductColorController> logger, ProductColorService productColorService)
    {
        _logger = logger;
        _productColorService = productColorService;
    }

    [HttpGet]
    [Route("/api/product_colors")]
    public ResponseDto Get()
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = _productColorService.GetProductColorForFeed()
        };
    }

    [HttpPost]
    [ValidateModel]
    [Route("/api/product_colors")]
    public ResponseDto Post([FromBody] CreateProductColorsRequestDto dto)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return new ResponseDto()
        {
            MessageToClient = "Successfully created an invoice",
            ResponseData = _productColorService.CreateProductColor(dto.product_id, dto.color_name, dto.color_code, dto.inventory, dto.total, dto.image_url1, dto.image_url2, dto.image_url3, dto.image_url4, dto.image_url5)
        };
    }

    [HttpPut]
    [ValidateModel]
    [Route("/api/product_colors/{id}")]
    public ResponseDto Put([FromRoute] Guid id, [FromBody] UpdateProductColorsRequestDto dto)
    {
        HttpContext.Response.StatusCode = 201;
        return new ResponseDto()
        {
            MessageToClient = "Successfully updated",
            ResponseData = _productColorService.UpdateProductColor(id, dto.color_name, dto.color_code, dto.inventory, dto.total, dto.image_url1, dto.image_url2, dto.image_url3, dto.image_url4, dto.image_url5)
        };
    }

    [HttpDelete]
    [Route("/api/product_colors/{id}")]
    public ResponseDto Delete([FromRoute] Guid id)
    {
        _productColorService.DeleteProductColor(id);
        return new ResponseDto()
        {
            MessageToClient = "Successfully deleted"
        };
    }
}
