
namespace GoldBank.Models.Product
{
    public class ProductGb : BaseDomain
    {
        public int ProductId { get; set; }
        public int? ProductTypeId { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int? ProductSourceId { get; set; }
        public int? VendorId { get; set; }
        public JewelleryGb Jewellery { get; set; } = new JewelleryGb();
        public ICollection<StoneProductGb> StoneProducts { get; set; } = new List<StoneProductGb>();
        public ICollection<DocumentGb> Documents { get; set; } = new List<DocumentGb>();
    }
}
