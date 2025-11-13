using GoldBank.Application.Application;
using GoldBank.Models;

namespace GoldBank.Application.IApplication
{
    public interface IDocumentApplication : IBaseApplication<Document>
    {
        Task<int> UploadImage(Document Document);
        Task<int> UploadFile(Document document);
        Task<bool> Delete(string documentIds);

    }
}
