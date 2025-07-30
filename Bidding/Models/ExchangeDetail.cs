using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace GoldBank.Models
{
    public class ExchangeDetail : BaseDomain
    {
        public int ExchangeDetailId { get; set; }
        public decimal? DeductionPercentage { get; set; }
        public decimal? DeductionValue { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? ExchangePrice { get; set; }
        public string? Notes { get; set; }
        public List<ExchangeDocument> ExchangeDocuments { get; set; } = new List<ExchangeDocument>();
    }

    public class ExchangeDocument 
    {
        public int ExchangeDocumentId { get; set; }
        public int? DocumentId { get; set; }
        public int? ExchangeDetailId { get; set; }
        public string? Url { get; set; }
        public bool? IsPrimary { get; set; }
    }
}
