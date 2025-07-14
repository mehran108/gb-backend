using Microsoft.AspNetCore.Http.HttpResults;

namespace GoldBank.Models
{
    public class CardPayment : BaseDomain
    {
        public int CardPaymentId { get; set; }
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionId { get; set; }
        public string ReceiptNo { get; set; }
        public DateTime TransactionDate { get; set; }
        public int LastFourDigits { get; set; }
    }
   
}