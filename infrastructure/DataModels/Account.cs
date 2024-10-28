namespace infrastructure.DataModels;

public class Account
{
    public int id { get; set; }  // Primary Key
    public string username  { get; set; } = string.Empty;  // Tên đăng nhập của người dùng
    public string password  { get; set; } = string.Empty;  // Mật khẩu
    public string name { get; set; } = string.Empty;  // Họ và tên người dùng
    public string email { get; set; } = string.Empty;  // Địa chỉ email của người dùng
    public string phone_number { get; set; } = string.Empty;  // Số điện thoại của người dùng
    public string role  { get; set; } = "User";  // Vai trò: admin hoặc user
}

public class User
{
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}