using System.ComponentModel.DataAnnotations;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using infrastructure.Repositories;

namespace service;

public class ProductService
{
    private readonly ProductRepository _productRepository;

    public ProductService(ProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public IEnumerable<Product> GetProducts()
    {
        return _productRepository.GetProducts();
    }

    public Product CreateProduct(string name, string desc, decimal price, int inventory, string image_url)
    {
        return _productRepository.CreateProduct(name, desc, price, inventory, image_url);
    }

    public Product UpdateProduct(int id, string name, string desc, decimal price, int inventory, string image_url)
    {
        return _productRepository.UpdateProduct(id, name, desc, price, inventory, image_url);
    }

    public void DeleteProduct(int id)
    {
        var result = _productRepository.DeleteProduct(id);
        if (!result)
        {
            throw new Exception("Could not delete product");
        }
    }
}
