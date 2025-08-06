using GoldBank.Models;
using GoldBank.Models.RequestModels;

namespace GoldBank.Application.IApplication
{
    public interface IDiscountApplication : IBaseApplication<Discount>
    {
        Task<AllResponse<Discount>> GetAllDiscounts(AllRequest<DiscountRM> entity);
    }
}
