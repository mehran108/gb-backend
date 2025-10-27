using GoldBank.Models;
using GoldBank.Models.Product;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Application.IApplication
{
    public interface ICustomerApplication : IBaseApplication<Customer>
    {
        Task<bool> Delete(Customer customer);
        Task<List<CustomerSummary>> GetCustomerSummary(int customerId);
        Task<int> AddSalesPersonFeedback(SalesPersonFeedback entity);
        Task<int> AddCustomerFeedback(CustomerFeedback entity);
    }
}
