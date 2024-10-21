namespace infrastructure.QueryModels;

public class ProductFeedQuery
{
    public Guid id  { get; set; }// Primary Key
    public string prod_name { get; set; } = string.Empty;  // Tên của sản phẩm
    public string pro_desc { get; set; } = string.Empty;  // Mô tả sản phẩm
    public decimal price { get; set; }  // Giá của sản phẩm
    public decimal width { get; set; }  // Giá của sản phẩm
    public decimal height { get; set; }  // Giá của sản phẩm
    public string type { get; set; } = string.Empty;  // Giá của sản phẩm
}