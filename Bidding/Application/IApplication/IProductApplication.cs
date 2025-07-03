using GoldBank.Models;
using GoldBank.Models.Product;

namespace GoldBank.Application.IApplication
{
    public interface IProductApplication : IBaseApplication<Product>
    {
        Task<int> UploadImage(Document Document);
        Task<bool> BulkImport(string fileContent, ProductBulkImport productBulkImport);
    }
}
