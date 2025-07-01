namespace GoldBank.Models
{
    public class CommonCode : BaseDomain
    {
        public int CommonCodeId { get; set; }
        public IFormFile? File { get; set; }
        public string? Image64String { get; set; }
        public string? FileName { get; set; }
        public int DocumentId { get; set; }
        public int? DocumentExtension { get; set; }
        public string? DocumentPath { get; set; }
        public string? DocumentType { get; set; }
    }
}
