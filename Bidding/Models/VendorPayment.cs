namespace GoldBank.Models
{
    public class VendorPayment : BaseDomain
    {
        public int VendorPaymentId { get; set; } 
        public int VendorId { get; set; }        
        public decimal Amount { get; set; }      
        public decimal CashAmount { get; set; }  
        public bool IsConfirmed { get; set; }    
        public int VendorPaymentTypeId { get; set; } 
        public int PaymentTypeId { get; set; }
    }
}
