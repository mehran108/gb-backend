using ExcelDataReader;
using GoldBank.Application.Application;
using GoldBank.Application.IApplication;
using GoldBank.Models;
using GoldBank.Models.Product;
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
        public async Task<bool> UpdateOrderById([FromBody] Order order)
        {
            return await ProductApplication.UpdateOrderById(order);
        }

        [HttpPost("BulkImport")]
        public async Task<bool> BulkImport(Document document)
        {
            return await ProductApplication.BulkImport(document);
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
