using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class PaymentInfrastructure : BaseInfrastructure, IPaymentInfrastructure
    {
        public PaymentInfrastructure(IConfiguration configuration) : base(configuration)
        {

        }

        public Task<bool> Activate(Payment entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> Add(Payment entity)
        {
            throw new NotImplementedException();
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
    }
}
