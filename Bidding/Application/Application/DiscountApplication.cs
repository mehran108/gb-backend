using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.RequestModels;

namespace GoldBank.Application.Application
{
    public class DiscountApplication : IBaseApplication<Discount>, IDiscountApplication
    {
        public DiscountApplication(IDiscountInfrastructure DiscountInfrastructure, IConfiguration configuration, ILogger<Discount> logger)
        {
            this.DiscountInfrastructure = DiscountInfrastructure;
        }

        public IDiscountInfrastructure DiscountInfrastructure { get; }

        public Task<bool> Activate(Discount entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Add(Discount entity)
        {
            return await this.DiscountInfrastructure.Add(entity);
        }

        public async  Task<Discount> Get(Discount entity)
        {
            return await this.DiscountInfrastructure.Get(entity);
        }

        public async Task<AllResponse<Discount>> GetAll(AllRequest<Discount> entity)
        {
            return await this.DiscountInfrastructure.GetAll(entity);
        }

        public async Task<AllResponse<Discount>> GetAllDiscounts(AllRequest<DiscountRM> entity)
        {
            return await this.DiscountInfrastructure.GetAllDiscounts(entity);
        }

        public Task<List<Discount>> GetList(Discount entity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Update(Discount entity)
        {
            return await this.DiscountInfrastructure.Update(entity);
        }
        public async Task<int> AddVoucherType(VoucherType voucherType)
        {
            return await this.DiscountInfrastructure.AddVoucherType(voucherType);
        }
        public async Task<bool> UpdateVoucherType(VoucherType voucherType)
        {
            return await this.DiscountInfrastructure.UpdateVoucherType(voucherType);
        }
        public async Task<VoucherType> GetVoucherType(VoucherType voucherType)
        {
            return await this.DiscountInfrastructure.GetVoucherType(voucherType);
        }
        public async Task<List<VoucherType>> GetAllVoucherType(VoucherType voucherType)
        {
            return await this.DiscountInfrastructure.GetAllVoucherType(voucherType);
        }
        public async Task<int> AddLoyaltyCardType(LoyaltyCardType LoyaltyCardType)
        {
            return await this.DiscountInfrastructure.AddLoyaltyCardType(LoyaltyCardType);
        }
        public async Task<bool> UpdateLoyaltyCardType(LoyaltyCardType LoyaltyCardType)
        {
            return await this.DiscountInfrastructure.UpdateLoyaltyCardType(LoyaltyCardType);
        }
        public async Task<LoyaltyCardType> GetLoyaltyCardType(LoyaltyCardType LoyaltyCardType)
        {
            return await this.DiscountInfrastructure.GetLoyaltyCardType(LoyaltyCardType);
        }
        public async Task<List<LoyaltyCardType>> GetAllLoyaltyCardType(LoyaltyCardType LoyaltyCardType)
        {
            return await this.DiscountInfrastructure.GetAllLoyaltyCardType(LoyaltyCardType);
        }
        public async Task<bool> UpdateDiscountStatus(Discount discount)
        {
            return await this.DiscountInfrastructure.UpdateDiscountStatus(discount);
        }
        public async Task<SaleSummary> GetSaleSummaryById(int discountId)
        {
            return await this.DiscountInfrastructure.GetSaleSummaryById(discountId);
        }
        public async Task<VoucherSummary> GetVoucherSummaryByTypeId(int voucherTypeId)
        {
            return await this.DiscountInfrastructure.GetVoucherSummaryByTypeId(voucherTypeId);
        }
        public async Task<LoyaltyCardSummary> GetLoyalCardSummaryByTypeId(int loyaltyCardTypeId)
        {
            return await this.DiscountInfrastructure.GetLoyalCardSummaryByTypeId(loyaltyCardTypeId);
        }
        public async Task<SaleSummary> GetBulkDiscountSummaryById(int discountId)
        {
            return await this.DiscountInfrastructure.GetBulkDiscountSummaryById(discountId);
        }
        public async Task<SaleSummary> GetPromoSummaryById(int discountId)
        {
            return await this.DiscountInfrastructure.GetPromoSummaryById(discountId);
        }
    }
}
