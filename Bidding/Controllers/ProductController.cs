using GoldBank.Application.Application;
using GoldBank.Application.IApplication;
using GoldBank.Models;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        public IProductApplication ProductApplication { get; }
        public ICommonCodeApplication CommonCodeApplication { get; }
        public ILogger logger { get; set; }
        public ProductController(IConfiguration configuration, ILogger<ProductController> logger, IProductApplication productApplication, ICommonCodeApplication CommonCodeApplication)
        {
            this.ProductApplication = productApplication;
            this.logger = logger;
            this.CommonCodeApplication = CommonCodeApplication;
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
        [HttpPost("UploadImage")]
        public async Task<int> UploadImage(CommonCode commonCode)
        {
            return await ProductApplication.UploadImage(commonCode);
        }
    }
}
