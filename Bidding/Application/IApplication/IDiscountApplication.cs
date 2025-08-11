using GoldBank.Models;
using GoldBank.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

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
        Task<SaleSummary> GetSaleSummaryById(int discountId);
        Task<VoucherSummary> GetVoucherSummaryByTypeId(int voucherTypeId);
        Task<LoyaltyCardSummary> GetLoyalCardSummaryByTypeId(int loyaltyCardTypeId);
        Task<SaleSummary> GetBulkDiscountSummaryById(int discountId);
        Task<SaleSummary> GetPromoSummaryById(int discountId);
    }
}
