
namespace GoldBank.Models.RequestModels
{
    public class PaymentOrderRM
    {
        public int PaymentOrderId { get; set; }
        public int PaymentId { get; set; }
        public int OrderId { get; set; }
        public decimal TotalAmount { get; set; }  
    }
}
