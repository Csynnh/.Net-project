namespace infrastructure.DataModels;

public class Product
{
    public int ID_HangHoa { get; set; }  // Primary Key
    public string TenHangHoa { get; set; } = string.Empty;  // Tên của sản phẩm
    public string MoTa { get; set; } = string.Empty;  // Mô tả sản phẩm
    public decimal Gia { get; set; }  // Giá của sản phẩm
    public int SoLuongTonKho { get; set; }  // Số lượng sản phẩm còn trong kho
    public string HinhAnh { get; set; } = string.Empty;  // Đường dẫn tới hình ảnh của sản phẩm
}