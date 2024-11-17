using infrastructure.Contansts;
using infrastructure.DataModels;
namespace api.Request;



public class CreateOderRequest
{
    public Guid account_id { get; set; }
    public string paymentMethod { get; set; } = PAYMENTMETHOD.GO_TO_STORE;
    public DateTime created_date { get; set; }
    public UserInformationRequest userInfo { get; set; }
    public string shippingMethod { get; set; } = SHIPPINGMETHOD.GO_TO_STORE;
    public decimal price { get; set; }
    public List<ProductCheckout> products { get; set; } = new List<ProductCheckout>();

}
