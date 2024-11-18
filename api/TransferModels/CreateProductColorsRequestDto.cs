namespace infrastructure.DataModels;

public class CreateProductColorsRequestDto
{
    public Guid product_id  { get; set; }  // Primary Key, Foreign Key, liên kết tới bảng Hàng hóa
    public string color_name { get; set; } = string.Empty;
    public string color_code { get; set; } = string.Empty;
    public int inventory { get; set; }
    public int total { get; set; }
    public string image_url1 { get; set; } = string.Empty;
    public string image_url2 { get; set; } = string.Empty;
    public string image_url3 { get; set; } = string.Empty;
    public string image_url4 { get; set; } = string.Empty;
    public string image_url5 { get; set; } = string.Empty;   
}