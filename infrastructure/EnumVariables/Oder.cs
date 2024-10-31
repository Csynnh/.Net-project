namespace infrastructure.EnumVariables
{
    public class Status
    {
        public static string PROCESSING = "PROCESSING";
        public static string COMPLETED = "COMPLETED";
        public static string CANCELLED = "CANCELLED";
    }

    public class PaymentMethod
    {
        public static string CREDIT_CARD = "CREDIT_CARD";
        public static string GO_TO_STORE = "GO_TO_STORE";
        public static string BANK_TRANSFERS = "BANK_TRANSFERS";
        public static string MOMO = "MOMO";
    }

    public class ShippingMethod
    {
        public static string GO_TO_STORE = "GO_TO_STORE";
        public static string STANDARD_SHIPPING = "STANDARD_SHIPPING";
        public static string EXPRESS_SHIPPING = "EXPRESS_SHIPPING";
    }
}