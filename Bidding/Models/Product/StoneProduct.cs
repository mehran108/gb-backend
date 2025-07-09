
namespace GoldBank.Models.Product
{
    public class StoneProduct : BaseDomain
    {
        public int StoneProductId { get; set; }
        public int? StoneTypeId { get; set; }
        public int? ProductId { get; set; }
        public StoneType? StoneType { get; set; }
        public int Quantity { get; set; }
        public int? StoneWeightTypeId { get; set; }
        public StoneWeightType? StoneWeightType { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalWeight { get; set; }
        public int? StoneShapeId { get; set; }
        public ICollection<StoneDocument> StoneDocuments { get; set; } = new List<StoneDocument>();
        public StoneShape? StoneShape { get; set; }
        public decimal MinStoneWeight {get; set; }
        public decimal MaxStoneWeight {get; set; }
        public decimal MinStonePrice  {get; set; }
        public decimal MaxStonePrice { get; set; }

    }
}