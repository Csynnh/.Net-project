using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using infrastructure.EnumVariables;

namespace api.TransferModels;

// CreateReviewRequestDto.cs
public class CreateCustomerReviewRequestDto
{
    public Guid account_id  { get; set; }
    public Guid product_id  { get; set; }
    public string content { get; set; } = string.Empty;
    public Rating vote { get; set; }
    public DateTime created_date { get; set; }
}