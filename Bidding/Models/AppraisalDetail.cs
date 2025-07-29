namespace GoldBank.Models
{
    public class AppraisalDetail : BaseDomain
    {
        public int AppraisalDetailId { get; set; }        
        public decimal TotalProductWeight { get; set; } 
        public decimal NetGoldWeight { get; set; }      
        public decimal PureGoldWeight { get; set; }     
        public decimal DeductionPercentage { get; set; }
        public decimal AppraisalPrice { get; set; }     
        public string? Notes { get; set; }               
        public int WeightTypeId { get; set; }
        public List<AppraisalStoneDetail> AppraisalStoneDetails {  get; set; } = new List<AppraisalStoneDetail> { };
        public List<AppraisalDocument> AppraisalDocuments { get; set; } = new List<AppraisalDocument> { };

    }
    public class AppraisalStoneDetail
    {
        public int AppraisalStoneDetailId { get; set; }
        public int AppraisalDetailId { get; set; }
        public int StoneTypeId { get; set; }
        public int StoneQuantity { get; set; }
        public decimal StoneWeight { get; set; }      
        public decimal StonePrice { get; set; }
        public int StoneWeightTypeId { get; set; }
    }
    public class AppraisalDocument
    {
        public int AppraisalDocumentId { get; set; }
        public int AppraisalDetailId { get; set; }
        public int DocumentId { get; set; }
        public bool IsPrimary { get; set; }
        public string? Url { get; set; }
    }
}
