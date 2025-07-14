using GoldBank.Application.IApplication;
using GoldBank.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        public ILogger logger { get; set; }
        public IPaymentApplication PaymentApplication { get; set; }
        public PaymentController(IConfiguration configuration, ILogger<PaymentController> logger, IPaymentApplication PaymentApplication, IDocumentApplication DocumentApplication)
        {
            this.PaymentApplication = PaymentApplication;
            this.logger = logger;
        }

        [HttpPost("Add")]
        public async Task<int> Add(Payment Payment)
        {
            return await this.PaymentApplication.Add(Payment);
        }

        [HttpPost("Update")]
        public async Task<bool> Update(Payment Payment)
        {
            return await this.PaymentApplication.Update(Payment);
        }

        [HttpGet("Get")]
        public async Task<Payment> GetById([FromQuery] int PaymentId)
        {
            var Payment = new Payment { PaymentId = PaymentId };
            return await this.PaymentApplication.Get(Payment);
        }

        [HttpPost("GetAll")]
        public async Task<AllResponse<Payment>> GetAll(AllRequest<Payment> Payment)
        {
            return await this.PaymentApplication.GetAll(Payment);
        }
    }
}
