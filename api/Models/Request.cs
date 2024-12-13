using System.Globalization;
using infrastructure.Contansts;
using infrastructure.DataModels;
namespace api.Request;



public class CreateOderRequest
{
    public Guid account_id { get; set; }
    public string paymentMethod { get; set; } = PAYMENTMETHOD.GO_TO_STORE;
    public UserInformationRequest userInfo { get; set; }
    public string shippingMethod { get; set; } = SHIPPINGMETHOD.GO_TO_STORE;
    public decimal price { get; set; }
    public List<ProductCheckout> products { get; set; } = new List<ProductCheckout>();

}

public class EmailSentByUserRequest
{
    public string userName { get; set; }

    public string userGender { get; set; }

    public string userPhone { get; set; }

    public string typeOfProduct { get; set; }

    public string message { get; set; }
};

public class RetrieveChartDataRequest
{
    public string ChartType { get; set; } = CHARTTYPE.DAILY;
}

public class GoogleRequest
{
    public string Token { get; set; }
}