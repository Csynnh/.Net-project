namespace infrastructure.DataModels;

public class Cart
{
    public Guid account_id { get; set; }  // Primary Key, Foreign Key, liên kết tới bảng Hóa đơn
    public Guid variant_product_id { get; set; }  // Primary Key, Foreign Key, liên kết tới bảng variant_product
    public int quantity { get; set; }  // Số lượng sản phẩm trong hóa đơn
}

