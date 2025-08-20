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
    public class MetalPurityVm
    {
        public string? MetalPurityIds { get; set; }
        public int? MetalTypeId { get; set; }
        public string? StoreIds { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? AggregationType { get; set; } = "DAY";

    }

}
