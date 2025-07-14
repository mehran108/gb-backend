using Microsoft.AspNetCore.Http.HttpResults;

namespace GoldBank.Models
{
    public class OnlinePayment : BaseDomain
    {
        public int OnlinePaymentId { get; set; }
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public int CustomerAccountId { get; set; }
        public string CompanyAccountId { get; set; }
        public string CustomerAccountNumber { get; set; }

        public bool IsVerficationRequested { get; set; }
        public bool IsVerficationPassed { get; set; }
        public bool IsVerficationFailed { get; set; }

    }
}
   