using GoldBank.Models.Product;

namespace GoldBank.Models
{
    public class CashManagementSummary
    {
        public List<Store> BranchDetails {  get; set; }
        public List<CompanyAccount> BankDetails {  get; set; }
        public decimal TotalBankBalance { get; set; }
        public decimal TotalAvailableCash { get; set; }
        public decimal TotalPhysicalCash { get; set; }
    }
    public class StoreCashManagementSummary
    {
       public string TransactionId { get; set; }
       public DateTime Date { get; set; }
       public string Store { get; set; }
       public string TransferType { get; set; }
       public string Source { get; set; }
       public string Destination { get; set; }
       public decimal Amount { get; set; }
       public bool IsCredit { get; set; }
       public bool IsDebit { get; set; }
       public int StoreId { get; set; }
    }
    public class StoreCashManagementRequestVm
    {
        public int? StoreId { get; set; }
        public int? SourceId { get; set; }
        public int? CompanyAccountId { get; set; }
        public decimal? TransactionAmount { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

    }
}
