using System;

namespace GoldBank.Models
{
    public class OrderDelievery : BaseDomain
    {
        public int OrderDelieveryId {get; set; }
        public int OrderId {get; set; }
        public int DelieveryMethodId {get; set; }
        public DelieveryMethod? DelieveryMethod {get; set; }
        public int EstDelieveryDate {get; set; }
        public decimal? ShippingCost {get; set; }
        public string? DelieveryAddress { get; set; }
    }
}
