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
        
        [HttpPost("AddVoucherType")]
        public async Task<int> AddVoucherType(VoucherType voucherType)
        {
            return await this.DiscountApplication.AddVoucherType(voucherType);
        }

        [HttpPost("UpdateVoucherType")]
        public async Task<bool> UpdateVoucherType(VoucherType voucherType)
        {
            return await this.DiscountApplication.UpdateVoucherType(voucherType);
        }

        [HttpGet("GetVoucherTypeById")]
        public async Task<VoucherType> GetVoucherTypeById([FromQuery] int voucherTypeId)
        {
            var voucherType = new VoucherType { VoucherTypeId = voucherTypeId };
            return await this.DiscountApplication.GetVoucherType(voucherType);
        }

        [HttpPost("GetAllVoucherType")]
        public async Task<List<VoucherType>> GetAllVoucherType()
        {
            var voucherType = new VoucherType();
            return await this.DiscountApplication.GetAllVoucherType(voucherType);
        }

        [HttpPost("AddLoyaltyCardType")]
        public async Task<int> AddLoyaltyCardType(LoyaltyCardType LoyaltyCardType)
        {
            return await this.DiscountApplication.AddLoyaltyCardType(LoyaltyCardType);
        }

        [HttpPost("UpdateLoyaltyCardType")]
        public async Task<bool> UpdateLoyaltyCardType(LoyaltyCardType LoyaltyCardType)
        {
            return await this.DiscountApplication.UpdateLoyaltyCardType(LoyaltyCardType);
        }

        [HttpGet("GetLoyaltyCardTypeById")]
        public async Task<LoyaltyCardType> GetLoyaltyCardTypeById([FromQuery] int LoyaltyCardTypeId)
        {
            var LoyaltyCardType = new LoyaltyCardType { LoyaltyCardTypeId = LoyaltyCardTypeId };
            return await this.DiscountApplication.GetLoyaltyCardType(LoyaltyCardType);
        }

        [HttpPost("GetAllLoyaltyCardType")]
        public async Task<List<LoyaltyCardType>> GetAllLoyaltyCardType()
        {
            var LoyaltyCardType = new LoyaltyCardType();
            return await this.DiscountApplication.GetAllLoyaltyCardType(LoyaltyCardType);
        }
        [HttpPost("UpdateDiscountStatus")]
        public async Task<bool> UpdateDiscountStatus([FromBody]Discount discount)
        {
            return await this.DiscountApplication.UpdateDiscountStatus(discount);
        }
    }
}
