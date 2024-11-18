namespace infrastructure.DataModels;

public class CreateContactHistoryRequestDto
{
    public Guid account_id { get; set; }  // id account thực hiện contact
    public string contact_details { get; set; } = string.Empty;  // Mô tả của contact
    public DateTime contact_date { get; set; } // Ngày thực hiện contact
}   