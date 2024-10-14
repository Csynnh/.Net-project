using System.ComponentModel.DataAnnotations;
using infrastructure.DataModels;
using infrastructure.QueryModels;
using infrastructure.Repositories;

namespace service;

public class ProductColorService
{
    private readonly ProductColorRepository _ProductColorRepository;

    public ProductColorService(ProductColorRepository ProductColorRepository)
    {
        _ProductColorRepository = ProductColorRepository;
    }

    public IEnumerable<ProductColor> GetProductColorForFeed()
    {
        return _ProductColorRepository.GetProductColorForFeed();
    }

    public ProductColor CreateProductColor(Guid productId, string colorName, string colorCode, int inventory, int total, string image1Url, string image2Url, string image3Url, string image4Url, string image5Url)
    {
        return _ProductColorRepository.CreateProductColor(productId, colorName, colorCode, inventory, total, image1Url, image2Url, image3Url, image4Url, image5Url);
    }

    public ProductColor UpdateProductColor(Guid colorId, string colorName, string colorCode, int inventory, int total, string image1Url, string image2Url, string image3Url, string image4Url, string image5Url)
    {
        return _ProductColorRepository.UpdateProductColor(colorId, colorName, colorCode, inventory, total, image1Url, image2Url, image3Url, image4Url, image5Url);
    }

    public void DeleteProductColor(Guid id)
    {
        var result = _ProductColorRepository.DeleteProductColor(id);
        if (!result)
        {
            throw new Exception("Could not delete ProductColor");
        }
    }
}
