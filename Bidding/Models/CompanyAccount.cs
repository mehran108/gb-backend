namespace GoldBank.Models
{
    public class CompanyAccount : BaseDomain
    {
        public int CompanyAccountId { get; set; }
        public string Description { get; set; }
        public decimal? CurrentBalance { get; set; }
        public decimal? InFlows { get; set; }
        public decimal? OutFlows { get; set; }
    }
}
