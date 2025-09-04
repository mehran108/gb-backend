using GoldBank.Models;
using GoldBank.Models.Product;
using GoldBank.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Infrastructure.IInfrastructure
{
    public interface IProductInfrastructure : IBaseInfrastructure<Product>
    {
        Task<bool> BulkImport(Document document);
        Task<AllResponse<Product>> GetAllProducts(AllRequest<ProductRequestVm> product);
        Task<Product> GetProductById(int productId);
        Task<int> AddOrder(Order order);
        Task<AllResponse<Order>> GetAllOrders(AllRequest<OrderRequestVm> order);
        Task<bool> UpdateOrder(Order order);
        Task<Order> GetOrderById(int orderId);
        Task<bool> UpdateOrderById(OrderStatusReqVm order);
        Task<bool> DeleteOrder(Order orderId);
        Task<List<OrderStatusCount>> GetOrderStatusByTypeId(int orderTypeId);
        Task<int> AddAlterationDetails (AlterationDetails alterationDetails);
        Task<bool> UpdateAlterationDetails(AlterationDetails alterationDetails);
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
        Task<AssetSummary> GetAssetSummary();
    }
}
