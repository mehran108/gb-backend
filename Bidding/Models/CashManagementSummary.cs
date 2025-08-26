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
}
