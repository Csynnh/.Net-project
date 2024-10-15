using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using infrastructure.EnumVariables;

namespace api.TransferModels;

// CreateInvoiceRequestDto.cs
public class CreateInvoiceRequestDto
{
    public Guid account_id  { get; set; }  
    public DateTime created_date { get; set; }  
    public decimal price { get; set; }  
    public Status status { get; set; } = Status.processing;  
    public Checkout_method checkout_method { get; set; }
    public Shipping_method shipping_method { get; set; }

}
