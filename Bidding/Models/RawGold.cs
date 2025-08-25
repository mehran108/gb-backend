namespace GoldBank.Models
{
    public class RawGold : BaseDomain
    {
        public int RawGoldId { get; set; }
        public decimal Value { get; set; }
        public string Description { get; set; }
    }
}
