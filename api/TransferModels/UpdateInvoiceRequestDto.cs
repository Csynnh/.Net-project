using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;

// UpdateInvoiceRequestDto.cs
public class UpdateInvoiceRequestDto
{
    public int id  { get; set; }
    public int account_id  { get; set; }
    public DateTime created_date { get; set; }
    public decimal price { get; set; }
    public string status { get; set; } = string.Empty;
}
