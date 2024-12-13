
namespace infrastructure.Contansts;

public class Status
{
    public static string CONFIRMING = "NEED_CONFIRM";
    public static string PREPARING = "PREPARING";
    public static string SHIPPING = "SHIPPING";
    public static string SUCCESSFULLY = "SUCCESSFULLY";
}

public class PAYMENTMETHOD
{
    public static string CREDIT_CARD = "CREDIT_CARD";
    public static string GO_TO_STORE = "GO_TO_STORE";
    public static string BANK_TRANSFERS = "BANK_TRANSFERS";
    public static string MOMO = "MOMO";

}

public class SHIPPINGMETHOD
{
    public static string GO_TO_STORE = "GO_TO_STORE";
    public static string STANDARD_SHIPPING = "STANDARD_SHIPPING";
    public static string EXPRESS_SHIPPING = "EXPRESS_SHIPPING";
}

public class CHARTTYPE
{
    public static string DAILY = "DAILY";
    public static string WEEKLY = "WEEKLY";
    public static string MONTHLY = "MONTHLY";
}