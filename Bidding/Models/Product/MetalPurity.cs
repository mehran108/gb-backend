namespace GoldBank.Models.Product
{
    public class MetalPurity : BaseDomain
    {
        public int MetalPurityId { get; set; }
        public string? Description { get; set; }
        public decimal? UnitPrice { get; set; }
        public int MetalTypeId { get; set; }
        public decimal PurityPercentage { get; set; }
        public int? StoreId { get; set; }
    }
}
