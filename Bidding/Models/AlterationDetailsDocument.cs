namespace GoldBank.Models
{
    public class AlterationDetailsDocument : BaseDomain
    {
        public int AlterationDetailsDocumentId { get; set; }
        public int AlterationDetailsId { get; set; }
        public int DocumentId { get; set; }
        public bool IsLacquer { get; set; }
        public bool IsPostAlteration { get; set; }
        public bool IsPrimary { get; set; }
    }
}
