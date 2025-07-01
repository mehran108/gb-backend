
namespace GoldBank.Models.Product
{
    public class StoneProductGb : BaseDomain
    {
        public int StoneProductId { get; set; }
        public int? StoneTypeId { get; set; }
        public int Quantity { get; set; }
        public int? StoneWeightTypeId { get; set; }
        public decimal TotalPrice { get; set; }
        public int? StoneShapeId { get; set; }
        public ICollection<DocumentGb> Documents { get; set; } = new List<DocumentGb>();

    }
}