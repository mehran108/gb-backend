using GoldBank.Application.IApplication;
using GoldBank.Extensions;
using GoldBank.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GoldBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly JwtSettings jwtSettings;        
        public ICustomerApplication customerApplication { get; set; }
        public IEmailApplication EmailApplication { get; set; }
        public CustomerController(ICustomerApplication CustomerApplication, JwtSettings jwtSettings, IEmailApplication emailApplication)
        {           
            customerApplication = CustomerApplication;
            EmailApplication= emailApplication;
            this.jwtSettings = jwtSettings;
        }      

        [HttpPost("Add")]
        public async Task<int> Add([FromBody]Customer Customer)
        {
            // to do : update hardcoded password to newpwd field
            Customer.PasswordHash = BCrypt.Net.BCrypt.HashPassword("Qwerty123@");
            return await this.customerApplication.Add(Customer);
        }


        [HttpPut("Update")]
        public async Task<bool> Update([FromBody]Customer Customer)
        {
            return await this.customerApplication.Update(Customer);
        }


        [HttpGet("GetById")]
        public async Task<Customer> Get([FromQuery] int customerId,  string? email = "")
        {
            Customer Customer = new Customer { CustomerId = customerId, Email = email };
            var res = await this.customerApplication.Get(Customer);
            res.PasswordHash = "";
            return res;
        }
        
        [HttpGet("GetCustomerSummary")]
        public async Task<List<CustomerSummary>> GetCustomerSummary([FromQuery] int customerId)
        {
            var res = await this.customerApplication.GetCustomerSummary(customerId);
            return res;
        }


        [HttpPost("GetAll")]
        public async Task<AllResponse<Customer>> GetAll(AllRequest<Customer> entity)
        {
            Customer Customer = new Customer();

            return await this.customerApplication.GetAll(entity);
        }

        [HttpPut("Delete")]
        public async Task<bool> Activate([FromBody]Customer Customer)
        {
            return await this.customerApplication.Delete(Customer);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login user)
        {
            Customer customer = new Customer();

            customer.Email = user.Email;
            var resultUser = await this.customerApplication.Get(customer);

            if (resultUser.Email != null && resultUser.IsActive != false)

            {
                bool verified = BCrypt.Net.BCrypt.Verify(user.Password, resultUser.PasswordHash);
                if (verified)
                {
                    var Token = new UserTokens();
                    Token = JwtHelpers.GenTokenkey(new UserTokens()
                    {
                        Email = user.Email,
                        UserId = resultUser.CustomerId,
                    }, jwtSettings);
                    return Ok(Token);
                }
                else
                {
                    return BadRequest("Invalid Username or Password");
                }
            }

            return BadRequest("Username does not exists or is Inactive.");
        }


    }
}