using GoldBank.Application.IApplication;
using GoldBank.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        public ILogger logger { get; set; }
        public IOrderApplication OrderApplication { get; set; }
        public OrderController(IConfiguration configuration, ILogger<OrderController> logger, IOrderApplication OrderApplication, IDocumentApplication DocumentApplication)
        {
            this.OrderApplication = OrderApplication;
            this.logger = logger;
        }

        [HttpPost("Add")]
        public async Task<int> Add(Order order)
        {
            return await this.OrderApplication.Add(order);
        }

        [HttpPost("Update")]
        public async Task<bool> Update(Order order)
        {
            return await this.OrderApplication.Update(order);
        }

        [HttpGet("Get")]
        public async Task<Order> GetById([FromQuery] int orderId)
        {
            var order  = new Order { OrderId = orderId };
            return await this.OrderApplication.Get(order);
        }

        [HttpPost("GetAll")]
        public async Task<AllResponse<Order>> GetAll(AllRequest<Order> order)
        {
            return await this.OrderApplication.GetAll(order);
        }
    }
}
