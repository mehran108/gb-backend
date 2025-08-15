namespace GoldBank.Models
{
    public class DiscountSummary
    {
        public int? DiscountTypeId { get; set; }
        public int? DiscountId { get; set; }
        public string? StoreIds { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
