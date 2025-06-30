using Bidding.Connector;
using Bidding.Infrastructure;
using Bidding.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bidding.Application
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
       
        public async Task<Users> GetUserByEmail(string email)
        {
            return await AccountInfrastructure.GetUserByEmail(email);
        }

        public async Task<Users> GetUserById(int UserId)
        {
            return await AccountInfrastructure.GetUserById(UserId);
        }

        public async Task<bool> ForgotPassword(string email)
        {
            Users user = await AccountInfrastructure.GetUserByEmail(email);
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

        public async Task<List<Users>> GetUsersList()
        {
            return await AccountInfrastructure.GetUsersList();

        }

        public async Task<int> RegisterUser(Users User)
        {
            return await AccountInfrastructure.RegisterUser(User);
        }

        public async Task<bool> UpdateUser(Users User)
        {
            return await AccountInfrastructure.UpdateUser(User);
        }

        public async Task<bool> PasswordReset(Users users)
        {
            return await AccountInfrastructure.PasswordReset(users);
        }
        public async Task<bool> ChangePassword([FromBody] Users users)
        {
            return await this.AccountInfrastructure.ChangePassword(users);
        }
        public async Task<bool> ActiveNonActive([FromBody] Users users)
        {
            return await this.AccountInfrastructure.ActiveNonActive(users);
        }
        public async Task<Request<Users>> GetUserPagination([FromBody] Request<Users> request)
        {
            return await this.AccountInfrastructure.GetUserPagination(request);
        }
        public async Task<Request<Users>> UserSorting([FromBody] Request<Users> request)
        {
             return await this.AccountInfrastructure.UserSorting(request);
        }
         public async Task<List<Users>> UserSearching(string Target)
        {         
           return await AccountInfrastructure.UserSearching(Target);           
        }
    }
}
