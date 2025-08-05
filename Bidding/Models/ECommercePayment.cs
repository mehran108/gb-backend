namespace GoldBank.Models
{
    public class ECommercePayment : BaseDomain
    {
        public int ECommercePaymentId { get; set; }
        public string ProductIds { get; set; }
        public int CustomerId { get; set; }
        public string BasketId { get; set; }
        public string MerchantId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public int DelieveryMethodId { get; set; }
        public DateTime EstDelieveryDate { get; set; }
        public decimal ShippingCost { get; set; }
        public string DelieveryAddress { get; set; }
        public string Status { get; set; }
        public string? TransactionDetail { get; set; }
        public string? TransactionId { get; set; }
    }
}
