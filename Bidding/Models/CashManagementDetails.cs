namespace GoldBank.Models
{
    public class CashManagementDetails : BaseDomain
    {
        public int CashManagementDetailId { get; set; }
        public int? CompanyAccountId { get; set; }
        public int? StoreId { get; set; }
        public decimal Amount { get; set; }
        public bool IsWithdraw { get; set; }
        public bool IsAddCash { get; set; }
        public string? Notes { get; set; }
        public string TransactionId { get; set; }
        public List<CashManagementDetailDocument> CashManagementDetailDocuments { get; set; } = new List<CashManagementDetailDocument>();
    }
    public class CashManagementDetailDocument
    {
        public int CashManagementDetailDocumentId { get; set; }
        public int CashManagementDetailId { get; set; }
        public int DocumentId { get; set; }
        public bool IsPrimary { get; set; }
        public string Url { get; set; }
    }
}
