using infrastructure.Contansts;
using infrastructure.EnumVariables;

namespace infrastructure.QueryModels;

public class InvoiceFeedQuery
{
     public int id { get; set; }  // Primary Key
    public int account_id  { get; set; }  // Foreign Key, liên kết tới bảng Tài khoản
    public DateTime created_date { get; set; }  // Ngày hóa đơn được xuất
    public decimal total { get; set; }  // Ngày hóa đơn được xuất
    public string status { get; set; } =  Status.CONFIRMING;  // Trạng thái của hóa đơn (ví dụ: đã thanh toán, đang xử lý, đã hủy)
    public int checkout_method { get; set; }  // Ngày hóa đơn được xuất
    public int shipping_method { get; set; }  // Ngày hóa đơn được xuất
}