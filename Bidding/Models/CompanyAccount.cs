namespace GoldBank.Models
{
    public class CompanyAccount : BaseDomain
    {
        public int CompanyAccountId { get; set; }
        public string Description { get; set; }
        public decimal? CurrentBalance { get; set; }
        public decimal? InFlows { get; set; }
        public decimal? OutFlows { get; set; }
        public string? AccountName { get; set; }
        public string? BranchCode { get; set; }
        public string? Iban { get; set; }
        public string? Currency { get; set; }
        public int? BankAccountId { get; set; }
        public string? BankName { get; set; }

    }
}
