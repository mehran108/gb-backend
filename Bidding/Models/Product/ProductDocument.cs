namespace GoldBank.Models.Product
{
    public class ProductDocument : BaseDomain
    {
        public int ProductDocumentId { get; set; }
        public int ProductId { get; set; }
        public int DocumentId { get; set; }
        public string Url { get; set; }
        public bool IsPrimary { get; set; }
        public int StoneId { get; set; }

    }
    public class StoneDocument
    {
        public int StoneDocumentId { get; set; }
        public int StoneId { get; set; }
        public int ProductId { get; set; }
        public string Url { get; set; }

        public int DocumentId { get; set; }
        public bool IsPrimary { get; set; }
    }
}
