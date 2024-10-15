using System.ComponentModel.DataAnnotations;

namespace api.TransferModels;

// UpdateInvoiceDetailRequestDto.cs
public class UpdateInvoiceDetailRequestDto
{
    public Guid invoices_id  { get; set; }
    public Guid product_id  { get; set; }
    public int amount { get; set; }
    public decimal price { get; set; }
}