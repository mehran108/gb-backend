using GoldBank.Models;

namespace GoldBank.Application
{
    public interface ILookupApplication
    {

        Task<List<LookupValue>> AddLookupValue(LookupValue entity);
        Task<List<LookupValue>> UpdateLookupValue(LookupValue entity);
        Task<List<LookupValue>> GetLookupsAll(LookupValue entity);
        Task<List<LookupValue>> GetLookupByCode(LookupValue entity);
        
    }
}
