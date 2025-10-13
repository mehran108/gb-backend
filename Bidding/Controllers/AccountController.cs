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
    public class AccountController : ControllerBase
    {
        private readonly JwtSettings jwtSettings;        
        public IAccountApplication AccountApplication { get; set; }
        public IApplicationUserApplication ApplicationUserApplication { get; set; }
        public IEmailApplication EmailApplication { get; set; }
        public AccountController(IAccountApplication accountApplication, JwtSettings jwtSettings, IEmailApplication emailApplication, IApplicationUserApplication applicationUserApplication)
        {           
            AccountApplication = accountApplication;
            ApplicationUserApplication = applicationUserApplication;
            EmailApplication= emailApplication;
            this.jwtSettings = jwtSettings;
        }




        //[HttpPost("GetToken")]
        //public IActionResult GetToken(Login userLogins)
        //{
        //    try
        //    {
        //        string passwordHash = BCrypt.Net.BCrypt.HashPassword(userLogins.Password);
        //        bool verified = BCrypt.Net.BCrypt.Verify(userLogins.Password, passwordHash);

        //        var Token = new UserTokens();
        //        var Valid = logins.Any(x => x.Email.Equals(userLogins.UserName, StringComparison.OrdinalIgnoreCase));
        //        if (Valid)
        //        {
        //            var user = logins.FirstOrDefault(x => x.Email.Equals(userLogins.UserName, StringComparison.OrdinalIgnoreCase));
        //            Token = JwtHelpers.GenTokenkey(new UserTokens()
        //            {
        //                EmailId = user.Email,                                              
        //                Id = user.Id,
        //            }, jwtSettings);
        //        }
        //        else
        //        {
        //            return BadRequest("wrong password");
        //        }
        //        return Ok(Token);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}


       // [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetInfo")]
        public string GetInfo()
        {
            return "GrowBots Project for lead generation";
        }


        [HttpPost("Login")]
        public async Task<IActionResult> Login(Login user)
        {
            var applicationUser = new ApplicationUser { Email = user.Email };
            var resultUser = await this.ApplicationUserApplication.Get(applicationUser);
            if (resultUser.Email != null )
                
            {
                if (resultUser.IsActive != false)
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
                else
                {
                    return BadRequest("Not Allowed");
                }
            }

            return BadRequest("Username does not exists. Please enter valid username");


        }


        [HttpPost("RegisterUser")]
        public async Task<ActionResult<int>> RegisterUser([FromBody] User user)
        {

            var resultuser = await this.AccountApplication.GetUserByEmail(user.Email);
            if (resultuser.Email == null)
            {
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.NewPwd);
                user.PasswordHash = passwordHash;
                int result = await this.AccountApplication.RegisterUser(user);
                return Ok(result);
            }
            else
                return BadRequest("Email Already Exists");
        }

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<bool>> UpdateUser([FromBody] User user)
        {
            var resultuser = await this.AccountApplication.GetUserById(user.UserId);
            var reultuseremail = await this.AccountApplication.GetUserByEmail(user.Email);
            if (reultuseremail.Email == null || resultuser.Email == user.Email)
            {
                bool result = await this.AccountApplication.UpdateUser(user);
                return Ok(result);
            }
            else
                return BadRequest("Email Already Exist");

        }


        [HttpGet("GetUserByEmail")]
        public async Task<ApplicationUser> GetUserByEmail([FromQuery] string email)
        {
            var applicationUser = new ApplicationUser { Email = email };
            return  await this.ApplicationUserApplication.Get(applicationUser);
        }

        [HttpGet("GetUserById")]
        public async Task<ApplicationUser> GetUserById([FromQuery] int UserId)
        {
            var applicationUser = new ApplicationUser { ApplicationUserId = UserId };
            return await this.ApplicationUserApplication.Get(applicationUser);
        }
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet("GetUserList")]
        public async Task<List<User>> GetUserList()
        {
            return await this.AccountApplication.GetUserList();
        }

        [HttpPut("ForgotPassword")]
        public async Task<bool> ForgotPassword([FromQuery] string email)
        {
            return await this.AccountApplication.ForgotPassword(email);
        }

        [HttpPut("PasswordReset")]
        public async Task<bool> PasswordReset([FromBody] User User)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(User.PasswordHash);
            User.PasswordHash = passwordHash;
            return await this.AccountApplication.PasswordReset(User);
        }
        [HttpPut("ChangePassword")]
        public async Task<ActionResult<bool>> ChangePassword([FromBody] User User)
        {
            var resultUser = await this.AccountApplication.GetUserById(User.UserId);

            bool verified = BCrypt.Net.BCrypt.Verify(User.OldPwd, resultUser.PasswordHash);
            if (verified)
            {
                string newpassword = BCrypt.Net.BCrypt.HashPassword(User.NewPwd);
                User.PasswordHash = newpassword;
                Response.Redirect("https://localhost:7051/api/Account/Login");
                await this.AccountApplication.ChangePassword(User);
                return Ok(true);
            }
            // Console.WriteLine(resultUser.UserId);
            return BadRequest("Invalid Old Password");

        }

        [HttpPut("Activate")]
        public async Task<ActionResult<bool>> ActiveNonActive([FromBody] User User)
        {
            
            bool result= await this.AccountApplication.ActiveNonActive(User);
            if (result == true)
            {
                return Ok(true);
            }
            else
                return BadRequest("Not Updated");
        }
        //[HttpGet("GetUserListPagination")]
        //public async Task<List<User>> GetUserListPagination( )
        //{
        //    return await this.AccountApplication.GetUserListPagination();
        //}

        [HttpPost("UserPagination")]
        public async Task<Request<User>> GetUserPagination([FromBody] Request<User> request)
        {

            return await this.AccountApplication.GetUserPagination(request);
        }
        [HttpGet("Userearching")]
        public async Task<List<User>> Userearching([FromQuery] string Target)
        {
            return await this.AccountApplication.Userearching(Target);
        }

        [HttpPost("Sorting")]
        public async Task<Request<User>> Userorting([FromBody] Request<User> request)
        {

            return await this.AccountApplication.Userorting(request);
        }

    }
}