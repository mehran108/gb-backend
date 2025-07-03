using GoldBank.Models.Product;

namespace GoldBank.Models.Product
{
    public class ProductBulkImport : BaseDomain
    {
        public string SKU { get; set; } = string.Empty;
        public string? ProductSourceDescription { get; set; }      
        public string? VendorDescription { get; set; }

        public string? PrimaryCategories { get; set; } 
        public string? CategoryDescription { get; set; }
        public string? ProductTypeDescription { get; set; }
        public string? WearingTypes { get; set; } 
        public string? Collections { get; set; }
        public string? GenderTypeDescription { get; set; }
        public string? Occasions { get; set; }
        public string? JewelleryDescription { get; set; }
        public string? MetalTypeDescription { get; set; }
        public string? MetalPurityTypeDescription { get; set; }       
        public string? MetalColorTypeDescription { get; set; }
        public string? WeightTypeDescription { get; set; }
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
        public IFormFile File { get; set; }
    }
    public class ProductStonesBulkImport : BaseDomain
    {
        public string SKU { get; set; }
        public string? StoneTypeDescription { get; set; }
        public int Quantity { get; set; }
        public string? StoneWeightTypeDescription { get; set; }
        public decimal StoneTotalPrice { get; set; }
        public decimal StoneTotalWeight { get; set; }
        public string? StoneShapeDescription { get; set; }        
        public IFormFile File { get; set; }
    }
}

