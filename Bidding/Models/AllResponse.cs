namespace GoldBank.Models
{
    public class AllResponse<T> : BaseDomain
    {
        public List<T> Data { get; set; }
        public int Offset { get; set; }
        public int PageSize { get; set; }
        public int TotalRecord { get; set; }
        public string SortColumn { get; set; }
        public bool SortAscending { get;set; }
    }
}
