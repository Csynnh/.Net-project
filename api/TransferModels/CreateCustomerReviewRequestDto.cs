using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;

namespace api.TransferModels;

// CreateReviewRequestDto.cs
public class CreateCustomerReviewRequestDto
{
    public int account_id  { get; set; }
    public int product_id  { get; set; }
    public string content { get; set; } = string.Empty;
    public int vote { get; set; }
    public DateTime created_date { get; set; }
}