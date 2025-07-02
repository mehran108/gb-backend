namespace GoldBank.Models
{
    public class AllResponse<T> : BaseDomain
    {
        public int Offset { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
    }
}
