namespace GoldBank.Models
{
    public class Document : BaseDomain
    {
        public int DocumentId { get; set; }
        public string? Url { get; set; }
        public string? Name { get; set; }
        public string? Extension { get; set; }
        public int? DocumentTypeId { get; set; }
    }
}
