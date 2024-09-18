using infrastructure.EnumVariables;

namespace infrastructure.DataModels;

public class Invoice
{
    public int id { get; set; }  // Primary Key
    public int account_id  { get; set; }  // Foreign Key, liên kết tới bảng Tài khoản
    public DateTime created_date { get; set; }  // Ngày hóa đơn được xuất
    public decimal price { get; set; }  // Tổng tiền của hóa đơn
    public Status status { get; set; } =  Status.processing;  // Trạng thái của hóa đơn (ví dụ: đã thanh toán, đang xử lý, đã hủy)
}