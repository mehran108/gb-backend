using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;

namespace GoldBank.Application.Application
{
    public class OrderApplication : IBaseApplication<Order>, IOrderApplication
    {
        public OrderApplication(IOrderInfrastructure OrderInfrastructure, IConfiguration configuration, ILogger<Order> logger)
        {
            this.OrderInfrastructure = OrderInfrastructure;
        }

        public IOrderInfrastructure OrderInfrastructure { get; }

        public Task<bool> Activate(Order entity)
        {
            throw new NotImplementedException();
        }

        public async Task<int> Add(Order entity)
        {
            return await this.OrderInfrastructure.Add(entity);
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
