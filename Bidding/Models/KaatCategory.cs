namespace GoldBank.Models
{
    public class KaatCategory : BaseDomain
    {
        public int KaatCategoryId { get; set; }
        public int VendorId { get; set; }
        public string Label { get; set; }
        public decimal Value { get; set; }
    }
}
