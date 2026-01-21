namespace GoldBank.Models
{
    public class Invoice
    {
        public List<InvoiceOrder> Orders { get; set; }
        public int PaymentId { get; set; }
        public string Url { get; set; }
        public string CreatedBy { get; set; } // SALESPERSON
        public string CustomerName { get; set; }
        public TransactionSummary? TransactionSummary { get; set; }
        public string Phone { get; set; }
    }
    public class InvoiceOrder
    {
        public string OrderType { get; set; }    
        public int OrderTypeId { get; set; }
        public ItemDetails? ItemDetails { get; set; }
        public MetalDetails MetalDetails { get; set; }
        public ItemPricing? ItemPricing { get; set; }
        public ReservationDetails? ReservationDetails { get; set; }
    }
    public class ItemDetails
    {
        public string? ItemName { get; set; }
        public string? SKU { get; set; }
        public int? Quantity { get; set; }
        public string? Image { get; set; }
    }
    public class MetalDetails
    {
        public string? MetalType { get; set; }
        public string? MetalPurity { get; set; }
        public string? Weight { get; set; }
        public string? RatePerGram { get; set; }
        public string? MetailValue { get; set; }
    }
    public class ItemPricing
    {
        public string? MetalValue { get; set; }
        public string? MakingCharges { get; set; }
        public string? LacquerCharges { get; set; }
        public string? GemStoneValue { get; set; }
        public string? SubTotalPrice { get; set; }
        public string? DiscountType { get; set; }
        public string? DiscountAmount { get; set; }
        public string? TotalPrice { get; set; }
        public string? AmountPayable { get; set; }

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
    public class ReservationDetails
    {
        public string? ReservationId { get; set; }
        public string? ReservationDate { get; set; }
        public string? CollectionDate { get; set; }
    }
    public class OrderInvoiceTemplate
    {
        public int? OrderTypeId { get; set; }
        public string Template { get; set; }
        public string? Code { get; set; }
    }
}
