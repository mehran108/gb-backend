using Bidding.Application;
using Bidding.Application.IApplication;
using Bidding.Extensions;
using Bidding.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Bidding.Controllers
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
            string pwd = "Bidding@1234";
           
            Customer.PasswordHash = BCrypt.Net.BCrypt.HashPassword(pwd);
            return await this.customerApplication.Add(Customer);
        }


        [HttpPut("Update")]
        public async Task<bool> Update([FromBody]Customer Customer)
        {
            return await this.customerApplication.Update(Customer);
        }


        [HttpGet("Get")]
        public async Task<Customer> GetById([FromQuery] int? CustomerId,  string? Email = "")
        {
            Customer Customer = new Customer();
            Customer.CustomerId = CustomerId ?? 0;
            Customer.Email = Email;
            return await this.customerApplication.GetById(Customer);
        }


        [HttpGet("GetAll")]
        public async Task<List<Customer>> GetAll()
        {
            Customer Customer = new Customer();

            return await this.customerApplication.GetAll(Customer);
        }

        [HttpPut("Activate")]
        public async Task<bool> Activate([FromBody]Customer Customer)
        {
            return await this.customerApplication.Activate(Customer);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login user)
        {
            Customer customer = new Customer();

            customer.Email = user.Email;
            var resultUser = await this.customerApplication.GetById(customer);

            if (resultUser.Email != null && resultUser.Active != false)

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