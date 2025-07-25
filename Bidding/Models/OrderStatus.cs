namespace GoldBank.Models
{
    public class OrderStatus : BaseDomain
    {
        public int OrderStatusId { get; set; }
        public string? Description { get; set; }
        public string? StatusClass { get; set; }
        public int OrderTypeId { get; set; }
        public string? PublicDescription { get; set; }

    }
}
