namespace GoldBank.Models
{
 
        public class Customer: BaseDomain
    {
            public int CustomerId { get; set; }
            public string UserName { get; set; }
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string PasswordHash { get; set; }
            public string Mobile { get; set; }
            public string Address { get; set; }
            public DateTime CreatedDate { get; set; }
            public DateTime ModifiedDate { get; set; }
            public int CreatedById { get; set; }
            public int ModifiedById { get; set; }
            public bool Active { get; set; }
        

    }

}
