using api.Filters;
using api.TransferModels;
using infrastructure.DataModels;
using Microsoft.AspNetCore.Mvc;
using service;
using Amazon.S3;
using Microsoft.AspNetCore.Authorization;

namespace library.Controllers;

public class ProductController : ControllerBase
{
    private readonly ILogger<ProductController> _logger;
    private readonly IAmazonS3 _s3Client;
    private readonly ProductService _productService;

    public ProductController(ILogger<ProductController> logger, ProductService productService, IAmazonS3 s3Client)
    {
        _logger = logger;
        _productService = productService;
        _s3Client = s3Client;
    }


    [HttpGet]
    [Route("/api/products/{id}")]
    public async Task<ResponseDto> GetProductForItemDetailPage([FromRoute] Guid id)
    {
        HttpContext.Response.StatusCode = 200;
        _logger.LogInformation($"Fetching product with id: {id}");
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = await _productService.GetProductByIdAsync(id)
        };
    }


    [HttpGet]
    [Route("/api/products/collections/{typeId}")]
    public async Task<ResponseDto> ListProductByTypeId(
    [FromRoute] Guid typeId,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? size = null,
    [FromQuery] decimal? minPrice = null,
    [FromQuery] decimal? maxPrice = null)
    {
        try
        {
            HttpContext.Response.StatusCode = 200;
            _logger.LogInformation($"Fetching products with type id: {typeId}, Page: {pageNumber}, Page Size: {pageSize}, Size: {size}, Min Price: {minPrice}, Max Price: {maxPrice}");

            var products = await _productService.ListProductByTypeIdAsync(typeId, pageNumber, pageSize, size, minPrice, maxPrice);

            return new ResponseDto()
            {
                MessageToClient = "Successfully fetched",
                ResponseData = products
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return new ResponseDto()
            {
                MessageToClient = "An error occurred while fetching the products",
                ResponseData = null
            };
        }
    }

    [HttpGet]
    [Route("/api/products/collections")]
    public async Task<ResponseDto> ListProductByType()
    {
        HttpContext.Response.StatusCode = 200;
        _logger.LogInformation("Fetching all products");
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = await _productService.ListProductByTypeAsync()
        };
    }


    // TODO: Implement the ListProductByOrderStatus method†
    [Authorize(Roles = "User")]
    [HttpGet]
    [Route("/api/products/oders/{accountId}/{orderStatus}")]
    public async Task<ResponseDto> ListProductByOrderStatus([FromRoute] Guid accountId, [FromRoute] string orderStatus)
    {
        HttpContext.Response.StatusCode = 200;
        _logger.LogInformation($"Fetching products with order status: {orderStatus}");
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = await _productService.ListProductByOderStatusAsync(accountId, orderStatus)
        };
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateModel]
    [Route("/api/products")]
    public async Task<ResponseDto> Post([FromForm] CreateProductModel dto)
    {
        try
        {
            HttpContext.Response.StatusCode = StatusCodes.Status201Created;
            _logger.LogInformation($"Creating a product with name: {dto.ProductName}");
            return new ResponseDto()
            {
                MessageToClient = "Successfully created a product",
                ResponseData = await _productService.CreateProductAsync(dto, _s3Client)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return new ResponseDto()
            {
                MessageToClient = "An error occurred while creating the product",
                ResponseData = ex.Message
            };
        }
    }
}
