using GoldBank.Models;

namespace GoldBank.Models
{
    public class Payment : BaseDomain
    {
        public int PaymentId { get; set; }
        public int CustomerId { get; set; }
        public int PaymentTypeId { get; set; }
        public decimal? TotalAmount { get; set; }
        public decimal? CashAmount { get; set; }
        public bool IsConfirmed { get; set; }
        public List<PaymentOrder> PaymentOrder {  get; set; }
    }
    public class PaymentOrder : BaseDomain
    {
        public int PaymentOrderId { get; set; }
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }

    }
}


