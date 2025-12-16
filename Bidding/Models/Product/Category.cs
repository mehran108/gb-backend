namespace GoldBank.Models.Product
{
    public class Category : BaseDomain
    {
        public int? CategoryId { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsDefault { get; set; }
        public bool? IsAppraisal { get; set; }
    }
}
