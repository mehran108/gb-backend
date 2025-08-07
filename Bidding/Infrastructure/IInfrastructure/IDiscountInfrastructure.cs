using GoldBank.Models;
using GoldBank.Models.RequestModels;

namespace GoldBank.Infrastructure.IInfrastructure
{
    public interface IDiscountInfrastructure : IBaseInfrastructure<Discount>
    {
        Task<AllResponse<Discount>> GetAllDiscounts(AllRequest<DiscountRM> entity);
        Task<int> AddVoucherType(VoucherType voucherType);
        Task<bool> UpdateVoucherType(VoucherType voucherType);
        Task<VoucherType> GetVoucherType(VoucherType voucherType);
        Task<List<VoucherType>> GetAllVoucherType(VoucherType voucherType);
        Task<int> AddLoyaltyCardType(LoyaltyCardType LoyaltyCardType);
        Task<bool> UpdateLoyaltyCardType(LoyaltyCardType LoyaltyCardType);
        Task<LoyaltyCardType> GetLoyaltyCardType(LoyaltyCardType LoyaltyCardType);
        Task<List<LoyaltyCardType>> GetAllLoyaltyCardType(LoyaltyCardType LoyaltyCardType);
    }
}
