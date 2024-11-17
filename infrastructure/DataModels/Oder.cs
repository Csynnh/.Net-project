using System.Text.Json;
using infrastructure.Contansts;

namespace infrastructure.DataModels;

public class Invoice
{
    public Guid id { get; set; }
    public Guid account_id { get; set; }
    public DateTime created_at { get; set; }
    public decimal total { get; set; }
    public string status { get; set; } = Status.PROCESSING;
    public string paymend_method { get; set; } = PAYMENTMETHOD.GO_TO_STORE;
    public string shipping_method { get; set; } = SHIPPINGMETHOD.GO_TO_STORE;

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

public class ListOderResponseModel
{
    public Guid id { get; set; }
    public Guid account_id { get; set; }
    public DateTime created_at { get; set; }
    public decimal total { get; set; }
    public string status { get; set; }
    public object paymend_method { get; set; }
    public UserInformationRequest user_info { get; set; }
    public ShippingMethod shipping_method { get; set; }
    public object list_products { get; set; }
}

public class ProductCheckout
{
    public string id { get; set; }
    public string name { get; set; }
    public decimal price { get; set; }
    public List<Variant> variants { get; set; }
}

public class Variant
{
    public Guid id { get; set; }
    public string color { get; set; }
    public string image { get; set; }
    public int count { get; set; }
}