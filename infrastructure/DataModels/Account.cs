using infrastructure.EnumVariables;

namespace infrastructure.DataModels;

public class Account
{
    public int id { get; set; }  // Primary Key
    public string username  { get; set; } = string.Empty;  // Tên đăng nhập của người dùng
    public string password  { get; set; } = string.Empty;  // Mật khẩu
    public string name { get; set; } = string.Empty;  // Họ và tên người dùng
    public string email { get; set; } = string.Empty;  // Địa chỉ email của người dùng
    public string phone_number { get; set; } = string.Empty;  // Số điện thoại của người dùng
    public string address  { get; set; } = string.Empty;  // Địa chỉ của người dùng
    public Role role  { get; set; } = Role.user;  // Vai trò: admin hoặc user
}