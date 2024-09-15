namespace infrastructure.DataModels;

public class Product
{
    public int id  { get; set; }  // Primary Key
    public string name { get; set; } = string.Empty;  // Tên của sản phẩm
    public string desc { get; set; } = string.Empty;  // Mô tả sản phẩm
    public decimal price { get; set; }  // Giá của sản phẩm
    public int inventory { get; set; }  // Số lượng sản phẩm còn trong kho
    public string image_url { get; set; } = string.Empty;  // Đường dẫn tới hình ảnh của sản phẩm
}