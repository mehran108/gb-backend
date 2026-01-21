namespace GoldBank.Models.Product
{
    public class ProductRequestVm
    {       
        public int? ProductTypeId { get; set; }
        public string? SKU { get; set; }
        public string? ProductSourceId { get; set; }
        public int? VendorId { get; set; }
        public string? PrimaryCategoryIds { get; set; }
        public int? CategoryIds { get; set; }
        public int? SubCategoryIds { get; set; }
        public string? WearingTypeIds { get; set; }
        public string? CollectionIds { get; set; }
        public int? GenderId { get; set; }
        public string? OccasionIds { get; set; }
        public string? Description { get; set; }
        public int? MetalTypeId { get; set; }
        public int? MetalPurityTypeId { get; set; }
        public int? MetalColorTypeId { get; set; }
        public int? WeightTypeId { get; set; }
        public decimal? NetWeight { get; set; }
        public decimal? WastageWeight { get; set; }
        public decimal? WastagePct { get; set; }
        public decimal? TotalWeight { get; set; }
        public decimal? MinWeight { get; set; }
        public decimal? MaxWeight { get; set; }
        public string? Width { get; set; }
        public string? Bandwidth { get; set; }
        public string? Thickness { get; set; }
        public string? Size { get; set; }
        public bool? IsEcommerce { get; set; }
        public bool? IsEngravingAvailable { get; set; }
        public bool? IsSizeAlterationAvailable { get; set; }
        public decimal? LacquerPrice { get; set; }
        public decimal? MakingPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public int? StoneTypeId { get; set; }
        public int? StoneShapeId { get; set; }
        public int? StoneWeightTypeId { get; set; }
        public string? ReferenceSKU { get; set; }
        public bool? IsSold { get; set; }
        public bool? IsReserved { get; set; }
        public int? KaatCategoryId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? LabelIds { get; set; }
    }
}
