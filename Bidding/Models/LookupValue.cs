namespace GoldBank.Models
{
    public class LookupValue : BaseDomain
    {
        public int Id { get; set; }
        public int LookupTableId { get; set; }
        public string LookupValueCode { get; set; }
        public string? Description { get; set; }
        public string? Extra { get; set; }
    }
}
