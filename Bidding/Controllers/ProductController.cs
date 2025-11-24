using ExcelDataReader;
using GoldBank.Application.Application;
using GoldBank.Application.IApplication;
using GoldBank.Models;
using GoldBank.Models.Product;
using GoldBank.Models.RequestModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text;

namespace GoldBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductController : ControllerBase
    {
        public IProductApplication ProductApplication { get; }
        public IDocumentApplication DocumentApplication { get; }
        public ILogger logger { get; set; }
        public ProductController(IConfiguration configuration, ILogger<ProductController> logger, IProductApplication productApplication, IDocumentApplication DocumentApplication)
        {
            this.ProductApplication = productApplication;
            this.logger = logger;
            this.DocumentApplication = DocumentApplication;
        }


        [HttpPost("Add")]
        public async Task<int> Add(Product product)
        {
            return await ProductApplication.Add(product);
        }

        [HttpPost("Update")]
        public async Task<bool> Update([FromBody] Product product)
        {
            return await ProductApplication.Update(product);
        }
        [HttpGet("Get")]
        public async Task<Product> GetById([FromQuery] int productId)
        {
            var product = new Product { ProductId = productId };
            return await ProductApplication.Get(product);
        }
        [HttpPost("DeleteProduct")]
        public async Task<bool> DeleteProduct(Product product)
        {
            return await this.ProductApplication.DeleteProduct(product);
        }
        [HttpPost("GetAll")]
        public async Task<AllResponse<Product>> GetList([FromBody] AllRequest<ProductRequestVm> product)
        {
            return await ProductApplication.GetAllProducts(product);
        }
        [HttpGet("GetById")]
        public async Task<Product> GetProductById([FromQuery] int productId)
        {
            return await ProductApplication.GetProductById(productId);
        }
        [HttpPost("UploadImage")]
        public async Task<int> UploadImage(Document Document)
        {
            return await ProductApplication.UploadImage(Document);
        }
        [HttpPost("DeleteProductStone")]
        public async Task<bool> DeleteProductStone([FromBody] StoneProduct stone)
        {
            return await ProductApplication.DeleteProductStone(stone);
        }

        [HttpPost("AddOrder")]
        public async Task<int> AddOrder(Order order)
        {
            return await ProductApplication.AddOrder(order);
        }
        [HttpPost("GetAllOrders")]
        public async Task<AllResponse<Order>> GetAllOrders([FromBody] AllRequest<OrderRequestVm> order)
        {
            return await ProductApplication.GetAllOrders(order);
        }
        [HttpPost("UpdateOrder")]
        public async Task<bool> UpdateOrder([FromBody] Order order)
        {
            return await ProductApplication.UpdateOrder(order);
        }
        [HttpGet("GetOrderById")]
        public async Task<Order> GetOrderById([FromQuery] int orderId)
        {
            return await ProductApplication.GetOrderById(orderId);
        }
        [HttpPost("UpdateOrderById")]
        public async Task<bool> UpdateOrderById([FromBody] OrderStatusReqVm order)
        {
            return await ProductApplication.UpdateOrderById(order);
        }
        [HttpPost("AddUpdateOrderDeliveryDetails")]
        public async Task<bool> AddUpdateOrderDeliveryDetails([FromBody] OrderDelievery order)
        {
            return await ProductApplication.AddUpdateOrderDeliveryDetails(order);
        }
        [HttpPost("DeleteOrder")]
        public async Task<bool> DeleteOrder(Order orderId)
        {
            return await this.ProductApplication.DeleteOrder(orderId);
        }
        [HttpGet("GetOrderStatusByTypeId")]
        public async Task<List<OrderStatusCount>> GetOrderStatusByTypeId([FromQuery]int orderTypeId)
        {
            return await this.ProductApplication.GetOrderStatusByTypeId(orderTypeId);
        }

        [HttpPost("BulkImport")]
        public async Task<bool> BulkImport(Document document)
        {
            return await ProductApplication.BulkImport(document);
        }
        [HttpPost("AddVendor")]
        public async Task<int> AddVendor(Vendor Vendor)
        {
            return await ProductApplication.AddVendor(Vendor);
        }
        [HttpPost("GetAllVendors")]
        public async Task<List<Vendor>> GetAllVendors()
        {
            return await ProductApplication.GetAllVendors();
        }
        [HttpPost("UpdateVendor")]
        public async Task<bool> UpdateVendor([FromBody] Vendor Vendor)
        {
            return await ProductApplication.UpdateVendor(Vendor);
        }
        [HttpGet("GetVendorById")]
        public async Task<Vendor> GetVendorById([FromQuery] int VendorId)
        {
            return await ProductApplication.GetVendorById(VendorId);
        }

        [HttpPost("AddRawGold")]
        public async Task<int> AddRawGold(RawGold RawGold)
        {
            return await ProductApplication.AddRawGold(RawGold);
        }
        [HttpPost("GetAllRawGolds")]
        public async Task<List<RawGold>> GetAllRawGolds()
        {
            return await ProductApplication.GetAllRawGolds();
        }
        [HttpPost("RemoveRawGold")]
        public async Task<bool> RemoveRawGold([FromBody] RawGold RawGold)
        {
            return await ProductApplication.RemoveRawGold(RawGold);
        }
        [HttpGet("GetRawGoldById")]
        public async Task<RawGold> GetRawGoldById([FromQuery] int RawGoldId)
        {
            return await ProductApplication.GetRawGoldById(RawGoldId);
        }
        [HttpGet("GetAssetSummary")]
        public async Task<AssetSummary> GetAssetSummary()
        {
            return await ProductApplication.GetAssetSummary();
        }
        [HttpPost("AddUpdateMetalPurity")]
        public async Task<bool> AddUpdateMetalPurity([FromBody] List<MetalPurity> metalPurities)
        {
            return await ProductApplication.AddUpdateMetalPurity(metalPurities);
        }
        [HttpPost("GetMetalPurityHistory")]

        public async Task<IEnumerable<MetalPurity>> GetMetalPurityHistory([FromBody] MetalPurityVm entity)
        {
            return await ProductApplication.GetMetalPurityHistory(entity);
        }

        [HttpPost("AddUpdateLabel")]
        public async Task<int> AddUpdateLabel([FromBody]Label label)
        {
            return await this.ProductApplication.AddUpdateLabel(label);
        }

        [HttpPost("AddProductsLabel")]
        public async Task<int> AddProductsLabel([FromBody]ProductLabel productLabel)
        {
            return await this.ProductApplication.AddProductsLabel(productLabel);
        }
        private async Task<string> ToCSV(DataTable dtDataTable)
        {
            var builder = new StringBuilder();
            foreach (DataRow dr in dtDataTable.Rows)
            {
                for (int i = 0; i < dtDataTable.Columns.Count; i++)
                {
                    if (!Convert.IsDBNull(dr[i]))
                    {
                        string value = dr[i].ToString();
                        if (value.Contains(","))
                        {
                            value = string.Format("\"{0}\"", value);
                            builder.Append(value);
                        }
                        else
                        {
                            builder.Append(dr[i].ToString());
                        }
                    }
                    if (i < dtDataTable.Columns.Count - 1)
                    {
                        builder.Append(",");
                    }
                }
                builder.Append(Environment.NewLine);
            }

            var csv = builder.ToString();
            return csv;
        }
    }
}
