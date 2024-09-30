using System.Text.Json;
using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.Extensions.Logging;

namespace service;

public class ProductService
{
    private readonly ProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;
    public ProductService(ProductRepository productRepository, ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public IEnumerable<CollectionResponse> GetProducts()
    {
        var productCollection = _productRepository.GetProducts();;
        var groupedProducts = productCollection
        .GroupBy(p => new { p.collection, p.name, p.image_url, p.price })
        .Select(g => new CollectionResponse
        {
            collection = g.Key.collection,
            name = g.Key.name,
            image_url = g.Key.image_url,
            price = g.Key.price,
            available_colors = g.SelectMany(p => p.available_colors).Distinct().ToList()
        })
        .ToList();

        return groupedProducts;
    }

    public ProductDetailsResponse? GetProductByName(string name, string collection)
    {
        var prod_details_response = _productRepository.GetProductByName(name.ToUpper(), collection.ToUpper());
        if(prod_details_response == null)
        {
            return null;
        }
        var prod_details = new ProductDetailsResponse
        {
            collection = prod_details_response.collection,
            name = prod_details_response.name,
            price = prod_details_response.price,
            available_colors = prod_details_response.available_colors,
            ext_fields = JsonSerializer.Deserialize<List<ExtField>>(prod_details_response.ext_fields)?.FirstOrDefault() ?? new ExtField(),
            images_per_color = JsonSerializer.Deserialize<Dictionary<string, List<object>>>(prod_details_response.images_per_color)
        };
        return prod_details;
    }

    public string CreateProduct(ProductRequest product_request)
    {
        try
        {
            // Check if product already exists
            var prod_details_response = _productRepository.GetProductByName(product_request.name.ToUpper(), product_request.collection.ToUpper());
            if(prod_details_response != null)
            {
                // Product already exists will check name, collection, price, description are different
                if(prod_details_response.price != product_request.price || prod_details_response.description != product_request.description)
                {
                    throw new Exception($"Could not create product:: Product already exists with different price or description:: price:{prod_details_response.price} - desc:{prod_details_response.description}");
                }
            }
            var count_product = product_request.count;
            var img_url = JsonSerializer.Serialize(new {
                main_image = product_request.main_img_url,
                additional_images = product_request.additional_img_url ?? new List<string>()
            });
            for (int i = 0; i < count_product; i++)
            {
                var product = new ProductDBRequest(
                    product_request.name.ToUpper(),
                    product_request.description,
                    product_request.collection.ToUpper(),
                    product_request.price,
                    img_url,
                    product_request.color,
                    product_request.ext_fields,
                    product_request.is_sold
                );
                _productRepository.CreateProduct(product);
            }
            return product_request.name;
        }
        catch (System.Exception e)
        {
            _logger.LogError($"Could not create product: {e}");
            throw new Exception($"Could not create product: {e}");
        }
    }

    public string? UpdateProduct(string collection, string name, ProductRequest product_request)
    {
        try
        {
            var prod_details_response = _productRepository.GetProductByName(name.ToUpper(), collection.ToUpper());
            if(prod_details_response == null)
            {
                return null;
            }
            _logger.LogInformation($"Updating: {prod_details_response != null}");
            var img_url = JsonSerializer.Serialize(new {
                main_image = product_request.main_img_url,
                additional_images = product_request.additional_img_url ?? new List<string>()
            });
            var product = new ProductDBRequest(
                product_request.name.ToUpper(),
                product_request.description,
                product_request.collection.ToUpper(),
                product_request.price,
                img_url,
                product_request.color,
                product_request.ext_fields,
                product_request.is_sold
            );
            return _productRepository.UpdateProduct(collection.ToUpper(), name.ToUpper(), product_request.count, product);
        }
        catch (System.Exception e)
        {
            _logger.LogError($"Could not update product:: {e}");
            return null;
        }
    }

    public bool DeleteProduct(string collection, string name, string color, int count)
    {
        try
        {
            var prod_details_response = _productRepository.GetProductByName(name.ToUpper(), collection.ToUpper(), color);
            if(prod_details_response == null)
            {
                throw new Exception("Product not found");
            }
            var result = _productRepository.DeleteProduct(collection.ToUpper(), name.ToUpper(), color, count);
            return true;
        }
        catch (System.Exception e)
        {
            
            throw new Exception($"Could not delete product: {e}");
        }
    }
}
