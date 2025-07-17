using GoldBank.Models;
using GoldBank.Models.Product;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Application.IApplication
{
    public interface IProductApplication : IBaseApplication<Product>
    {
        Task<int> UploadImage(Document Document);
        Task<bool> BulkImport(Document document);
        Task<AllResponse<Product>> GetAllProducts(AllRequest<ProductRequestVm> product);
        Task<Product> GetProductById(int productId);
        Task<int> AddOrder(Order order);
        Task<AllResponse<Order>> GetAllOrders(AllRequest<OrderRequestVm> order);
        Task<bool> UpdateOrder(Order order);
        Task<Order> GetOrderById(int orderId);
        Task<bool> UpdateOrderById(Order order);
        Task<bool> DeleteOrder(Order orderId);
    }
}
