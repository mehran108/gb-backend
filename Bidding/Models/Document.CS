namespace GoldBank.Models
{
    public class Document : BaseDomain
    {
        public int DocumentId { get; set; }
        public string? Url { get; set; }
        public string? Name { get; set; }
        public string? Extension { get; set; }
        public string? Image64String { get; set; }
        public int? DocumentTypeId { get; set; }
        public IFormFile File { get; set; }
    }
    public class DeleteDocumentRequest
    {
        public string? DocumentIds { get; set; }
        public int? UpdatedBy { get; set; }
        public int? AlterationDetailId { get; set; }
        public int? ProductId { get; set; }
        public int? CashManagementDetailId { get; set; }
        public int? ExchangeDetailId { get; set; }
        public int? RepairDetailId { get; set; }
        public int? AppraisalDetailId { get; set; }
        public int? StoneId { get; set; }
    }


}
