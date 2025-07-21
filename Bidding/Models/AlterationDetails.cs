namespace GoldBank.Models
{
    public class AlterationDetails : BaseDomain
    {
        public int AlterationDetailsId { get; set; }
        public string CurrentJewellerySize { get; set; }
        public string DesiredJewellerySize { get; set; }
        public string SizeNote { get; set; }
        public decimal ResizingPrice { get; set; }
        public int LacquerTypeId { get; set; }
        public string LacquerNote { get; set; }
        public string LacquerReferenceSKU { get; set; }
        public decimal LacquerPrice { get; set; }
        public string OtherDescription { get; set; }
        public decimal OtherPrice { get; set; }
        public decimal ProductTotalPrice { get; set; }
        public decimal WeightChange { get; set; }
        public decimal WeightChangePrice { get; set; }
        public List<StoneAlteration> Stones { get; set; }
        public List<AlterationDetailsDocument> Documents{ get; set; }
    }
}
