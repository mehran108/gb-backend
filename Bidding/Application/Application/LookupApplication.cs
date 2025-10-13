using Amazon.Runtime.Documents;
using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;
using GoldBank.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;
using static Amazon.S3.Util.S3EventNotification;

namespace GoldBank.Application.Application
{
    public class LookupApplication : IBaseApplication<LookupValue>, ILookupApplication
    {
        public LookupApplication(ILookupInfrastructure LookupInfrastructure, IConfiguration configuration, ILogger<LookupApplication> logger)
        {
            this.LookupInfrastructure = LookupInfrastructure;
        }

        public ILookupInfrastructure LookupInfrastructure;
        public async Task<int> Add(LookupValue entity)
        {
            return await this.LookupInfrastructure.Add(entity);
        }
        public async Task<List<LookupValue>> GetLookupByCode(LookupValue entity)
        {
            return await this.LookupInfrastructure.GetLookupByCode(entity);
        }
        public async Task<IEnumerable<ProductType>> GetAllProductTypeGbAsync()
        {
            return await this.LookupInfrastructure.GetAllProductTypeGbAsync();
        }
        public async Task<IEnumerable<GenderType>> GetAllGenderTypeGbAsync()
        {
            return await this.LookupInfrastructure.GetAllGenderTypeGbAsync();
        }
        public async Task<IEnumerable<ProductSource>> GetAllProductSourceGbAsync()
        {
            return await this.LookupInfrastructure.GetAllProductSourceGbAsync();
        }
        public async Task<IEnumerable<Vendor>> GetAllVendorGbAsync()
        {
            return await this.LookupInfrastructure.GetAllVendorGbAsync();
        }
        public async Task<IEnumerable<MetalType>> GetAllMetalTypeGbAsync()
        {
            return await this.LookupInfrastructure.GetAllMetalTypeGbAsync();
        }
        public async Task<IEnumerable<MetalPurity>> GetAllMetalPurityGbAsync()
        {
            return await this.LookupInfrastructure.GetAllMetalPurityGbAsync();
        }
        public async Task<IEnumerable<MetalColor>> GetAllMetalColorGbAsync()
        {
            return await this.LookupInfrastructure.GetAllMetalColorGbAsync();
        }
        public async Task<IEnumerable<WeightType>> GetAllWeightTypeGbAsync()
        {
            return await this.LookupInfrastructure.GetAllWeightTypeGbAsync();
        }
        public async Task<IEnumerable<StoneType>> GetAllStoneTypeGbAsync()
        {
            return await this.LookupInfrastructure.GetAllStoneTypeGbAsync();
        }
        public async Task<IEnumerable<StoneWeightType>> GetAllStoneWeightTypeGbAsync()
        {
            return await this.LookupInfrastructure.GetAllStoneWeightTypeGbAsync();
        }
        public async Task<IEnumerable<StoneShape>> GetAllStoneShapeGbAsync()
        {
            return await this.LookupInfrastructure.GetAllStoneShapeGbAsync();
        }
        public async Task<IEnumerable<WearingType>> GetAllWearingTypeGbAsync()
        {
            return await this.LookupInfrastructure.GetAllWearingTypeGbAsync();
        }
        public async Task<IEnumerable<Collection>> GetAllCollectionGbAsync()
        {
            return await this.LookupInfrastructure.GetAllCollectionGbAsync();
        }
        public async Task<IEnumerable<Occasion>> GetAllOccasionGbAsync()
        {
            return await this.LookupInfrastructure.GetAllOccasionGbAsync();
        }
        public async Task<IEnumerable<PrimaryCategory>> GetPrimaryCategories()
        {
            return await this.LookupInfrastructure.GetPrimaryCategories();
        }
        public async Task<IEnumerable<Category>> GetCategories()
        {
            return await this.LookupInfrastructure.GetCategories();
        }
        public async Task<IEnumerable<SubCategory>> GetSubCategories()
        {
            return await this.LookupInfrastructure.GetSubCategories();
        }

