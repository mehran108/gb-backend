namespace GoldBank.Models
{
    public class Invoice
    {
        public List<InvoiceOrder> Orders { get; set; }
        public int PaymentId { get; set; }
        public string Url { get; set; }
        public string CreatedBy { get; set; } // SALESPERSON
        public string CustomerName { get; set; }
        public string Phone { get; set; }
        public string? AmountReceivable { get; set; }
        public TransactionSummary? TransactionSummary { get; set; }
    }

    public class InvoiceOrder
    {
        public string OrderType { get; set; }
        public int OrderTypeId { get; set; }
        public ItemDetails? ItemDetails { get; set; }
        public MetalDetails MetalDetails { get; set; }
        public ItemPricing? ItemPricing { get; set; }
        public ReservationDetails? ReservationDetails { get; set; }
        public BookingDetails? BookingDetails { get; set; }
        public RepairInvoiceDetails? RepairDetails { get; set; }
        public AlterationInvoiceDetails? AlterationDetails { get; set; }
        public AppraisalInvoiceDetails? AppraisalDetails { get; set; }
        public List<StoneDetails>? DiamondDetails { get; set; }
    }

    public class ItemDetails
    {
        public string? ItemName { get; set; }
        public string? SKU { get; set; }
        public int? Quantity { get; set; }
        public string? Image { get; set; }        
        public string? ItemCategory { get; set; }
        public string? ImageBeforeRepair { get; set; }
        public string? ImageAfterRepair { get; set; }
    }

    public class MetalDetails
    {
        public string? MetalType { get; set; }
        public string? MetalPurity { get; set; }        
        public string? Weight { get; set; }
        public string? RatePerGram { get; set; }
        public string? MetailValue { get; set; }
        public string? GrossWeight { get; set; }
        public string? NetWeight { get; set; }
        public string? WeightVariance { get; set; }
        public string? MetalRateLockStatus { get; set; }
        public string? LockedRatePerGram { get; set; }
        public string? CurrentRatePerGram { get; set; }
        public string? DeductionPercentage { get; set; }
        public string? DeductionValue { get; set; }
        public string? Gold24KWeight { get; set; }
    }

    public class StoneDetails
    {
        public string? StoneName { get; set; }
        public string? Quantity { get; set; }
        public string? Carat { get; set; }
        public string? Price { get; set; }
    }

    public class ItemPricing
    {
        public string? MetalValue { get; set; }
        public string? MakingCharges { get; set; }
        public string? LacquerCharges { get; set; }
        public string? GemStoneValue { get; set; }
        public string? SubtotalMetalPrice { get; set; }
        public string? SubtotalStonePrice { get; set; }
        public string? SubTotalPrice { get; set; }        
        public string? DiscountType { get; set; }
        public string? DiscountAmount { get; set; }
        public string? DiscountCode { get; set; }
        public string? TotalPrice { get; set; }
        public string? BaselineTotalPrice { get; set; }
        public string? AdvanceAmount { get; set; }
        public string? BalanceAmount { get; set; }
        public string? AmountPayable { get; set; }
    }

    public class ReservationDetails
    {
        public string? ReservationId { get; set; }
        public string? ReservationDate { get; set; }
        public string? CollectionDate { get; set; }
    }

    public class BookingDetails
    {
        public string? BookingId { get; set; }
        public string? RateLockStatus { get; set; }
        public string? LockedRatePerGram { get; set; }
        public string? CurrentRatePerGram { get; set; }
        public string? BookingPrice { get; set; }
        public string? AdvanceAmount { get; set; }
        public string? BalanceAmount { get; set; }
        public string? BalanceAmountEstimated { get; set; }
    }

    public class RepairInvoiceDetails
    {
        public string? RepairId { get; set; }
        public string? ServiceTypeOne { get; set; }
        public string? ServiceTypeTwo { get; set; }
        public string? Notes { get; set; }
        public string? SubtotalRepairPrice { get; set; }
        public string? AdvanceAmount { get; set; }
        public string? BalanceAmount { get; set; }
        public string? AmountPayable { get; set; }
    }

    public class AlterationInvoiceDetails
    {
        public string? AlterationId { get; set; }
        public string? ServiceTypeOne { get; set; }
        public string? ServiceTypeTwo { get; set; }
        public string? Notes { get; set; }
        public string? SubtotalAlterationPrice { get; set; }
        public string? AdvanceAmount { get; set; }
        public string? BalanceAmount { get; set; }
        public string? AmountPayable { get; set; }
    }

    public class AppraisalInvoiceDetails
    {
        public string? AppraisalId { get; set; }
        public string? AppraisedPrice { get; set; }
        public string? ItemValuation { get; set; }
    }

    public class TransactionSummary
    {
        public string? GrandTotalPayable { get; set; }
        public string? CashPaid { get; set; }
        public OnlineTransfer? OnlineTransfer { get; set; }
        public CardMachine? CardMachine { get; set; }
    }

    public class OnlineTransfer
    {
        public string? CompanyBank { get; set; }
        public string? CustomerBank { get; set; }
        public string? CustomerAccount { get; set; }
        public string? TransactionId { get; set; }
        public string? Amount { get; set; }
    }

    public class CardMachine
    {
        public string? CompanyBank { get; set; }
        public string? Last4Digits { get; set; }
        public string? TransactionId { get; set; }
        public string? Amount { get; set; }
    }
    public class OrderInvoiceTemplate
    {
        public int? OrderTypeId { get; set; }
        public string Template { get; set; }
        public string? Code { get; set; }
    }
}
