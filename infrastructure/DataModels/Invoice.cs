using infrastructure.EnumVariables;

namespace infrastructure.DataModels;

public class Invoice
{
    public Guid id { get; set; }
    public Guid account_id { get; set; }
    public DateTime created_at { get; set; }
    public decimal total { get; set; }
    public string status { get; set; } = Status.PROCESSING;
    public string paymend_method { get; set; } = PaymentMethod.GO_TO_STORE;
    public string shipping_method { get; set; } = ShippingMethod.GO_TO_STORE;

}


public class OderResponseModel
{
    public Guid id { get; set; }
    public Guid account_id { get; set; }
    public DateTime created_at { get; set; }
    public decimal total { get; set; }
    public string status { get; set; }
    public object paymend_method { get; set; }
    public object user_info { get; set; }
}
