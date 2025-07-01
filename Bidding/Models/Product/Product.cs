
namespace GoldBank.Models.Product
{
    public class Product : BaseDomain
    {
        public int ProductId { get; set; }
        public int? ProductTypeId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int? ProductSourceId { get; set; }
        public int? VendorId { get; set; }
        public Jewellery Jewellery { get; set; } = new Jewellery();
        public ICollection<StoneProduct> StoneProducts { get; set; } = new List<StoneProduct>();
        public ICollection<Document> Documents { get; set; } = new List<Document>();
    }
}
