using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;

namespace api.TransferModels;

public class CreateInvoiceDetailRequestDto
{
    public Guid invoices_id  { get; set; }
    public Guid product_id  { get; set; }
    public int amount { get; set; }
    public decimal price { get; set; }
}