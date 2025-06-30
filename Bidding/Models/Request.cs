namespace GoldBank.Models
{
    public class Request<T>
    {
        public int Limit { get; set; }
        public int PageNo { get; set; }
        public int TotalCount { get; set; }
        public int Active { get; set; }
        public int Order { get; set; }
        public string Column { get; set; }
        public string Target { get; set; }
        public List<T> ItemList { get; set; }
        public string PName { get; set; }
    }
}