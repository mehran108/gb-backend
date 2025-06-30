using Bidding.Models;

namespace Bidding.Infrastructure.IInfrastructure
{
    public interface ICustomerInfrastructure
    {
        public Task<int> Add(Customer Customer);
        public Task<bool> Update(Customer Customer);
        public Task<Customer> GetById(Customer Customer);
        public Task<List<Customer>> GetAll(Customer Customer);
        public Task<bool> Activate(Customer Customer);
    }
}
