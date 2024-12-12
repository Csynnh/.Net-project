using Microsoft.AspNetCore.Http;


namespace infrastructure.DataModels;

public class Product
{
    public Guid id { get; set; }// Primary Key
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

    public string ShortDesc { get; set; } = string.Empty;
    public string Material { get; set; } = string.Empty;
    public string Waterproof { get; set; } = string.Empty;
    public string CareInstructions {get; set;} = string.Empty;
    public string Dimensions { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
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

public class CreateProductModel
{
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public decimal Price { get; set; }
    public string? Size { get; set; }
    public string Type { get; set; }
    public int? Inventory { get; set; }
    public ProductDetails Details { get; set; }
    public ProductImages Images { get; set; }
    public string? Color { get; set; }
}

public class ProductImages
{
    public IFormFile ImageThumbnail { get; set; }
    public List<IFormFile> AdditionalImages { get; set; }
}

public class UpdateProductModel
{
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public decimal Price { get; set; }
    public string? Size { get; set; }
    public string Type { get; set; }
    public int? Inventory { get; set; }
    public ProductDetails Details { get; set; }
    public ProductImagesUrl Images { get; set; }
    public string? Color { get; set; }
}

public class ProductImagesUrl
{
    public string ImageThumbnail { get; set; }
    public List<string> AdditionalImages { get; set; }
}


public class UploadImageRequest {
    public IFormFile Image { get; set; }
}