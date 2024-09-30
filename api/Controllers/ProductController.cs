using api.Filters;
using api.TransferModels;
using infrastructure.DataModels;
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

    /// <summary>
    /// Get list of products grouped by collections.
    /// </summary>
    /// <response code="200">Returns the list of products successfully.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpGet]
    [Route("/api/products")]
    public ResponseDto List()
    {
        HttpContext.Response.StatusCode = 200;
        HttpContext.Response.ContentType = "application/json";
        return new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = _productService.GetProducts()
        };
    }


    /// <summary>
    /// Get details of product by name.
    /// </summary>
    /// <response code="200">Returns the list of products successfully.</response>
    /// <response code="404">If the product is not found.</response>
    [HttpGet]
    [Route("/api/products/{collection}/{product_name}")]
    public IActionResult Get(string product_name, string collection)
    {
        var product = _productService.GetProductByName(product_name, collection);
        if (product == null)
        {
            return NotFound();
        }

        return Ok(new ResponseDto()
        {
            MessageToClient = "Successfully fetched",
            ResponseData = product
        });
    }


    /// <summary>
    /// Create a new product.
    /// </summary>
    /// <response code="201">Returns the newly created product.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPost]
    [ValidateModel]
    [Route("/api/products")]
    public ResponseDto Post([FromBody] ProductRequest product_request)
    {
        var MessageToClient = "Successfully created a product";
        string? ResponseData;
        try
        {
            ResponseData = _productService.CreateProduct(product_request);
            HttpContext.Response.StatusCode = StatusCodes.Status201Created;
        }
        catch (System.Exception e)
        {
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            MessageToClient = "Failed to create a product";
            ResponseData = e.Message;
        }
        return new ResponseDto()
        {
            MessageToClient = MessageToClient,
            ResponseData = ResponseData
        };
    }

    /// <summary>
    /// Update a product.
    /// </summary>
    /// <response code="201">Returns the updated product.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="500">If an internal server error occurs.</response>
    [HttpPut]
    [ValidateModel]
    [Route("/api/products/{collection}/{product_name}")]
    public ResponseDto Put([FromRoute] string product_name, [FromRoute] string collection, [FromBody] ProductRequest product_request)
    {
        HttpContext.Response.StatusCode = 201;
        var MessageToClient = "Successfully updated";
        string? ResponseData;
        try
        {
            ResponseData = _productService.UpdateProduct(collection, product_name, product_request);
            if(ResponseData == null)
            {
                HttpContext.Response.StatusCode = 404;
                MessageToClient = "Product not found";
            }
        }
        catch (System.Exception e)
        {
            HttpContext.Response.StatusCode = 500;
            MessageToClient = "Failed to update a product";
            ResponseData = e.Message;
        }
        return new ResponseDto()
        {
            MessageToClient = MessageToClient,
            ResponseData = ResponseData
        };
    }

    [HttpDelete]
    [Route("/api/products/{collection}/{product_name}")]
    public ResponseDto Delete([FromRoute] string collection, [FromRoute] string product_name, [FromRoute] string color, [FromRoute] int count)
    {
        var MessageToClient = "Successfully deleted";
        object ResponseData = false;
        try
        {
            var results = _productService.DeleteProduct(collection, product_name, color, count);
            if(results) {
                ResponseData = results;
            }

        }
        catch (System.Exception e)
        {
            HttpContext.Response.StatusCode = 500;
            MessageToClient = "Failed to delete a product";
            ResponseData = e.Message;
        }
        return new ResponseDto()
        {
            MessageToClient = MessageToClient,
            ResponseData = ResponseData
        };
    }
}
