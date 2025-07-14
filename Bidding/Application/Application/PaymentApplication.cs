using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.RequestModels;

namespace GoldBank.Application.Application
{
    public class PaymentApplication : IBaseApplication<Payment>, IPaymentApplication
    {
        public PaymentApplication(IPaymentInfrastructure PaymentInfrastructure, IConfiguration configuration, ILogger<Payment> logger)
        {
            this.PaymentInfrastructure = PaymentInfrastructure;
        }

        public IPaymentInfrastructure PaymentInfrastructure { get; }

        public Task<bool> Activate(Payment entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Add(Payment entity)
        {
            return await this.PaymentInfrastructure.Add(entity);
        }

        public Task<Payment> Get(Payment entity)
        {
            throw new NotImplementedException();
        }

        public Task<AllResponse<Payment>> GetAll(AllRequest<Payment> entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<Payment>> GetList(Payment entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Payment entity)
        {
            throw new NotImplementedException();
        }
        public async Task<int> AddPayment(PaymentRM paymentRM)
        {
            return await this.PaymentInfrastructure.AddPayment(paymentRM);
        }
    }
}
