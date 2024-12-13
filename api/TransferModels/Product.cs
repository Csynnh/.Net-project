namespace Api.TransferModels
{

  public class ProductDetails
  {
    public string[] specification { get; set; } = Array.Empty<string>();
    public string[] features { get; set; } = Array.Empty<string>();
    public string[] additional { get; set; } = Array.Empty<string>();
  }

  public class ProductImages
  {
    public string ImageThumbnail { get; set; }
    public List<string> AdditionalImages { get; set; }
  }

  public class CreateProductModel
  {
    public string ProductName { get; set; }
    public string ProductDescription { get; set; }
    public decimal Price { get; set; }
    public string Size { get; set; }
    public string Type { get; set; }
    public int Inventory { get; set; }
    public ProductDetails Details { get; set; }
    public ProductImages Images { get; set; }
    public string Color { get; set; }
  }
}