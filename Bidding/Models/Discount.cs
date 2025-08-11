

namespace GoldBank.Models
{
    public class Discount : BaseDomain
    {
        public int DiscountId { get; set; }
        public int DiscountTypeId { get; set; }
        public string Name { get; set; }
        public string CardDisplayName { get; set; }
        public string Code { get; set; }
        public decimal MinInvoiceAmount { get; set; }
        public int? MaxUsage { get; set; }
        public string PersonName { get; set; }
        public string Description { get; set; }
        public decimal? SalesComissionPct { get; set; }
        public int? MaxUser { get; set; }
        public int? CustomerId { get; set; }

        public int? ExpiryDuration { get; set; }
        public int? ExpiryDurationType { get; set; }
        public int? LoyaltyCardTypeId { get; set; }
        public LoyaltyCardType? LoyaltyCardType { get; set; }
        public int? VoucherTypeId { get; set; }
        public VoucherType? VoucherType { get; set; }

        public string PrimaryCategories { get; set; }
        public string CategoryIds { get; set; }
        public string SubCategoryIds { get; set; }
        public string CollectionTypeIds { get; set; }
        public string LabelTypeIds { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPct { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsEcommerce { get; set; }
        public bool IsInStore { get; set; }
        public string? StoreIds { get; set; }

    }
}
