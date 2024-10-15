using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using api.Filters;
using api.TransferModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using service;

namespace library.Controllers;

public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly ProductService _productService;

    public ProductController(ILogger<ProductController> logger, ProductService productService)
    {
        _logger = logger;
        _productService = productService;
    }

    [HttpGet]
    [Route("/api/products")]
    public ResponseDto Get()
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = _productService.GetProductForFeed()
        };
    }

    [HttpGet]
    [Route("/api/products_for_homepage")]
    public ResponseDto GetProductForHomePage()
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = _productService.GetProductForHomePage()
        };
    }

    [HttpGet]
    [Route("/api/products_for_item_detail_page, {id}")]
    public ResponseDto GetProductForItemDetailPage([FromRoute] Guid id)
    {
        HttpContext.Response.StatusCode = 200;
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = _productService.GetProductForItemDetailPage(id)
        };
    }

    [HttpPost]
    [ValidateModel]
    [Route("/api/products")]
    public ResponseDto Post([FromBody] CreateProductRequestDto dto)
    {
        HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        return new ResponseDto()
        {
            MessageToClient = "Successfully created a product",
            ResponseData = _productService.CreateProduct(dto.name, dto.desc, dto.price, dto.width, dto.height, dto.type)
        };
    }

    [HttpPut]
    [ValidateModel]
    [Route("/api/products/{id}")]
    public ResponseDto Put([FromRoute] Guid id, [FromBody] UpdateProductRequestDto dto)
    {
        HttpContext.Response.StatusCode = 201;
        return new ResponseDto()
        {
            MessageToClient = "Successfully updated",
            ResponseData = _productService.UpdateProduct(id, dto.name, dto.desc, dto.price, dto.width, dto.height, dto.type)
        };
    }

    [HttpDelete]
    [Route("/api/products/{id}")]
    public ResponseDto Delete([FromRoute] Guid id)
    {
        _productService.DeleteProduct(id);
        return new ResponseDto()
        {
            MessageToClient = "Successfully deleted"
        };
    }
}
