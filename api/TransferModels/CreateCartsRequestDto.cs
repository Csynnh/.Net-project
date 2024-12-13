namespace infrastructure.DataModels;

public class CreateCartsRequestDto
{
    public Guid account_id  { get; set; }  // Primary Key, Foreign Key, liên kết tới bảng Hóa đơn
    public Guid product_id  { get; set; }  // Primary Key, Foreign Key, liên kết tới bảng Hàng hóa
    public int quantity { get; set; }  // Số lượng sản phẩm trong hóa đơn
    public DateTime added_at { get; set; }  // Số lượng sản phẩm trong hóa đơn
}