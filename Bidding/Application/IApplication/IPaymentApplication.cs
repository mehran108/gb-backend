using GoldBank.Models;
using GoldBank.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Application.IApplication
{
    public interface IPaymentApplication : IBaseApplication<Payment>
    {
        Task<int> AddPayment(PaymentRM paymentRM);
    }
}
