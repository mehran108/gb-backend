using GoldBank.Models;
using GoldBank.Models.RequestModels;

namespace GoldBank.Infrastructure.IInfrastructure
{
    public interface IDiscountInfrastructure : IBaseInfrastructure<Discount>
    {
        Task<AllResponse<Discount>> GetAllDiscounts(AllRequest<DiscountRM> entity);
    }
}
