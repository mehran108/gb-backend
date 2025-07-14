using GoldBank.Models;
using GoldBank.Models.RequestModels;

namespace GoldBank.Infrastructure.IInfrastructure
{
    public interface IPaymentInfrastructure : IBaseInfrastructure<Payment>
    {
        Task<int> AddPayment(AddPaymentRequest paymentRM);
        Task<int> AddOnlinePayment(OnlinePaymentRM paymentRM);
        Task<int> AddCardPayment(CardPaymentRM paymentRM);
    }
}
