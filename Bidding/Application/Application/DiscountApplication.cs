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
    }
}
