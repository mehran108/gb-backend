namespace GoldBank.Models
{
    public class CustomerFeedback : BaseDomain
    {
        public int CustomerFeedbackId { get; set; }
        public int CustomerId { get; set; }
        public string Feedback { get; set; }
        public int? Rating { get; set; }
    }
    public class  SalesPersonFeedback : BaseDomain
    {
        public int SalesPersonFeedbackId { get; set; }
        public int UserId { get; set; }
        public int CustomerId { get; set; }
        public string Feedback { get; set; }
        public int? Rating { get; set; }
    }
}
