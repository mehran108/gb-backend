using GoldBank.Application;
using GoldBank.Application.IApplication;
using GoldBank.Extensions;
using GoldBank.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace GoldBank.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ApplicationUserController : ControllerBase
    {
        private readonly JwtSettings jwtSettings;
        public IApplicationUserApplication applicationUserApplication { get; set; }
        public IEmailApplication EmailApplication { get; set; }
        public ApplicationUserController(IApplicationUserApplication ApplicationUserApplication, JwtSettings jwtSettings, IEmailApplication emailApplication)
        {
            applicationUserApplication = ApplicationUserApplication;
            EmailApplication = emailApplication;
            this.jwtSettings = jwtSettings;
        }

        [HttpPost("Add")]
        public async Task<int> Add([FromBody] ApplicationUser ApplicationUser)
        {
            string pwd = "GoldBank@1234";

            ApplicationUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(pwd);
            return await applicationUserApplication.Add(ApplicationUser);
        }


        [HttpPut("Update")]
        public async Task<bool> Update([FromBody] ApplicationUser ApplicationUser)
        {
            return await applicationUserApplication.Update(ApplicationUser);
        }


        [HttpGet("Get")]
        public async Task<ApplicationUser> GetById([FromQuery] int? ApplicationUserId, string? Email = "")
        {
            ApplicationUser applicationUser = new ApplicationUser();
            applicationUser.ApplicationUserId = ApplicationUserId ?? 0;
            applicationUser.Email = Email;
            return await applicationUserApplication.GetById(applicationUser);
        }


        [HttpGet("GetAll")]
        public async Task<List<ApplicationUser>> GetAll()
        {
            ApplicationUser applicationUser = new ApplicationUser();

            return await applicationUserApplication.GetAll(applicationUser);
        }

        [HttpPut("Activate")]
        public async Task<bool> Activate([FromBody] ApplicationUser ApplicationUser)
        {
            return await applicationUserApplication.Activate(ApplicationUser);
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login user)
        {
            ApplicationUser applicationUser = new ApplicationUser();

            applicationUser.Email = user.Email;
            var resultUser = await this.applicationUserApplication.GetById(applicationUser);

            if (resultUser.Email != null && resultUser.Active != false)

            {
                    bool verified = BCrypt.Net.BCrypt.Verify(user.Password, resultUser.PasswordHash);
                    if (verified)
                    {
                        var Token = new UserTokens();
                        Token = JwtHelpers.GenTokenkey(new UserTokens()
                        {
                            Email = user.Email,
                            UserId = resultUser.ApplicationUserId,
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