using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;

namespace GoldBank.Application.Application
{
    public class BankApplication : IBaseApplication<CompanyAccount>, IBankApplication
    {
        public BankApplication(IBankInfrastructure BankInfrastructure, IConfiguration configuration, ILogger<CompanyAccount> logger)
        {
            this.BankInfrastructure = BankInfrastructure;
        }

        public IBankInfrastructure BankInfrastructure { get; }

        public async Task<bool> Activate(CompanyAccount entity)
        {
            return await this.BankInfrastructure.Activate(entity);
        }

        public async Task<int> Add(CompanyAccount entity)
        {
            return await this.BankInfrastructure.Add(entity);
        }

        public async Task<CompanyAccount> Get(CompanyAccount entity)
        {
            return await this.BankInfrastructure.Get(entity);
        }

        public async Task<AllResponse<CompanyAccount>> GetAll(AllRequest<CompanyAccount> entity)
        {
            return await this.BankInfrastructure.GetAll(entity);
        }

        public async Task<List<CompanyAccount>> GetList(CompanyAccount entity)
        {
            return await this.BankInfrastructure.GetList(entity);
        }
        public async Task<bool> Update(CompanyAccount entity)
        {
            return await this.BankInfrastructure.Update(entity);
        }
    }
}
