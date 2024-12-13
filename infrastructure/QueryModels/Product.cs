using Microsoft.VisualBasic;

namespace infrastructure.QueryModels;

public class ProductModel
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? Size { get; set; }
    public string? Color { get; set; }
    public string Type { get; set; }
    public decimal Price { get; set; }
    public int? Inventory { get; set; }
    public required string Details { get; set; }
    public string Images { get; set; }
}


public class ProductModelResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public object Details { get; set; }
    public object Variants { get; set; } // Can be ProductVariant or string
    public string Type { get; set; }
}

public class ProductImagesModel
{
    public string ImageThumbnail { get; set; }
    public List<string> AdditionalImages { get; set; }
}

public class ProductVariant
{
    public Guid Id { get; set; }
    public int Inventory { get; set; }
    public ProductImagesModel Images { get; set; }
    public string Size { get; set; }
    public string Color { get; set; }
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

public class Color
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
}

public class Size
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Name { get; set; }
}

public class ListProductByTypeResponse
{
    public string Type { get; set; }
    public List<ProductModelResponse> Products { get; set; }
}

public class ProductOrderResponse: ProductModelResponse
{
    public Guid OrderId { get; set; }
    public string OrderStatus { get; set; }
}

public class ListProductByOderStatusResponse
{
    public Guid OrderId { get; set; }
    public string OrderStatus { get; set; }
    public List<ProductOrderResponse> Products { get; set; }
}