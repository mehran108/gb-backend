using GoldBank.Application.IApplication;
using GoldBank.Models;
using GoldBank.Models.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;

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

        [HttpGet("GetPaymentById")]
        public async Task<Payment> GetById([FromQuery] int paymentId)
        {
            var Payment = new Payment { PaymentId = paymentId };
            return await this.PaymentApplication.Get(Payment);
        }

        //[HttpPost("GetAll")]
        //public async Task<AllResponse<Payment>> GetAll(AllRequest<Payment> Payment)
        //{
        //    return await this.PaymentApplication.GetAll(Payment);
        //}

        [HttpPost("GetAllOnlinePayments")]
        public async Task<AllResponse<OnlinePaymentVerificationVM>> GetAllOnlinePayments(AllRequest<OnlinePaymentVerificationRM> Payment)
        {
            return await this.PaymentApplication.GetAllOnlinePayments(Payment);
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
        public async Task<ActionResult<bool?>> CheckOnlinePaymentStatus([FromQuery] int onlinePaymentId)
        {
            var res = await this.PaymentApplication.CheckOnlinePaymentStatus(onlinePaymentId);
            if (res == null)
                return Ok(); // returns 200 OK with no body

            return Ok(res); // returns 200 OK with value
        }
        [HttpPost("CancelPayment")]
        public async void CancelPayment( int paymentId)
        {
            this.PaymentApplication.CancelPayment(paymentId);
        }
        [HttpPost("CancelOnlinePayment")]
        public async void CancelOnlinePayment( int onlinePaymentId)
        {
            this.PaymentApplication.CancelOnlinePayment(onlinePaymentId);
        }

        [HttpPost("GetAccessToken")]
        public async Task<ActionResult<string>> GetAccessToken([FromBody] PaymentTransaction request)
        {
            var environment = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";

            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .Build();

            var url = config["Transaction:URL"];

            PaymentTransaction payload = new PaymentTransaction();
            payload.MERCHANT_ID = request.MERCHANT_ID;
            payload.TXNAMT = request.TXNAMT;
            payload.CURRENCY_CODE = request.CURRENCY_CODE;
            payload.SECURED_KEY = config["Transaction:SECURED_KEY"] ?? "zWHjBp2AlttNu1sK";
            var accessToken = "";

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                try
                {
                    var response = await client.PostAsync(url, content);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    accessToken = responseBody;
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine("HTTP Request Error:");
                    Console.WriteLine(e.Message);
                }
            }
            return Ok(accessToken);
        }
        [HttpPost("AddECommercePayment")]
        public async Task<int> AddOnlinePayment([FromBody] ECommercePayment eCommercePayment)
        {
            return await this.PaymentApplication.AddECommercePayment(eCommercePayment);
        }
        [HttpPost("ConfirmECommercePayment")]
        public async Task<int> VerifyECommercePayment([FromBody] ECommercePayment eCommercePayment)
        {
            return await this.PaymentApplication.VerifyECommercePayment(eCommercePayment);
        }
        [HttpPost("UpdateECommercePayment")]
        public async Task<bool> UpdateECommercePayment([FromBody] ECommercePayment eCommercePayment)
        {
            return await this.PaymentApplication.UpdateECommercePayment(eCommercePayment);
        }
        [HttpGet("GetECommercePaymentById")]
        public async Task<ECommercePayment> GetECommercePaymentById([FromQuery] string basketId)
        {
            return await this.PaymentApplication.GetECommercePaymentById(basketId);
        }

    }
}
