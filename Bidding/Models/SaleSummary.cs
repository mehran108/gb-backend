namespace GoldBank.Models
{
    public class SaleSummary
    {
        public int DiscountId { get; set; }
        public string? Name { get; set; }
        public string PrimaryCategories { get; set; }
        public string CategoryIds { get; set; }
        public string SubCategoryIds { get; set; }
        public int StockLevel { get; set; }
        public int UnitsSold { get; set; }
        public decimal Revenue { get; set; }
        public int? TotalUsage { get; set; }
        public decimal? DiscountGiven { get; set; }
        public decimal WeightSold { get; set; }
        public decimal? DiscountAmount { get; set; }
        public int? RedeemedCount { get; set; }
        public decimal? ComissionEarned { get; set; }
        public decimal? DiscountPct { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
    public class VoucherSummary
    {
        public int VoucherTypeId { get; set; }
        public string Name {  get; set; }
        public int ActiveCount { get; set; }
        public int RedeemedCount { get; set; }
        public int ExpiredCount { get; set; }
        public int DeactivatedCount { get; set; }
    }
    public class LoyaltyCardSummary
    {
        public int LoyaltyCardTypeId { get; set; }
        public string Name { get; set; }
        public int ActiveCount { get; set; }
        public int ExpiredCount { get; set; }
        public int UsedByCount { get; set; }
        public decimal WeightSold { get; set; }
        public decimal Revenue { get; set; }
    }
}
