namespace infrastructure.DataModels;

public class ContactHistoryFeedQuery
{
    public Guid id  { get; set; }// Primary Key
    public string account_id { get; set; } = string.Empty;  // id account thực hiện contact
    public string desc { get; set; } = string.Empty;  // Mô tả của contact
    public DateTime contact_date { get; set; } // Ngày thực hiện contact
}   