using System.ComponentModel.DataAnnotations;
using infrastructure.EnumVariables;

namespace api.TransferModels;


// UpdateReviewRequestDto.cs
public class UpdateCustomerReviewRequestDto
{
    public Guid id { get; set; }
    public Guid account_id  { get; set; }
    public Guid product_id  { get; set; }
    public string content { get; set; }  = string.Empty;
    public Rating vote { get; set; }
    public DateTime created_date { get; set; }
}