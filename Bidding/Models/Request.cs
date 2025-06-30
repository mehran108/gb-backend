namespace Bidding.Models
{
    public class Request<T>
    {
        public int limit { get; set; }
        public int PageNo { get; set; }
        public int TotalCount { get; set; }
        public int IsActive { get; set; }
        public int Order { get; set; }
        public string Colomn { get; set; }
        public string Target { get; set; }
        public List<T> itemList { get; set; }
        public string PName { get; set; }
        }
    }

//FirstName LIKE '%' + ISNULL(@PTarget,'') +'%'  or LastName  LIKE '%' + ISNULL(@PTarget,'') +'%' or Email LIKE '%' + ISNULL(@PTarget,'') +'%'