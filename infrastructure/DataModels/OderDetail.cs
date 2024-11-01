namespace infrastructure.DataModels;

public class InvoiceDetail
{
    public int invoices_id  { get; set; }  // Primary Key, Foreign Key, liên kết tới bảng Hóa đơn
    public int product_id  { get; set; }  // Primary Key, Foreign Key, liên kết tới bảng Hàng hóa
    public int amount { get; set; }  // Số lượng sản phẩm trong hóa đơn
    public decimal price { get; set; }  // Giá của sản phẩm tại thời điểm mua
}

public class OderDetailRequest
{
    public Guid order_id {get; set;}
    public Guid account_id {get; set;}
    public Guid product_variant_id {get; set;}
    public int quantity {get; set;}
    public decimal price {get; set;}
}

public class OderDetailResponse
{
    public Guid id {get; set;}
    public Guid order_id {get; set;}
    public Guid product_variant_id {get; set;}
    public int quantity {get; set;}
    public decimal price {get; set;}
}