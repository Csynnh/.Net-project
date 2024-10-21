using Amazon.S3;
using infrastructure.DataModels;
using infrastructure.Repositories;
using Microsoft.AspNetCore.Http;

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

    public async Task<string> CreateProduct(string prod_name, string pro_desc, decimal price, string size, string type, int inventory, ProductDetails details, IFormFile imageFile, string color, IAmazonS3 _s3Client)
    {
        if (imageFile == null || imageFile.Length == 0)
            throw new ArgumentException("No image uploaded.");

        using var stream = imageFile.OpenReadStream();
        var uploader = new S3Uploader(_s3Client);
        var fileName = $"images/{imageFile.FileName}";
        string image_url = await uploader.UploadImageAsync(imageFile);

        return image_url;
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
