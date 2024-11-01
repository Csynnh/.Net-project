using System.ComponentModel.DataAnnotations;
using api.CustomDataAnnotations;
using infrastructure.EnumVariables;

namespace api.TransferModels;

// CreateInvoiceRequestDto.cs
public class CreateOderRequestDto
{
    public Guid account_id  { get; set; }
    public Guid payment_method_id { get; set; }
    public DateTime created_date { get; set; }
    public Guid shipping_method_id { get; set; }
    public Guid user_stored_info_id { get; set; }
    public decimal price { get; set; }
    public string status { get; set; } = Status.PROCESSING;

}
