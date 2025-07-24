namespace GoldBank.Models
{
    public class PaymentTransaction
    {
        public string? MERCHANT_ID { get; set; }
        public string? SECURED_KEY { get; set; }
        public string? BASKET_ID { get; set; }
        public decimal? TXNAMT { get; set; }
        public string? CURRENCY_CODE { get; set; }
    }
}
