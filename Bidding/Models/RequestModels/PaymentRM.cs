namespace GoldBank.Models.RequestModels
{
    public  class PaymentRM
    {
        public int PaymentId { get; set; }
        public int CustomerId { get; set; }
        public int PaymentTypeId { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CashPayment {  get; set; }
        public bool IsConfirmed { get; set; }
        public int CreatedBy { get; set; }
        public List<PaymentOrderRM> PaymentOrderRM { get; set; }
    }
}
