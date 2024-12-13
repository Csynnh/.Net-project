using Microsoft.AspNetCore.Http;
using api.Filters;
using api.TransferModels;
using infrastructure.DataModels;
using Microsoft.AspNetCore.Mvc;
using service;
using Microsoft.AspNetCore.Authorization;

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
    [Route("/api/products")]
    public async Task<ResponseDto> GetIdByName([FromQuery] string name)
    {
        try
        {
            HttpContext.Response.StatusCode = 200;
            return new ResponseDto()
            {
                MessageToClient = "Successfully fetched",
                ResponseData = await _productService.GetIdByName(name)
            };
        }
        catch (Exception ex)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return new ResponseDto()
            {
                MessageToClient = $"An error occurred while fetching the products: {ex.Message}",
                ResponseData = null
            };
        }
    }

    [HttpGet]
    [Route("/api/products/collections/{name}")]
    public async Task<ResponseDto> ListProductByTypeName(
    [FromRoute] string name,
    [FromQuery] int pageNumber = 1,
    [FromQuery] int pageSize = 10,
    [FromQuery] string? size = null,
    [FromQuery] decimal? minPrice = null,
    [FromQuery] decimal? maxPrice = null)
    {
        try
        {
            HttpContext.Response.StatusCode = 200;
            _logger.LogInformation($"Fetching products with type id: {name}, Page: {pageNumber}, Page Size: {pageSize}, Size: {size}, Min Price: {minPrice}, Max Price: {maxPrice}");

            var products = await _productService.ListProductByTypeNameAsync(name, pageNumber, pageSize, size, minPrice, maxPrice);

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
                ResponseData = await _productService.CreateProductAsync(dto)
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

    [Authorize(Roles = "Admin")]
    [HttpPut]
    [ValidateModel]
    [Route("/api/products/{id}")]
    public async Task<ResponseDto> UpdateProduct([FromRoute] Guid id, [FromBody] UpdateProductModel dto)
    {
        try
        {
            HttpContext.Response.StatusCode = 200;
            _logger.LogInformation($"Updating a product with id: {id}");
            return new ResponseDto()
            {
                MessageToClient = "Successfully updated",
                ResponseData = await _productService.UpdateProductAsync(id, dto)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            HttpContext.Response.StatusCode = 204;
            return new ResponseDto()
            {
                MessageToClient = "An error occurred while updating the product",
                ResponseData = ex.Message
            };
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [Route("/api/products/upload-image")]
    public async Task<ResponseDto> UploadImage([FromForm] UploadImageRequest request)
    {
        try
        {
            HttpContext.Response.StatusCode = 200;
            _logger.LogInformation("Uploading image");
            return new ResponseDto()
            {
                MessageToClient = "Successfully uploaded",
                ResponseData = await _productService.UploadImageAsync(request.Image)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            HttpContext.Response.StatusCode = 204;
            return new ResponseDto()
            {
                MessageToClient = "An error occurred while uploading the image",
                ResponseData = ex.Message
            };
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete]
    [Route("/api/products/{id}")]
    public async Task<ResponseDto> DeleteProduct([FromRoute] Guid id)
    {
        try
        {
            HttpContext.Response.StatusCode = 200;
            _logger.LogInformation($"Deleting a product with id: {id}");
            await _productService.DeleteProductAsync(id);
            return new ResponseDto()
            {
                MessageToClient = "Successfully deleted",
                ResponseData = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            HttpContext.Response.StatusCode = 204;
            return new ResponseDto()
            {
                MessageToClient = "An error occurred while deleting the product",
                ResponseData = false
            };
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete]
    [Route("/api/products/color/{id}")]
    public async Task<ResponseDto> DeleteProductColor([FromRoute] Guid id)
    {
        try
        {
            HttpContext.Response.StatusCode = 200;
            _logger.LogInformation($"Deleting a product with id: {id}");
            await _productService.DeleteProductVariantAsync(id);
            return new ResponseDto()
            {
                MessageToClient = "Successfully deleted",
                ResponseData = true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            HttpContext.Response.StatusCode = 204;
            return new ResponseDto()
            {
                MessageToClient = "An error occurred while deleting the product",
                ResponseData = false
            };
        }
    }
}
