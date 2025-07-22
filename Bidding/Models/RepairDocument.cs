namespace GoldBank.Models
{
   public class RepairDocument : BaseDomain
    {
        public int RepairDocumentId { get; set; }
        public int RepairDetailId { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsPostRepair { get; set; }
    }
}
