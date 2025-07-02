namespace GoldBank.Models.Product
{
    public class SubCategory : BaseDomain
    {
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public string? Description { get; set; }
    }
}
