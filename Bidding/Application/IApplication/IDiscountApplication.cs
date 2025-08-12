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
        Task<int> AddOrderDiscount(OrderDiscount discount);
        Task<bool> UpdateDiscountStatus(DiscountURM discount);
        Task<List<SaleSummary>> GetActiveSalesSummary(int discountTypeId, int? discountId);
        Task<List<VoucherSummary>> GetVoucherSummary();
        Task<List<LoyaltyCardSummary>> GetLoyaltyCardSummary();
        Task<DiscountCodeVerification> GetDiscountValidityByCode(OrderDiscount entity);
    }
}
