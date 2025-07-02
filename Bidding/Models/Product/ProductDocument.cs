namespace GoldBank.Models.Product
{
    public class ProductDocument : BaseDomain
    {
        public int ProductDocumentId { get; set; }
        public int ProductId { get; set; }
        public int DocumentId { get; set; }
        public bool IsPrimary { get; set; }

    }
    public class StoneDocument
    {
        public int StoneDocumentId { get; set; }
        public int ProductId { get; set; }
        public int DocumentId { get; set; }
        public bool IsPrimary { get; set; }
    }
}
