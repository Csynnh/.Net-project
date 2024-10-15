using infrastructure.EnumVariables;

namespace infrastructure.DataModels;

public class CreateUserAddressRequestDto
{
    public Guid account_id { get; set; }  // Primary Key
    public string address  { get; set; } = string.Empty;  // Tên đăng nhập của người dùng
}