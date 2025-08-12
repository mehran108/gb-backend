namespace GoldBank.Models
{
    public class OrderDiscount : BaseDomain
    {
        public int OrderDiscountId { get; set; }
        public int OrderId { get; set; }
        public int DiscountId { get; set; }
        public int DiscountTypeId { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
        public string DiscountCode {  get; set; }
    }
}
