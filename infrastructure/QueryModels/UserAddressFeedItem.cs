using infrastructure.EnumVariables;

namespace infrastructure.DataModels;

public class UserAddressFeedQuery
{
    public int id { get; set; }  // Primary Key
    public int account_id { get; set; }  // Primary Key
    public string address  { get; set; } = string.Empty;  // Tên đăng nhập của người dùng
}