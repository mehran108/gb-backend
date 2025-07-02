using GoldBank.Application.Application;
using GoldBank.Application.IApplication;
using GoldBank.Models;
using GoldBank.Models.Product;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
        public async Task<bool> Update(Product product)
        { 
            return await ProductApplication.Update(product);
        }
        [HttpGet("Get")]
        public async Task<Product> GetById([FromQuery]int productId)
        {
            var product = new Product { ProductId = productId };
            return await ProductApplication.Get(product);
        }
        [HttpPost("GetAll")]
        public async Task<AllResponse<Product>> GetList([FromBody] AllRequest<Product> product)
        {
            return await ProductApplication.GetAll(product);
        }
        [HttpPost("UploadImage")]
        public async Task<int> UploadImage(Document Document)
        {
            return await ProductApplication.UploadImage(Document);
        }
    }
}
