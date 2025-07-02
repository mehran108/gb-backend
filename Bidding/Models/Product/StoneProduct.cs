
namespace GoldBank.Models.Product
{
    public class StoneProduct : BaseDomain
    {
        public int StoneProductId { get; set; }
        public int? StoneTypeId { get; set; }
        public StoneType? StoneType { get; set; }
        public int Quantity { get; set; }
        public int? StoneWeightTypeId { get; set; }
        public StoneWeightType? StoneWeightType { get; set; }
        public decimal TotalPrice { get; set; }
        public int? StoneShapeId { get; set; }
        public StoneShape? StoneShape { get; set; }
        public ICollection<Document> Documents { get; set; } = new List<Document>();

    }
}