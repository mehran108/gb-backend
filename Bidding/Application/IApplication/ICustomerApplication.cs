using GoldBank.Models;
using GoldBank.Models.Product;

namespace GoldBank.Application.IApplication
{
    public interface ICustomerApplication : IBaseApplication<Customer>
    {
        Task<bool> Delete(Customer customer);
    }
}
