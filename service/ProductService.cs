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

    public Product CreateProduct(string tenHangHoa, string moTa, decimal gia, int soLuongTonKho, string hinhAnh)
    {
        return _productRepository.CreateProduct(tenHangHoa, moTa, gia, soLuongTonKho, hinhAnh);
    }

    public Product UpdateProduct(int idHangHoa, string tenHangHoa, string moTa, decimal gia, int soLuongTonKho, string hinhAnh)
    {
        return _productRepository.UpdateProduct(idHangHoa, tenHangHoa, moTa, gia, soLuongTonKho, hinhAnh);
    }

    public void DeleteProduct(int idHangHoa)
    {
        var result = _productRepository.DeleteProduct(idHangHoa);
        if (!result)
        {
            throw new Exception("Could not delete product");
        }
    }
}
