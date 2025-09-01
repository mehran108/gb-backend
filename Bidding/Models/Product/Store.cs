namespace GoldBank.Models.Product
{
    public class Store : BaseDomain
    {
        public int? StoreId { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public decimal? AvailableCash { get; set; }
        public decimal? InFlows { get; set; }
        public decimal? OutFlows { get; set; }
    }
}
