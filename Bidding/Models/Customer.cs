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
        public string? BangleSize { get; set; }
    }

}
