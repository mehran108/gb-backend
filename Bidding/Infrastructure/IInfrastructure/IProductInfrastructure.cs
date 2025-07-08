using GoldBank.Models;
using GoldBank.Models.Product;

namespace GoldBank.Infrastructure.IInfrastructure
{
    public interface IProductInfrastructure : IBaseInfrastructure<Product>
    {
        Task<bool> BulkImport(ProductBulkImport productBulkImport);
        Task<AllResponse<Product>> GetAllProducts(AllRequest<ProductRequestVm> product);
        Task<Product> GetProductById(int productId);
    }
}
