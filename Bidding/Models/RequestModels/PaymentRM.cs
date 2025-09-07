namespace GoldBank.Models.RequestModels
{
    public class PaymentRM
    {
        public AddPaymentRequest PaymentModel { get; set; }
        public AddOnlinePaymentRequest OnlinePayment { get; set; }
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
    public class AddOnlinePaymentRequest
    {
        public int OnlinePaymentId { get; set; }
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public int CustomerAccountId { get; set; }
        public int CompanyAccountId { get; set; }
        public string CustomerAccountNumber { get; set; }
        public int CreatedBy { get; set; }
        public List<OnlinePaymentDocumentRM> OnlinePaymentDocumentRM { get; set; }
    }
    public class OnlinePaymentDocumentRM
    {
        public int? OnlinePaymentId { get; set; }
        public int? VendorOnlinePaymentId { get; set; }
        public int? VendorPaymentId { get; set; }
        public int DocumentId { get; set; }
        public bool IsPrimary { get; set; }
        public int? OnlinePaymentDocumentId { get; set; }
    }
    public class VerifyOnlinePaymentRequest
    {
        public int OnlinePaymentId { get; set; }
        public bool IsApproved { get; set; }
        public string Notes { get; set; }
    }
    public class ConfirmPaymentRequest
    {
        public int PaymentId { get; set; }
        public decimal CashAmount { get; set; }
        public List<CardPayment> CardPayment { get; set; }
        public int CreatedBy { get; set; }
    }
    public class CardPayment
    {
        public string TransactionId { get; set; }
        public DateTime TransactionDate { get; set; }
        public string ReceiptNo { get; set; }
        public decimal Amount { get; set; }
        public int LastFourDigit { get; set; }
        public int? CompanyAccountId { get; set; }
    }
    public class AddVendorPaymentRequest
    {
        public int VendorPaymentId { get; set; }
        public int VendorId { get; set; }
        public int PaymentTypeId { get; set; }
        public int VendorPaymentTypeId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? GoldAmount { get; set; }
        public int? VendorGoldPaymentTypeId { get; set; }
        public decimal? CashAmount { get; set; }
        public int CreatedBy { get; set; }

    }
    public class AddVendorOnlinePaymentRequest
    {
        public int VendorOnlinePaymentId { get; set; }
        public int VendorPaymentId { get; set; }
        public decimal? Amount { get; set; }
        public decimal? GoldAmount { get; set; }
        public string TransactionId { get; set; }
        public int VendorAccountId { get; set; }
        public int CompanyAccountId { get; set; }
        public string VendorAccountNumber { get; set; }
        public int CreatedBy { get; set; }
        public List<OnlinePaymentDocumentRM> OnlinePaymentDocumentRM { get; set; }
    }

    public class ConfirmVendorPaymentRequest
    {
        public int VendorPaymentId { get; set; }
        public int? VendorGoldPaymentTypeId { get; set; }
        public decimal? CashAmount { get; set; }
        public decimal? GoldAmount { get; set; }
        public int? ProductId { get; set; }
        public string Notes { get; set; }
        public int CreatedBy { get; set; }
        public List<OnlinePaymentDocumentRM>? PaymentDocumentRM { get; set; }
    }

}
