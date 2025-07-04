using GoldBank.Application.IApplication;
using GoldBank.Connector;
using GoldBank.Infrastructure;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Infrastructure.Infrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;

namespace GoldBank.Application.Application
{
    public class CustomerApplication : IBaseApplication<Customer>, ICustomerApplication
    {
        public CustomerApplication(IConfiguration Configuration, ICustomerInfrastructure CustomerInfrastructure)
        {
            this.Configuration = Configuration;
            this.CustomerInfrastructure = CustomerInfrastructure;

        }

        public IConfiguration Configuration { get; }
        public ICustomerInfrastructure CustomerInfrastructure { get; set; }

        public async Task<bool> Activate(Customer customer)
        {
            return await CustomerInfrastructure.Activate(customer);
        }

        public async Task<int> Add(Customer customer)
        {
            return await CustomerInfrastructure.Add(customer);
        }

        public async Task<Customer> Get(Customer customer)
        {
            return await CustomerInfrastructure.Get(customer);
        }

        public async Task<AllResponse<Customer>> GetAll(AllRequest<Customer> customer)
        {
            return await CustomerInfrastructure.GetAll(customer);
        }

        public async Task<bool> Update(Customer customer)
        {
            return await CustomerInfrastructure.Update(customer);
        }


        public Task<List<Customer>> GetList(Customer entity)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> Delete(Customer customer)
        {
            return await CustomerInfrastructure.Delete(customer);
        }
    }
}
