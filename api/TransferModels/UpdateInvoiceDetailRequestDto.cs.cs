using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;

// UpdateInvoiceDetailRequestDto.cs
public class UpdateInvoiceDetailRequestDto
{
    public int invoices_id  { get; set; }
    public int product_id  { get; set; }
    public int amount { get; set; }
    public decimal price { get; set; }
}