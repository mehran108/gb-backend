namespace GoldBank.Models
{
    public class Order : BaseDomain
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int ProductId { get; set; }
        public Product.Product? Product { get; set; } = new Product.Product();
        public int StoreId { get; set; }
        public int OrderTypeId { get; set; }
        public OrderType? OrderType { get; set; }
        public decimal EstStartingPrice { get; set; } = 0;
        public decimal EstMaxPrice { get; set; } = 0;
        public decimal Rate { get; set; } = 0;
        public decimal TotalPayment { get; set; } = 0;
        public bool IsRateLocked { get; set; } = false;
        public decimal AdvancePayment { get; set; } = 0;
        public decimal PendingPayment { get; set; } = 0;
        public decimal PaymentReceived { get; set; } = 0;
        public int OrderStatusId { get; set; }
        public decimal? AdvancePaymentPct { get; set; } = 0;
        public int? AlterationDetailsId { get; set; }
        public int? RepairDetailsId { get; set; }
        public int? AppraisalDetailsId { get; set; }
        public int? ExchangeDetailsId { get; set; }
        public int? GoldBookingDetailsId { get; set; }
        public int? GiftCardDetailsId { get; set; }
        public bool? IsEcommerceOrder { get; set; }
        public bool? IsOnlinePosOrder { get; set; }
        public string? Comments { get; set; }
        public string? SizeType { get; set; }
        public string? TrackingId { get; set; }

        public OrderStatus? OrderStatus { get; set; }
        public List<CustomCharge>? CustomCharge { get; set; }
        public OrderDelievery? OrderDelievery { get; set; }
        public AlterationDetails? AlterationDetails { get; set; }
        public RepairDetails? RepairDetails { get; set; }
        public AppraisalDetail? AppraisalDetails { get; set; }
        public ExchangeDetail? ExchangeDetails { get; set; }
        public GoldBookingDetail? GoldBookingDetails { get; set; }
        public GiftCardDetail? GiftCardDetails { get; set; }
    }

    public class OrderStatusReqVm : BaseDomain
    { 
        public int OrderId { get; set; }
        public int OrderStatusId { get; set; }
        public DateTime? ReservationDate { get; set; }
        public string? Comments { get; set; }
    }
}
