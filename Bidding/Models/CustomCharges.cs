namespace GoldBank.Models
{
    public class CustomCharge : BaseDomain
    {
        public int CustomChargesId { get; set; }
        public int OrderId { get; set; }
        public string Label { get; set; }
        public decimal Value { get; set; }
    }
}
