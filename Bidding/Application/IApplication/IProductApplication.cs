using GoldBank.Models;
using GoldBank.Models.Product;
using GoldBank.Models.RequestModels;
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
        Task<bool> UpdateOrderById(OrderStatusReqVm order);
        Task<bool> DeleteOrder(Order orderId);
        Task<int> AddVendor(Vendor Vendor);
        Task<List<Vendor>> GetAllVendors();
        Task<bool> UpdateVendor(Vendor Vendor);
        Task<Vendor> GetVendorById(int VendorId);
        Task<bool> AddUpdateMetalPurity(List<MetalPurity> metalPurities);
        Task<IEnumerable<MetalPurity>> GetMetalPurityHistory(MetalPurityVm entity);
        Task<int> AddRawGold(RawGold RawGold);
        Task<List<RawGold>> GetAllRawGolds();
        Task<bool> RemoveRawGold(RawGold RawGold);
        Task<RawGold> GetRawGoldById(int rawGoldId);
    }
}
