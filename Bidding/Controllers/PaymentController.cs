using GoldBank.Application.IApplication;
using GoldBank.Models;
using GoldBank.Models.RequestModels;
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
        [HttpPost("AddPayment")]
        public async Task<int> AddPayment([FromBody] AddPaymentRequest paymentRM)
        {
            return await this.PaymentApplication.AddPayment(paymentRM);
        }
        [HttpPost("AddOnlinePayment")]
        public async Task<int> AddOnlinePayment([FromBody] AddOnlinePaymentRequest paymentRM)
        {
            return await this.PaymentApplication.AddOnlinePayment(paymentRM);
        }
        [HttpPost("VerifyOnlinePayment")]
        public async Task<bool> VerifyOnlinePayment([FromBody] VerifyOnlinePaymentRequest verifyOnlinePaymentRequest)
        {
            return await this.PaymentApplication.VerifyOnlinePayment(verifyOnlinePaymentRequest);
        }
        [HttpPost("ConfirmPayment")]
        public async Task<bool> ConfirmPayment([FromBody] ConfirmPaymentRequest confirmPaymentRequest)
        {
            return await this.PaymentApplication.ConfirmPayment(confirmPaymentRequest);
        }
        [HttpGet("CheckOnlinePaymentStatus")]
        public async Task<bool?> CheckOnlinePaymentStatus([FromQuery] int onlinePaymentId)
        {
            return await this.PaymentApplication.CheckOnlinePaymentStatus(onlinePaymentId);
        }
        [HttpGet("CancelPayment")]
        public async void CancelPayment([FromQuery] int paymentId)
        {
            this.PaymentApplication.CancelPayment(paymentId);
        }
        [HttpGet("CancelOnlinePayment")]
        public async void CancelOnlinePayment([FromQuery] int onlinePaymentId)
        {
            this.PaymentApplication.CancelOnlinePayment(onlinePaymentId);
        }

    }
}
