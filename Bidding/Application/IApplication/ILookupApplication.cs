using Amazon.Runtime.Documents;
using GoldBank.Models;
using GoldBank.Models.Product;
using Microsoft.AspNetCore.Mvc;

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
        Task<IEnumerable<PrimaryCategory>> GetPrimaryCategories();
        Task<IEnumerable<Category>> GetCategories();
        Task<IEnumerable<SubCategory>> GetSubCategories();
        Task<IEnumerable<Store>> GetAllStores();
        Task<ActionResult<IEnumerable<OrderType>>> GetAllOrderTypes();
        Task<ActionResult<IEnumerable<DelieveryMethod>>> GetAllDeliveryMethods();
    }
}
