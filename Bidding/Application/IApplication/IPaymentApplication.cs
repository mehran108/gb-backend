using GoldBank.Models;
using GoldBank.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Application.IApplication
{
    public interface IPaymentApplication : IBaseApplication<Payment>
    {
        Task<int> AddPayment(AddPaymentRequest paymentRM);
        Task<int> AddOnlinePayment(AddOnlinePaymentRequest paymentRM);
        Task<bool> VerifyOnlinePayment(VerifyOnlinePaymentRequest verifyOnlinePaymentRequest);
        Task<bool> ConfirmPayment(ConfirmPaymentRequest verifyOnlinePaymentRequest);
        Task<bool?> CheckOnlinePaymentStatus(int onlinePaymentId);
        void CancelPayment(int paymentId);
    }
}
