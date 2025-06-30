using GoldBank.Models;

namespace GoldBank.Infrastructure
{
    public interface ILookupInfrastructure 
    {
        Task<int> AddLookupValue(LookupValue entity);
        Task<int> UpdateLookupValue(LookupValue entity);
        Task<int> GetLookupsAll(LookupValue entity);
        Task<int> GetLookupByCode(LookupValue entity);
    }
}
