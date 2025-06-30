using GoldBank.Models;

namespace GoldBank.Infrastructure
{
    public interface ILookupInfrastructure 
    {
        Task<List<LookupValue>> AddLookupValue(LookupValue entity);
        Task<List<LookupValue>> UpdateLookupValue(LookupValue entity);
        Task<List<LookupValue>> GetLookupsAll(LookupValue entity);
        Task<List<LookupValue>> GetLookupByCode(LookupValue entity);
    }
}
