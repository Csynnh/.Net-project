using System.ComponentModel;
using System.Reflection;
using System.Text.Json;

namespace infrastructure.DataModels;

public class PaymentMethod
{
  public Guid id { get; set; } = Guid.Empty;
  public Guid account_id { get; set; } = Guid.Empty;
  public string payment_method { get; set; } = string.Empty;
}

public class PaymentMethodRequest
{
  public Guid account_id { get; set; } = Guid.Empty;
  public PaymentMethodModel payment_method { get; set; } = new PaymentMethodModel();
}

public class paymentMethodResponse
{
  public Guid id { get; set; } = Guid.Empty;
  public Guid account_id { get; set; } = Guid.Empty;
  public object payment_method { get; set; } = new object();
}

public class PaymentMethodModel
{
  public string payment_name { get; set; } = string.Empty;
  public string card_number { get; set; } = string.Empty;
  public string card_holder_name { get; set; } = string.Empty;
  public string expiration_date { get; set; } = string.Empty;
  public string cvv { get; set; } = string.Empty;
  public string phone_number { get; set; } = string.Empty;

  [DefaultValue(false)]
  public bool is_payment_in_store { get; set; } = false;

  public object ToUpper()
  {
    foreach (PropertyInfo property in GetType().GetProperties())
    {
      if (property.PropertyType == typeof(string) && property.GetValue(this) != null)
      {
        var value = property.GetValue(this) as string;
        if (!string.IsNullOrEmpty(value))
        {
          property.SetValue(this, value.ToUpper());
        }
      }
    }
    var stringProperties = GetType().GetProperties()
      .Where(p => p.PropertyType == typeof(string) && p.GetValue(this) != null)
      .ToDictionary(p => p.Name, p => (string)p.GetValue(this)!);

    var filteredPaymentMethod = stringProperties
      .Where(kv => !string.IsNullOrEmpty(kv.Value))
      .ToDictionary(kv => kv.Key, kv => kv.Value.ToUpper());
    return filteredPaymentMethod;
  }
}