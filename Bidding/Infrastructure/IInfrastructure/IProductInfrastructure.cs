using GoldBank.Models;
using GoldBank.Models.Product;

namespace GoldBank.Infrastructure.IInfrastructure
{
    public interface IProductInfrastructure : IBaseInfrastructure<Product>
    {
        Task<bool> BulkImport(Document document);
        Task<AllResponse<Product>> GetAllProducts(AllRequest<ProductRequestVm> product);
        Task<Product> GetProductById(int productId);
        Task<int> AddOrder(Order order);
        Task<AllResponse<OrderRequestVm>> GetAllOrders(AllRequest<OrderRequestVm> order);
        Task<bool> UpdateOrder(Order order);
    }
}
