namespace infrastructure.DataModels;

public class UserAddress
{
    public Guid id { get; set; }  // Primary Key
    public Guid account_id { get; set; }  // Primary Key
    public string address { get; set; } = string.Empty;  // Tên đăng nhập của người dùng
}

public class UserAddressRequest
{
    public string address { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string phone { get; set; } = string.Empty;
}

public class UserAddressModel : UserAddressRequest
{
    public int id { get; set; }
    public Guid account_id { get; set; }
}

public class UserAddressResponse {
    public Guid id { get; set; }
    public Guid account_id { get; set; }
    public UserAddressModel address { get; set; }
}