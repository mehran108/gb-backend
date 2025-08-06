namespace GoldBank.Models
{
    public class GiftCardDetail : BaseDomain
    {
        public int GiftCardDetailId { get; set; }
        public string RecipientName { get; set; }
        public string RecipientMobileNumber { get; set; }
        public string RecipientCnic { get; set; }
        public decimal Amount { get; set; }
        public string? DepositorName { get; set; }
        public string? DepositorMobileNumber { get; set; }
        public string? Code { get; set; }
        public string Sku { get; set; }
        public DateTime? RedeemDate { get; set; }
    }
}
