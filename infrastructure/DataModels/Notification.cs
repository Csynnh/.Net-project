namespace infrastructure.DataModels;

public class Notification
{
    public Guid id { get; set; }
    public string content { get; set; }
    public DateTime created_at { get; set; }
    public string? type { get; set; }
    public Guid? account_id { get; set; }
}

public class NotificationQueryResponse
{
    public Guid id { get; set; }
    public string content { get; set; }
    public DateTime created_at { get; set; }
    public bool is_read { get; set; }
}

public class NotificationResponseModel
{
    public Guid id { get; set; }
    public NotificationContent content { get; set; }
    public DateTime created_at { get; set; }
    public bool is_read { get; set; }
}


public class NotificationContent
{
    public Guid Id { get; set; }
    public string message { get; set; }
    public DateTime createdAt { get; set; }
    public string? phone {get; set;}
}