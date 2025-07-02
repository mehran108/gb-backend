namespace GoldBank.Models
{
    public class AllRequest<T> : BaseDomain
    {
        public T Data {get;set; }
        public int Offset { get; set; }
        public int PageSize { get; set; }
        public string SortColumn { get; set; }
        public bool SortAscending { get; set; }
        public string SearchText { get; set; }
    }
}
