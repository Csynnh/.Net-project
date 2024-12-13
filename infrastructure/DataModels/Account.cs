namespace infrastructure.DataModels;

public class Account
{
    public Guid id { get; set; }  // Primary Key
    public string username { get; set; } = string.Empty;  // Tên đăng nhập của người dùng
    public string password { get; set; } = string.Empty;  // Mật khẩu
    public string name { get; set; } = string.Empty;  // Họ và tên người dùng
    public string email { get; set; } = string.Empty;  // Địa chỉ email của người dùng
    public string phone_number { get; set; } = string.Empty;  // Số điện thoại của người dùng
    public string role { get; set; } = "User";  // Vai trò: admin hoặc user
}

public class User
{
    public Guid Id { get; set; } = Guid.Empty;
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}

public class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class TokenModel
{
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiredTime { get; set; }
}

public class LoginResponse
{
    public string Name { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public Guid AccountId { get; set; } = Guid.Empty;
    public DateTime ExpiredTime { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;

}


public class OtpRequest
{
    public string Email { get; set; }
}
public class OtpConfirmRequest
{
    public string Email { get; set; }
    public string Otp { get; set; }
}

public class PasswordChangeRequest
{
    public string Email { get; set; }
    public string NewPassword { get; set; }
    public string Otp { get; set; }
}

public class OtpRessponse
{
    public string Otp { get; set; }
    public DateTime Created_At { get; set; } = DateTime.UtcNow;
}