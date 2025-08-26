using GoldBank.Application.IApplication;
using GoldBank.Infrastructure.IInfrastructure;
using GoldBank.Models;
using GoldBank.Models.Product;
using GoldBank.Models.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Application.Application
{
    public class ProductApplication : IBaseApplication<Product>, IProductApplication
    {
        public ProductApplication(IProductInfrastructure ProductInfrastructure, IConfiguration configuration, ILogger<Product> logger, IDocumentApplication DocumentApplication)
        {
            this.ProductInfrastructure = ProductInfrastructure;
            this.DocumentApplication = DocumentApplication;
        }

        public IProductInfrastructure ProductInfrastructure { get;}
        public IDocumentApplication DocumentApplication { get;}
        public async Task<bool> Activate(Product entity)
        {
            return await ProductInfrastructure.Activate(entity);
        }

        public async Task<int> Add(Product entity)
        {
            return await ProductInfrastructure.Add(entity);
        }

        public async Task<Product> Get(Product entity)
        {
            return await ProductInfrastructure.Get(entity);
        }

        public async Task<List<Product>> GetList(Product entity)
        {
            return await ProductInfrastructure.GetList(entity);
        }

        public async Task<bool> Update(Product entity)
        {
            return await ProductInfrastructure.Update(entity);
        }
        public async Task<int> UploadImage(Document Document)
        {
            return await DocumentApplication.UploadImage(Document);            
        }
        public Task<AllResponse<Product>> GetAll(AllRequest<Product> entity)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> BulkImport(Document document)
        {
            var dId = await DocumentApplication.UploadFile(document);
            if (dId > 0)
            {
                return await ProductInfrastructure.BulkImport(document);
            }
            return false;
        }
        public async Task<AllResponse<Product>> GetAllProducts(AllRequest<ProductRequestVm> product)
        {
            return await ProductInfrastructure.GetAllProducts(product);
        }
        public async Task<Product> GetProductById(int productId)
        {
            return await ProductInfrastructure.GetProductById(productId);
        }
        public async Task<int> AddOrder(Order order)
        {
            return await ProductInfrastructure.AddOrder(order);
        }
        public async Task<AllResponse<Order>> GetAllOrders(AllRequest<OrderRequestVm> order)
        {
            return await ProductInfrastructure.GetAllOrders(order);
        }
        public async Task<bool> UpdateOrder(Order order)
        {
            return await ProductInfrastructure.UpdateOrder(order);
        }
        public async Task<Order> GetOrderById(int orderId)
        {
            return await ProductInfrastructure.GetOrderById(orderId);
        }
        public async Task<bool> UpdateOrderById(OrderStatusReqVm order)
        {
            return await this.ProductInfrastructure.UpdateOrderById(order);
        }
        public async Task<bool> DeleteOrder(Order orderId)
        {
            return await this.ProductInfrastructure.DeleteOrder(orderId);
        }
        public async Task<int> AddVendor(Vendor Vendor)
        {
            return await ProductInfrastructure.AddVendor(Vendor);
        }
        public async Task<List<Vendor>> GetAllVendors()
        {
            return await ProductInfrastructure.GetAllVendors();
        }
        public async Task<bool> UpdateVendor(Vendor Vendor)
        {
            return await ProductInfrastructure.UpdateVendor(Vendor);
        }
        public async Task<Vendor> GetVendorById(int VendorId)
        {
            return await ProductInfrastructure.GetVendorById(VendorId);
        }
        public async Task<bool> AddUpdateMetalPurity(List<MetalPurity> metalPurities)
        {
            return await this.ProductInfrastructure.AddUpdateMetalPurity(metalPurities);
        }
        public async Task<IEnumerable<MetalPurity>> GetMetalPurityHistory(MetalPurityVm entity)
        {
            return await this.ProductInfrastructure.GetMetalPurityHistory(entity);
        }
        public async Task<int> AddRawGold(RawGold RawGold)
        {
            return await this.ProductInfrastructure.AddRawGold(RawGold);
        }
        public async Task<List<RawGold>> GetAllRawGolds()
        {
            return await this.ProductInfrastructure.GetAllRawGolds();
        }
        public async Task<bool> RemoveRawGold(RawGold RawGold)
        {
            return await this.ProductInfrastructure.RemoveRawGold(RawGold);
        }
        public async Task<RawGold> GetRawGoldById(int rawGoldId)
        {
            return await this.ProductInfrastructure.GetRawGoldById(rawGoldId);
        }
        public async Task<AssetSummary> GetAssetSummary()
        {
            return await this.ProductInfrastructure.GetAssetSummary();
        }
    }
}
