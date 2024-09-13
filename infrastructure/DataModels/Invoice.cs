namespace infrastructure.DataModels;

public class Invoice
{
    public int ID_HoaDon { get; set; }  // Primary Key
    public int ID_TaiKhoan { get; set; }  // Foreign Key, liên kết tới bảng Tài khoản
    public DateTime NgayXuatHoaDon { get; set; }  // Ngày hóa đơn được xuất
    public decimal TongTien { get; set; }  // Tổng tiền của hóa đơn
    public string TrangThai { get; set; } = "Đang xử lý";  // Trạng thái của hóa đơn (ví dụ: đã thanh toán, đang xử lý, đã hủy)
}