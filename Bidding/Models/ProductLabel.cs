namespace GoldBank.Models
{
    public class ProductLabel : BaseDomain
    {
        public int LabelId { get; set; }
        public int ProductId { get; set; }     
        public string? ProductIds { get; set; }
    }
}
