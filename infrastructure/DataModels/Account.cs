namespace infrastructure.DataModels;

public class Account
{
    public int ID_TaiKhoan { get; set; }  // Primary Key
    public string TenDangNhap { get; set; } = string.Empty;  // Tên đăng nhập của người dùng
    public string MatKhau { get; set; } = string.Empty;  // Mật khẩu
    public string HoTen { get; set; } = string.Empty;  // Họ và tên người dùng
    public string Email { get; set; } = string.Empty;  // Địa chỉ email của người dùng
    public string SoDienThoai { get; set; } = string.Empty;  // Số điện thoại của người dùng
    public string DiaChi { get; set; } = string.Empty;  // Địa chỉ của người dùng
    public string VaiTro { get; set; } = "user";  // Vai trò: admin hoặc user
}