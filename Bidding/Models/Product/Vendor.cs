namespace GoldBank.Models.Product
{
    public class Vendor : BaseDomain
    {
        public int VendorId { get; set; }
        public string? Description { get; set; }
        public string? SerialNumber { get; set; }
        public string?  Contact { get; set; }
        public List<KaatCategory>? KaatCategory { get; set; }
    }
}
