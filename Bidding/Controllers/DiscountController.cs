using GoldBank.Application.IApplication;
using GoldBank.Models;
using GoldBank.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        public ILogger logger { get; set; }
        public IDiscountApplication DiscountApplication { get; set; }
        public DiscountController(IConfiguration configuration, ILogger<DiscountController> logger, IDiscountApplication DiscountApplication, IDocumentApplication DocumentApplication)
        {
            this.DiscountApplication = DiscountApplication;
            this.logger = logger;
        }

        [HttpPost("Add")]
        public async Task<int> Add(Discount Discount)
        {
            return await this.DiscountApplication.Add(Discount);
        }

        [HttpPost("Update")]
        public async Task<bool> Update(Discount Discount)
        {
            return await this.DiscountApplication.Update(Discount);
        }

        [HttpGet("Get")]
        public async Task<Discount> GetById([FromQuery] int DiscountId)
        {
            var Discount = new Discount { DiscountId = DiscountId };
            return await this.DiscountApplication.Get(Discount);
        }

        [HttpPost("GetAllDiscounts")]
        public async Task<AllResponse<Discount>> GetAllDiscounts(AllRequest<DiscountRM> Discount)
        {
            return await this.DiscountApplication.GetAllDiscounts(Discount);
        }
    }
}
