namespace GoldBank.Models
{

    public class Customer : BaseDomain
    {
        public int CustomerId { get; set; }
        public int? ReferenceCustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string Mobile { get; set; }
        public string PostalAddress { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public bool? IsPOS { get; set; }
        public string? Title { get; set; }
        public DateTime? BirthAnniversary { get; set; }
        public DateTime? WeddingAnniversary { get; set; }
        public bool? IsNewsSubscribe { get; set; }
        public string? PasswordHash { get; set; }
        public string? OldPwd {  get; set; }
        public string? NewPwd { get; set; }
        public string? RingSize { get; set; }
        public int? StateId { get; set; }
        public string? ZipCode { get; set; }
        public string? Gender { get; set; }
        public int? CustomerCategoryId { get; set; }
        public string? CustomerCategoryDescription { get; set; }
        public string? BangleSize { get; set; }
        public decimal? TotalSpent { get; set; }
        public DateTime? LastPurchase { get; set; }
        public string? Tag { get; set; }
        public decimal? TotalWeightPurchased { get; set; }
        public int? TotalPurchase { get; set; }
        public decimal? Rating { get; set; }
        public List<Discount>? Discounts { get; set; }
        public List<SalesPersonFeedback>? SalesPersonFeedbacks { get; set; }
    }
    public class CustomerSummary
    {
        public int InvoiceNumber { get; set; }
        public DateTime PurchaseDate {  get; set; }
        public string Branch {  get; set; }
        public decimal Amount { get; set; }
        public string OrderType { get; set; }
        public int OrderId { get; set; }
    }

}
