namespace GoldBank.Models
{
    public class RepairDetails : BaseDomain
    {
        public int RepairDetailId { get; set; }
        public int ProductTypeId { get; set; }
        public int MetalTypeId { get; set; }
        public decimal WeightBeforeRepair { get; set; }
        public int RepairCleaningId { get; set; }
        public string? CleaningNotes { get; set; }
        public decimal CleaningPrice { get; set; }
        public int RepairPolishingId { get; set; }
        public string? PolishingNotes { get; set; }
        public decimal PolishingPrice { get; set; }
        public string CurrentJewellerySize { get; set; }
        public string DesiredJewellerySize { get; set; }
        public string?  ResizingNotes { get; set; }
        public decimal ResizingPrice { get; set; }
        public string RepairDamageTypeIds { get; set; }
        public string RepairDamageAreaIds { get; set; }
        public string? RepairingNotes { get; set; }
        public decimal RepairingPrice { get; set; }
        public decimal EstRepairingCost { get; set; }
        public decimal WeightChange { get; set; }
        public decimal WeightChangePrice { get; set; }
        public decimal ActualWeight { get; set; } // weight after repair
        public decimal TotalRepairCost { get; set; }
        public DateTime EstDeliveryDate { get; set; }
        public int WeightTypeId { get; set; }      
        public List<RepairDocument> RepairDocuments { get; set; } = new List<RepairDocument>();
        public List<RepairStoneDetails> RepairStoneDetails { get; set; } = new List<RepairStoneDetails>();



    }
    
}
