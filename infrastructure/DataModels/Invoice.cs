using infrastructure.EnumVariables;

namespace infrastructure.DataModels;

public class Invoice
{
    public Guid id { get; set; }  // Primary Key
    public Guid account_id  { get; set; }  // Foreign Key, liên kết tới bảng Tài khoản
    public DateTime created_date { get; set; }  // Ngày hóa đơn được xuất
    public decimal total { get; set; }  // Ngày hóa đơn được xuất
    public Status status { get; set; } =  Status.processing;  // Trạng thái của hóa đơn (ví dụ: đã thanh toán, đang xử lý, đã hủy)
    public Checkout_method checkout_method { get; set; }  // Ngày hóa đơn được xuất
    public Shipping_method shipping_method { get; set; }  // Ngày hóa đơn được xuất
}