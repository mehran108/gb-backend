using GoldBank.Models;

namespace GoldBank.Application.IApplication
{
    public interface ILookupApplication : IBaseApplication<LookupValue>
    {
        Task<List<LookupValue>> GetLookupByCode(LookupValue entity);
    }
}
