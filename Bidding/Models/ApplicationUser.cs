namespace Bidding.Models
{
    public class ApplicationUser : Base
    {
        public int ApplicationUserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int CreatedById { get; set; }
        public int ModifiedById { get; set; }
        public bool Active { get; set; }

    }

}
