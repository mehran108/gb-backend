using Org.BouncyCastle.Utilities;

namespace GoldBank.Models
{
    public class RepairStoneDetails : BaseDomain
    {
        public int RepairStoneDetailId {  get; set; }
		public int RepairDetailId {  get; set; }
		public int CurrentStoneId {  get; set; }
		public int DesiredStoneId { get; set; }
        public string StoneTypeIds { get; set; }
		public bool IsFixed {  get; set; }
		public bool IsReplacement {  get; set; }
	    public string? Notes {  get; set; }
	    public decimal Price {  get; set; }
	    public decimal ActualWeight {  get; set; }
	    public decimal ActualPrice {  get; set; }
    }

	
}
