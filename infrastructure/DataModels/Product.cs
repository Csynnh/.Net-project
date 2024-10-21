using Microsoft.AspNetCore.Http;


namespace infrastructure.DataModels;

public class Product
{
    public Guid id  { get; set; }// Primary Key
    public string prod_name { get; set; } = string.Empty;  // Tên của sản phẩm
    public string pro_desc { get; set; } = string.Empty;  // Mô tả sản phẩm
    public decimal price { get; set; }  // Giá của sản phẩm
    public decimal width { get; set; }  // Giá của sản phẩm
    public decimal height { get; set; }  // Giá của sản phẩm
    public string type { get; set; } = string.Empty;  // Giá của sản phẩm
}

public class Size
{
    public const string XS = "XS";
    public const string S = "S";
    public const string M = "M";
    public const string L = "L";
    public const string XL = "XL";
    public const string XXL = "XXL";
    public const string XXXL = "XXXL";
}

public class Collection
{
    public const string NEW_COLLECTION = "NEW_COLLECTION";
    public const string BAG = "BAG";
    public const string JACKET = "JACKET";
}
public class ProductDetails
{
    public string[] specification { get; set; } = Array.Empty<string>();
    public string[] features { get; set; } = Array.Empty<string>();
    public string[] additional { get; set; } = Array.Empty<string>();
}

public class CreateProductRequestDto
{
    public string name { get; set; } = string.Empty;
    public string desc { get; set; } = string.Empty;
    public decimal price { get; set; } = 0;
    public string size { get; set; } = string.Empty;
    public string type { get; set; } = string.Empty;
    public int inventory { get; set; } = 0;
    public ProductDetails details { get; set; } = new ProductDetails();
    // public IFormFile image { get; set; }
    public string color { get; set; } = string.Empty;
}
