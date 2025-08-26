namespace GoldBank.Models
{
    public class RawGold : BaseDomain
    {
        public int RawGoldId { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
    }
    public class AssetSummary
    {
        public decimal TotalGold { get; set; }
        public decimal TotalInventory { get; set; }
        public decimal RawGold { get; set; }
    }
}
