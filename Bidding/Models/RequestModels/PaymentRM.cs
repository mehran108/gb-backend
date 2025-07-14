namespace GoldBank.Models.RequestModels
{
    public  class PaymentRM
    {
        public AddPaymentRequest PaymentModel { get; set; }
        public OnlinePaymentRM OnlinePayment { get; set; }
    }
    public class AddPaymentRequest
    {
        public int PaymentId { get; set; }
        public int CustomerId { get; set; }
        public int PaymentTypeId { get; set; }
        public decimal TotalAmount { get; set; }
        public int CreatedBy { get; set; }
        public List<PaymentOrderRM> PaymentOrderRM { get; set; }

    }
    public class PaymentOrderRM
    {
        public int? PaymentOrderId { get; set; } = 0;
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }
    }
    public class OnlinePaymentRM
    {
        public int OnlinePaymentId { get; set; }
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public int CustomerAccountId { get; set; }
        public int CompanyAccountId { get; set; }
        public string CustomerAccountNumber { get; set; }
        public bool IsVerificationRequested { get; set; }
        public bool IsVerificationPassed { get; set; }
        public bool IsVerificationFailed { get; set; }
        public int CreatedBy { get; set; }
        public List<OnlinePaymentDocumentRM> OnlinePaymentDocumentRM { get; set; }
    }
    public class OnlinePaymentDocumentRM
    {
        public int OnlinePaymentId { get; set; }
        public int DocumentId { get; set; }
        public bool IsPrimary { get; set; }
        public int OnlinePaymentDocumentId { get; set; }
    }
    public class CardPaymentRM
    {
        public int CardPaymentId { get; set; }
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ReceiptNo { get; set; }
        public string TransactionId { get; set; }
        public int LastFourDigits { get; set; }
        public int CreatedBy { get; set; }
    }
}
