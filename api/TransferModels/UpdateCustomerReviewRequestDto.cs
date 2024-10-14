using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;


// UpdateReviewRequestDto.cs
public class UpdateCustomerReviewRequestDto
{
    public int id { get; set; }
    public int account_id  { get; set; }
    public int product_id  { get; set; }
    public string content { get; set; }  = string.Empty;
    public int vote { get; set; }
    public DateTime created_date { get; set; }
}