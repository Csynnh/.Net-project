namespace infrastructure.DataModels;

public class CartInQueryResult
{

    public Guid Id { get; set; }  // Primary Key, Foreign Key, liên kết tới bảng Hóa đơn
    public string product_name { get; set; }  // Primary Key, Foreign Key, liên kết tới bảng variant_product
    public decimal product_price { get; set; }  // Số lượng sản phẩm trong hóa đơn

    public int product_quantity { get; set; }

    public string variants { get; set; }

    public string product_color { get; set; }
}
