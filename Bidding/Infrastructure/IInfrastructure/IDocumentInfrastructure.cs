using GoldBank.Application.Application;
using GoldBank.Models;

namespace GoldBank.Infrastructure.IInfrastructure
{
    public interface IDocumentInfrastructure : IBaseInfrastructure<Document>
    {
        Task<bool> Delete(string documentIds);        
    }
}
