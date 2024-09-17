using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;

namespace api.TransferModels;

public class CreateInvoiceDetailRequestDto
{
    public int invoices_id  { get; set; }
    public int product_id  { get; set; }
    public int amount { get; set; }
    public decimal price { get; set; }
}