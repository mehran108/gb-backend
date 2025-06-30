using GoldBank.Connector;
using GoldBank.Infrastructure;
using GoldBank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GoldBank.Application
{
    public class AccountApplication : IAccountApplication
    {
        public AccountApplication(IConfiguration configuration, IAccountInfrastructure accountInfrastructure, IServiceConnector serviceConnector)
        {
            Configuration = configuration;
            AccountInfrastructure = accountInfrastructure;
            ServiceConnector = serviceConnector;
        }
        public IConfiguration Configuration { get; }
        public IAccountInfrastructure AccountInfrastructure { get; set; }
        public IServiceConnector ServiceConnector { get; set; }
       
        public async Task<User> GetUserByEmail(string email)
        {
            return await AccountInfrastructure.GetUserByEmail(email);
        }

        public async Task<User> GetUserById(int UserId)
        {
            return await AccountInfrastructure.GetUserById(UserId);
        }

        public async Task<bool> ForgotPassword(string email)
        {
            User user = await AccountInfrastructure.GetUserByEmail(email);
            if (user != null)
            {
                Email message = new Email();

                message.Subject = "Forget Password Email";
                message.Body = "Please reset your password by clicking on this  " +this.Configuration["ResetPasswordUrl"] + " ";
                message.To = email;

                return await this.ServiceConnector.sendEmail(message);               
            }
            else
            {
                return false;
            }

        }

        public async Task<List<User>> GetUserList()
        {
            return await AccountInfrastructure.GetUserList();

        }

        public async Task<int> RegisterUser(User User)
        {
            return await AccountInfrastructure.RegisterUser(User);
        }

        public async Task<bool> UpdateUser(User User)
        {
            return await AccountInfrastructure.UpdateUser(User);
        }

        public async Task<bool> PasswordReset(User User)
        {
            return await AccountInfrastructure.PasswordReset(User);
        }
        public async Task<bool> ChangePassword([FromBody] User User)
        {
            return await this.AccountInfrastructure.ChangePassword(User);
        }
        public async Task<bool> ActiveNonActive([FromBody] User User)
        {
            return await this.AccountInfrastructure.ActiveNonActive(User);
        }
        public async Task<Request<User>> GetUserPagination([FromBody] Request<User> request)
        {
            return await this.AccountInfrastructure.GetUserPagination(request);
        }
        public async Task<Request<User>> Userorting([FromBody] Request<User> request)
        {
             return await this.AccountInfrastructure.Userorting(request);
        }
         public async Task<List<User>> Userearching(string Target)
        {         
           return await AccountInfrastructure.Userearching(Target);           
        }
    }
}
