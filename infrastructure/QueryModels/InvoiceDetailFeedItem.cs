namespace infrastructure.QueryModels;

public class InvoiceDetailFeedQuey
{
    public int invoices_id  { get; set; }  // Primary Key, Foreign Key, liên kết tới bảng Hóa đơn
    public int product_id  { get; set; }  // Primary Key, Foreign Key, liên kết tới bảng Hàng hóa
    public int amount { get; set; }  // Số lượng sản phẩm trong hóa đơn
    public decimal price { get; set; }  // Giá của sản phẩm tại thời điểm mua
}