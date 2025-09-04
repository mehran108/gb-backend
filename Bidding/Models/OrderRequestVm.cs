namespace GoldBank.Models
{
    public class OrderRequestVm : BaseDomain
    {
        public int? OrderId { get; set; }
        public int? CustomerId { get; set; }
        public int? ProductId { get; set; }
        public int? StoreId { get; set; }
        public int? OrderTypeId { get; set; }
        public decimal? EstStartingPrice { get; set; }
        public decimal? EstMaxPrice { get; set; }
        public decimal? Rate { get; set; }
        public bool? IsRateLocked { get; set; }
        public decimal? AdvancePayment { get; set; }
        public decimal? PendingPayment { get; set; }
        public decimal? PaymentReceived { get; set; }
        public int? OrderStatusId { get; set; }
    }
    public class OrderStatusCount
    {
        public int TotalOrders { get; set; }
        public int OrderStatusId { get; set; }
    }
}
