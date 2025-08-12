namespace GoldBank.Models.RequestModels
{
    public class DiscountRM
    {
        public int? DiscountTypeId { get; set; }
    }
    public class DiscountURM
    {
        public int DiscountId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int UpdatedBy { get; set; }

    }
}
