using Amazon.Runtime.Documents;
using GoldBank.Models;
using GoldBank.Models.Product;

namespace GoldBank.Application.IApplication
{
    public interface ILookupApplication : IBaseApplication<LookupValue>
    {
        Task<List<LookupValue>> GetLookupByCode(LookupValue entity);
        Task<IEnumerable<ProductType>> GetAllProductTypeGbAsync();
        Task<IEnumerable<GenderType>> GetAllGenderTypeGbAsync();
        Task<IEnumerable<ProductSource>> GetAllProductSourceGbAsync();
        Task<IEnumerable<Vendor>> GetAllVendorGbAsync();
        Task<IEnumerable<MetalType>> GetAllMetalTypeGbAsync();
        Task<IEnumerable<MetalPurity>> GetAllMetalPurityGbAsync();
        Task<IEnumerable<MetalColor>> GetAllMetalColorGbAsync();
        Task<IEnumerable<WeightType>> GetAllWeightTypeGbAsync();
        Task<IEnumerable<StoneType>> GetAllStoneTypeGbAsync();
        Task<IEnumerable<StoneWeightType>> GetAllStoneWeightTypeGbAsync();
        Task<IEnumerable<StoneShape>> GetAllStoneShapeGbAsync();
        Task<IEnumerable<WearingType>> GetAllWearingTypeGbAsync();
        Task<IEnumerable<Collection>> GetAllCollectionGbAsync();
        Task<IEnumerable<Occasion>> GetAllOccasionGbAsync();
    }
}
