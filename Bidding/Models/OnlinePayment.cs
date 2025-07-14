
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
        public bool IsVerficationRequested { get; set; }
        public bool IsVerficationPassed { get; set; }
        public bool IsVerficationFailed { get; set; }

    }
    public class OnlinePaymentDocument : BaseDomain
    {
        public int OnlinePaymentDocumentId { get; set; }
        public int OnlinePaymentId { get; set; }
        public int DocumentId { get; set; }
        public bool IsPrimary { get; set; }

    }
}
   