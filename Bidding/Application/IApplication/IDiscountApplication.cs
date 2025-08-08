using GoldBank.Models;
using GoldBank.Models.RequestModels;

namespace GoldBank.Application.IApplication
{
    public interface IDiscountApplication : IBaseApplication<Discount>
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
        Task<bool> UpdateDiscountStatus(Discount discount);
    }
}
