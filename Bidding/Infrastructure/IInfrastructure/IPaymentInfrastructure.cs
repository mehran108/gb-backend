using GoldBank.Models;
using GoldBank.Models.RequestModels;

namespace GoldBank.Infrastructure.IInfrastructure
{
    public interface IPaymentInfrastructure : IBaseInfrastructure<Payment>
    {
        Task<int> AddPayment(AddPaymentRequest paymentRM);
        Task<int> AddOnlinePayment(AddOnlinePaymentRequest paymentRM);
        Task<bool> VerifyOnlinePayment(VerifyOnlinePaymentRequest verifyOnlinePaymentRequest);
        Task<bool> ConfirmPayment(ConfirmPaymentRequest confirmPaymentRequest);
        Task<bool?> CheckOnlinePaymentStatus(int onlinePaymentId);
        void CancelOnlinePayment(int onlinePaymentId);
        void CancelPayment(int paymentId);
        Task<AllResponse<OnlinePaymentVerificationVM>> GetAllOnlinePayments(AllRequest<OnlinePaymentVerificationRM> Payment);
    }
}
