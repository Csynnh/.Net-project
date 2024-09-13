namespace infrastructure.DataModels;

public class InvoiceDetail
{
    public int ID_HoaDon { get; set; }  // Primary Key, Foreign Key, liên kết tới bảng Hóa đơn
    public int ID_HangHoa { get; set; }  // Primary Key, Foreign Key, liên kết tới bảng Hàng hóa
    public int SoLuong { get; set; }  // Số lượng sản phẩm trong hóa đơn
    public decimal Gia { get; set; }  // Giá của sản phẩm tại thời điểm mua
}