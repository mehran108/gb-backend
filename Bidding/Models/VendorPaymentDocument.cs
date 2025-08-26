namespace GoldBank.Models
{
    public class VendorPaymentDocument : BaseDomain
    {
        public int VendorPaymentDocumentId { get; set; }
        public int VendorPaymentId { get; set; }
        public int DocumentId { get; set; }
        public string Url { get; set; }
        public bool IsPrimary { get; set; }
    }
}
