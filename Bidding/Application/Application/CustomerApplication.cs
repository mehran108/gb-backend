using GoldBank.Application.IApplication;
using GoldBank.Connector;
using GoldBank.Infrastructure;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Infrastructure.Infrastructure;
using GoldBank.Models;

namespace GoldBank.Application.Application
{
    public class CustomerApplication : ICustomerApplication
    {
        public CustomerApplication(IConfiguration Configuration, ICustomerInfrastructure CustomerInfrastructure) 
        {
            this.Configuration = Configuration;
            this.CustomerInfrastructure = CustomerInfrastructure;
               
        }

        public IConfiguration Configuration { get; }
        public ICustomerInfrastructure CustomerInfrastructure { get; set; }
        public IServiceConnector ServiceConnector { get; set; }

        public async Task<bool> Activate(Customer Customer)
        {
            return await CustomerInfrastructure.Activate(Customer);
        }

        public async Task<int> Add(Customer Customer)
        {
            return await CustomerInfrastructure.Add(Customer);
        }

        public async Task<Customer> GetById(Customer Customer)
        {
            return await CustomerInfrastructure.GetById(Customer);
        }

        public async Task<List<Customer>> GetAll(Customer Customer)
        {
            return await CustomerInfrastructure.GetAll(Customer);
        }

        public async Task<bool> Update(Customer Customer)
        {
            return await CustomerInfrastructure.Update(Customer);
        }
    }
}
