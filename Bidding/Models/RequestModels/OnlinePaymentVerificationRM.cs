namespace GoldBank.Models.RequestModels
{
    public class OnlinePaymentVerificationRM
    {
        public bool? IsVerficationRequested { get; set; }
        public bool? IsVerficationPassed { get; set; }
        public bool? IsVerficationFailed { get; set; }
        public int? CustomerId { get; set; }
        public int? CompanyAccountId { get; set; }
    }
    public class OnlinePaymentVerificationVM : BaseDomain
    {
        public int PaymentId { get; set; }
        public int OnlinePaymentId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public int CustomerAccountId { get; set; }
        public string CustomerAccount { get; set; }
        public string CustomerAccountNumber { get; set; }
        public int CompanyAccountId { get; set; }
        public string CompanyAccount { get; set; }
        public bool IsVerficationRequested { get; set; }
        public bool IsVerficationPassed { get; set; }
        public bool IsVerficationFailed { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        
    }
    public class OnlinePaymentSummary
    {
        public int Pending {  get; set; }
        public int Passed { get; set; }
        public int Rejected { get; set; }
    }
}