        public async Task<IEnumerable<Store>> GetAllStores()
        {
            return await this.LookupInfrastructure.GetAllStores();
        }
        public async Task<IEnumerable<OrderType>> GetAllOrderTypes()
        {
            return await this.LookupInfrastructure.GetAllOrderTypes();
        }
        public async Task<IEnumerable<DelieveryMethod>> GetAllDeliveryMethods()
        {
            return await this.LookupInfrastructure.GetAllDeliveryMethods();
        }
        public async Task<IEnumerable<OrderStatus>> GetAllOrderStatus()
        {
            return await this.LookupInfrastructure.GetAllOrderStatus();
        }
        public async Task<IEnumerable<PaymentType>> GetAllPaymentType()
        {
            return await this.LookupInfrastructure.GetAllPaymentType();
        }
        public async Task<IEnumerable<CustomerAccount>> GetAllCustomerAccounts()
        {
            return await this.LookupInfrastructure.GetAllCustomerAccounts();
        }
        public async Task<IEnumerable<CompanyAccount>> GetAllCompanyAccounts()
        {
            return await this.LookupInfrastructure.GetAllCompanyAccounts();
        }
        public async Task<IEnumerable<LacquerType>> GetAllLacquerTypes()
        {
            return await this.LookupInfrastructure.GetAllLacquerTypes();
        }

        public async Task<IEnumerable<RepairDamageArea>> GetAllRepairDamageAreas()
        {
            return await this.LookupInfrastructure.GetAllRepairDamageAreas();
        }

        public async Task<IEnumerable<RepairDamageType>> GetAllRepairDamageTypes()
        {
            return await this.LookupInfrastructure.GetAllRepairDamageTypes();
        }
        public async Task<IEnumerable<RepairCleaning>> GetAllRepairCleaning()
        {
            return await this.LookupInfrastructure.GetAllRepairCleaning();
        }
        public async Task<IEnumerable<RepairPolishing>> GetAllRepairPolishing()
        {
            return await this.LookupInfrastructure.GetAllRepairPolishing();
        }
        public async Task<IEnumerable<DiscountType>> GetAllDiscountType()
        {
            return await this.LookupInfrastructure.GetAllDiscountType();
        }
        public async Task<IEnumerable<ExpiryDurationType>> GetAllExpiryDuration()
        {
            return await this.LookupInfrastructure.GetAllExpiryDuration();
        }
        public async Task<IEnumerable<VendorPaymentType>> GetAllVendorPaymentTypes()
        {
            return await this.LookupInfrastructure.GetAllVendorPaymentTypes();
        }
        public async Task<IEnumerable<CustomerCategory>> GetAllCustomerCategories()
        {
            return await this.LookupInfrastructure.GetAllCustomerCategories();
        }
        public async Task<IEnumerable<VendorGoldPaymentType>> GetAllVendorGoldPaymentTypes()
        {
            return await this.LookupInfrastructure.GetAllVendorGoldPaymentTypes();
        }
        public async Task<IEnumerable<Label>> GetAllLabels()
        {
            return await this.LookupInfrastructure.GetAllLabels();
        }
        public Task<bool> Activate(LookupValue entity)
        {
            throw new NotImplementedException();
        }

        Task<LookupValue> IBaseApplication<LookupValue>.Get(LookupValue entity)
        {
            throw new NotImplementedException();
        }

        Task<List<LookupValue>> IBaseApplication<LookupValue>.GetList(LookupValue entity)
        {
            throw new NotImplementedException();
        }

        Task<bool> IBaseApplication<LookupValue>.Update(LookupValue entity)
        {
            throw new NotImplementedException();
        }
        public Task<AllResponse<LookupValue>> GetAll(AllRequest<LookupValue> entity)
        {
            throw new NotImplementedException();
        }
    }
}
