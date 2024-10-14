using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;

namespace api.TransferModels;

// CreateInvoiceRequestDto.cs
public class CreateInvoiceRequestDto
{
    public int id  { get; set; }  
    public int account_id  { get; set; }  
    public DateTime created_date { get; set; }  
    public decimal price { get; set; }  
    public string status { get; set; } = "Đang xử lý";  
    public int checkout_method { get; set; }
    public int shipping_method { get; set; }
}
