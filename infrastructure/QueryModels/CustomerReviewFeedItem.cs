namespace infrastructure.QueryModels;

public class CustomerReviewFeedQuery
{
    public int ID_NhanXet { get; set; }  // Primary Key
    public int ID_TaiKhoan { get; set; }  // Foreign Key, liên kết tới bảng Tài khoản
    public int ID_HangHoa { get; set; }  // Foreign Key, liên kết tới bảng Hàng hóa
    public string NoiDung { get; set; } = string.Empty;  // Nội dung của nhận xét
    public int DanhGia { get; set; }  // Đánh giá (thang điểm từ 1 đến 5 sao)
    public DateTime NgayNhanXet { get; set; }  // Ngày người dùng viết nhận xét
}