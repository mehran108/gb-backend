namespace GoldBank.Models
{
    public class EmailT
    {
        public int EmailTId { get; set; }
        public string Code { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string To { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public int CreatedById { get; set; }
        public int ModifiedById { get; set; }
        public bool Active { get; set; }

    }
}