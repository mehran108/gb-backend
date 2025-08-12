namespace GoldBank.Models
{
    public class OrderDiscount : BaseDomain
    {
        public int? OrderDiscountId { get; set; }
        public int? OrderId { get; set; }
        public int? DiscountId { get; set; }
        public int DiscountTypeId { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string Code {  get; set; }
    }
    public class DiscountCodeVerification 
    {
        public int DiscountId { get; set; }
        public bool IsValid { get; set; }
        public string? Code { get; set; }
        public string? status { get; set; }
        public string? Description { get; set; }
        public Discount? DiscountDetails { get; set; }
    }

}
