using System.ComponentModel.DataAnnotations;
using infrastructure.Contansts;
using infrastructure.EnumVariables;

namespace api.TransferModels;

// UpdateInvoiceRequestDto.cs
public class UpdateInvoiceRequestDto
{
    public Guid id  { get; set; }
    public Guid account_id  { get; set; }
    public DateTime created_date { get; set; }
    public decimal price { get; set; }
    public string status { get; set; } = Status.CONFIRMING;
    public Checkout_method checkout_method { get; set; }
    public Shipping_method shipping_method { get; set; }
}
