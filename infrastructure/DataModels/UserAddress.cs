namespace infrastructure.DataModels;

public class UserStoredInformation
{
    public Guid id { get; set; }  // Primary Key
    public Guid account_id { get; set; }  // Primary Key
    public string info { get; set; } = string.Empty;  // Tên đăng nhập của người dùng
}

public class UserInformationRequest
{
    public string address { get; set; } = string.Empty;
    public string name { get; set; } = string.Empty;
    public string phone { get; set; } = string.Empty;
}

public class UserInformationModel : UserInformationRequest
{
    public int id { get; set; }
    public Guid account_id { get; set; }
}

public class UserInformationResponse {
    public Guid id { get; set; }
    public Guid account_id { get; set; }
    public UserInformationModel info { get; set; }
}