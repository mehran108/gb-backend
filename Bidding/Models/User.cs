namespace GoldBank.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public int UserId { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string? Name { get; set; }
        public string LastName { get; set; }
        public string IdentificationNumber { get; set; }
        public string Address1 { get; set; }
        public string PostalCode { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
        public string OldPwd { get; set; }
        public string NewPwd { get; set; }

    }
}
