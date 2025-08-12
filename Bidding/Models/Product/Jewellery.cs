namespace GoldBank.Models.Product
{
    public class Jewellery : BaseDomain
    {
        public int JewelleryId { get; set; }
        public string? PrimaryCategoryIds { get; set; }
        public ICollection<PrimaryCategory> PrimaryCategoryList { get; set; } = new List<PrimaryCategory>();
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? SubCategoryId { get; set; }
        public ProductType? ProductType { get; set; }
        public SubCategory? SubCategory { get; set; }
        public string? WearingTypeIds { get; set; }
        public ICollection<WearingType> WearingTypeList { get; set; } = new List<WearingType>();
        public string? CollectionIds { get; set; }
        public ICollection<Collection> CollectionList { get; set; } = new List<Collection>();
        public int? GenderId { get; set; }
        public GenderType? GenderType { get; set; }
        public string? OccasionIds { get; set; }
        public ICollection<Occasion> OccasionList { get; set; } = new List<Occasion>();
        public string? Description { get; set; }
        public int? MetalTypeId { get; set; }
        public MetalType? MetalType { get; set; }
        public int? MetalPurityTypeId { get; set; }
        public MetalPurity? MetalPurity { get; set; }
        public int? MetalColorTypeId { get; set; }
        public MetalColor? MetalColor { get; set; }
        public int? WeightTypeId { get; set; }
        public WeightType? WeightType { get; set; }
        public decimal NetWeight { get; set; }
        public decimal WastageWeight { get; set; }
        public decimal WastagePct { get; set; }
        public decimal TotalWeight { get; set; }
        public string? Width { get; set; }
        public string? Bandwidth { get; set; }
        public string? Thickness { get; set; }
        public string? Size { get; set; }
        public bool IsEcommerce { get; set; }
        public bool IsEngravingAvailable { get; set; }
        public bool IsSizeAlterationAvailable { get; set; }
        public decimal LacquerPrice { get; set; }
        public decimal MakingPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int ProductId { get; set; }
        public decimal MinWeight { get; set; } = 0;
        public decimal MaxWeight { get; set; } = 0;
        public string? SerialNumber { get; set; }
    }
}
