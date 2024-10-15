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

    public IEnumerable<Product> GetProductForFeed()
    {
        return _productRepository.GetProductForFeed();
    }

    public IEnumerable<Product> GetProductForHomePage()
    {
        return _productRepository.GetProductForHomePage();
    }

    public IEnumerable<Product> GetProductForItemDetailPage(Guid product_id)
    {
        return _productRepository.GetProductForItemDetailPage(product_id);
    }

    public Product CreateProduct(string prod_name, string pro_desc, decimal price, decimal width, decimal height, string type)
    {
        return _productRepository.CreateProduct(prod_name, pro_desc, price, width, height, type);
    }

    public Product UpdateProduct(Guid productId, string prod_name, string pro_desc, decimal price, decimal width, decimal height, string type)
    {
        return _productRepository.UpdateProduct(productId, prod_name, pro_desc, price, width, height, type);
    }

    public void DeleteProduct(Guid id)
    {
        var result = _productRepository.DeleteProduct(id);
        if (!result)
        {
            throw new Exception("Could not delete product");
        }
    }
}
