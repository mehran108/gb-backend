using System;

namespace GoldBank.Models
{
    public class OrderDelievery : BaseDomain
    {
        public int OrderDelieveryId {get; set; }
        public int OrderId {get; set; }
        public int? DelieveryMethodId {get; set; }
        public DelieveryMethod? DelieveryMethod {get; set; }
        public DateTime? EstDelieveryDate {get; set; }
        public decimal? ShippingCost {get; set; }
        public string? DelieveryAddress { get; set; }
        public string? CourierService { get; set; }
        public string? TrackingId { get; set; }
        public DateTime? ShippingDate { get; set; }
    }
}
