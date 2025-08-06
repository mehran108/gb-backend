namespace GoldBank.Models
{
    public class VoucherType : BaseDomain
    {
        public int VoucherTypeId { get; set; }
        public string Name { get; set; }
        public string PrimaryCategories { get; set; }
        public string CategoryIds { get; set; }
        public string SubCategoryIds { get; set; }
        public string CollectionTypeIds { get; set; }
        public string LabelTypeIds { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountPct { get; set; }
        public int? ExpiryDuration { get; set; }
        public int? ExpiryDurationType { get; set; }
        public bool IsEcommerce { get; set; }
        public bool IsInStore { get; set; }
        public decimal MinInvoiceAmount { get; set; }
        public int? MaxUsage { get; set; }
        public string Description { get; set; }
    }
}
