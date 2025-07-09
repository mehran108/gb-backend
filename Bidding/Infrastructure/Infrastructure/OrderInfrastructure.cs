using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;

namespace GoldBank.Infrastructure.Infrastructure
{
    public class OrderInfrastructure : BaseInfrastructure, IOrderInfrastructure
    {
        public OrderInfrastructure(IConfiguration configuration) : base(configuration)
        {

        }

        public Task<bool> Activate(Order entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> Add(Order entity)
        {
            throw new NotImplementedException();
        }

        public Task<Order> Get(Order entity)
        {
            throw new NotImplementedException();
        }

        public Task<AllResponse<Order>> GetAll(AllRequest<Order> entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<Order>> GetList(Order entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Update(Order entity)
        {
            throw new NotImplementedException();
        }
    }
}
