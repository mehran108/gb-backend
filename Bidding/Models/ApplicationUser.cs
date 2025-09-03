namespace GoldBank.Models
{
    public class ApplicationUser : BaseDomain
    {
        public int ApplicationUserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Phone { get; set; }
        public int UserRoleId { get;set;}

    }

}
