
namespace GoldBank.Models.Product
{
    public class Product : BaseDomain
    {
        public int ProductId { get; set; }
        public int? ProductTypeId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int? ProductSourceId { get; set; }
        public ProductSource? ProductSource { get; set; }
        public int? VendorId { get; set; }
        public int? StoreId { get; set; }
        public Vendor? Vendor { get; set; }
        public Jewellery Jewellery { get; set; } = new Jewellery();
        public ICollection<StoneProduct> StoneProducts { get; set; } = new List<StoneProduct>();
        public ICollection<ProductDocument> ProductDocuments { get; set; } = new List<ProductDocument>();
    }
}
