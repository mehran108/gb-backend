using GoldBank.Models;

namespace GoldBank.Application.IApplication
{
    public interface IDocumentApplication : IBaseApplication<Document>
    {
        Task<int> UploadImage(Document Document);
    }
}
