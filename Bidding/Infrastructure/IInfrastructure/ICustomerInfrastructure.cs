using GoldBank.Models;
using GoldBank.Models.Product;

namespace GoldBank.Infrastructure.IInfrastructure
{
    public interface ICustomerInfrastructure : IBaseInfrastructure<Customer>
    {
        Task<bool> Delete(Customer customer);
        Task<List<CustomerSummary>> GetCustomerSummary(int customerId);
    }
}
