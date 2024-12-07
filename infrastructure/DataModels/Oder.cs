using System.Text.Json;
using infrastructure.Contansts;

namespace infrastructure.DataModels;

public class Invoice
{
    public Guid id { get; set; }
    public Guid account_id { get; set; }
    public DateTime created_at { get; set; }
    public decimal total { get; set; }
    public string status { get; set; } = Status.CONFIRMING;
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
    public Guid id { get; set; }
    public List<Variant> variants { get; set; }
}

public class Variant
{
    public Guid id { get; set; }
    public int count { get; set; }
}

public class OrderStatusSummary
{
    public string Status { get; set; }
    public int Total { get; set; }
}

public class RetrieveChartDataResponse
{
    public Guid id { get; set; }
    public int tax_rate { get; set; }
    public int units_sold { get; set; }
    public decimal price { get; set; }
    public string item_name { get; set; }
    public string type { get; set; }
    public DateTime created_at { get; set; }
}
