public class ShippingMethod
{
  public Guid id { get; set; }
  public string shipping_name { get; set; } = string.Empty;
  public decimal shipping_cost { get; set; } = 0;
}

public class ShippingMethodRequest
{
  public string shipping_name { get; set; } = string.Empty;
  public decimal shipping_cost { get; set; } = 0;
}