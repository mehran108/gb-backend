using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;

namespace GoldBank.Application.Application
{
    public class LookupApplication : IBaseApplication<LookupValue>, ILookupApplication
    {
        public LookupApplication(IProductInfrastructure ProductInfrastructure, IConfiguration configuration, ILogger<Product> logger)
        {
            LookupInfrastructure = LookupInfrastructure;
        }

        public ILookupInfrastructure LookupInfrastructure { get; }
        public async Task<int> Add(LookupValue entity)
        {
            return await LookupInfrastructure.Add(entity);
        }
        public async Task<List<LookupValue>> GetLookupByCode(LookupValue entity)
        {
            return await LookupInfrastructure.GetLookupByCode(entity);
        }


        public Task<bool> Activate(LookupValue entity)
        {
            throw new NotImplementedException();
        }

        Task<LookupValue> IBaseApplication<LookupValue>.Get(LookupValue entity)
        {
            throw new NotImplementedException();
        }

        Task<List<LookupValue>> IBaseApplication<LookupValue>.GetList(LookupValue entity)
        {
            throw new NotImplementedException();
        }

        Task<bool> IBaseApplication<LookupValue>.Update(LookupValue entity)
        {
            throw new NotImplementedException();
        }
    }
}
