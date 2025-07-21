namespace GoldBank.Models
{
    public class StoneAlteration : BaseDomain
    {
        public int StoneAlterationId { get; set; }
        public int AlterationDetailsId { get; set; }
        public int CurrentStoneTypeId { get; set; }
        public int DesiredStoneTypeId { get; set; }
        public string AdditionalNote { get; set; }
        public string ReferenceSKU { get; set; }
        public int WeightTypeId { get; set; }
        public decimal Weight { get; set; }
        public decimal Price { get; set; }

    }
}
