namespace GoldBank.Models
{
    public class GoldBookingDetail : BaseDomain
    {
        public int GoldBookingDetailId { get; set; }
        public decimal BookingWeight { get; set; }
        public decimal BookingPrice { get; set; }
        public int WeightTypeId { get; set; }
        public string? Notes { get; set; }
        public string Sku { get; set; }
        public int? VendorId { get; set; }
        public int? KaatCategoryId { get; set; }
        public decimal? VendorAmount { get; set; }
        public DateTime? ReservationDate { get; set; }
    }
}
