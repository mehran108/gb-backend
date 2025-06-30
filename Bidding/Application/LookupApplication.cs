using GoldBank.Infrastructure;
using GoldBank.Models;

namespace GoldBank.Application
{
    public class LookupApplication
    {
        public LookupApplication(ILookupInfrastructure LookupInfrastructure, IConfiguration configuration, ILogger<LookupApplication> logger)
        {
            this.LookupInfrastructure = LookupInfrastructure;
        }

        public ILookupInfrastructure LookupInfrastructure { get; }
        public async Task<List<LookupValue>> AddLookupValue(LookupValue entity)
        {
            return await LookupInfrastructure.AddLookupValue(entity);
        }
        public async Task<List<LookupValue>> UpdateLookupValue(LookupValue entity)
        {
            return await LookupInfrastructure.UpdateLookupValue(entity);
        }
        public async Task<List<LookupValue>> GetLookupsAll(LookupValue entity)
        {
            return await LookupInfrastructure.GetLookupsAll(entity);
        }
        public async Task<List<LookupValue>> GetLookupByCode(LookupValue entity)
        {
            return await LookupInfrastructure.GetLookupByCode(entity);
        }
    }
}
