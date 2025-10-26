
namespace GoldBank.Models
{
    public class OnlinePayment : BaseDomain
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
        public string? CustomerAccount {  get; set; }
        public string? CompanyAccount {  get; set; }
        public List<OnlinePaymentDocument> OnlinePaymentDocument { get;set; }

    }
    public class OnlinePaymentDocument : BaseDomain
    {
        public int OnlinePaymentDocumentId { get; set; }
        public int OnlinePaymentId { get; set; }
        public int DocumentId { get; set; }
        public bool IsPrimary { get; set; }
        public string Url { get; set; }
    }
}
   