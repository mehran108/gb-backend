using GoldBank.Models;

namespace GoldBank.Infrastructure.IInfrastructure
{
    public interface ILookupInfrastructure : IBaseInfrastructure<LookupValue>
    {
        Task<int> Add(LookupValue entity);
        Task<List<LookupValue>> GetLookupByCode(LookupValue entity);
    }
}
