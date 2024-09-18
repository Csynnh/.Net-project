using infrastructure.EnumVariables;

namespace infrastructure.DataModels;

public class CustomerReview
{
    public int id { get; set; }  // Primary Key
    public int account_id  { get; set; }  // Foreign Key, liên kết tới bảng Tài khoản
    public int product_id  { get; set; }  // Foreign Key, liên kết tới bảng Hàng hóa
    public string content { get; set; } = string.Empty;  // Nội dung của nhận xét
    public Rating vote { get; set; }  // Đánh giá (thang điểm từ 1 đến 5 sao)
    public DateTime created_date { get; set; }  // Ngày người dùng viết nhận xét
}