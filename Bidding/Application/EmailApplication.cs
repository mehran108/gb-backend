using Bidding.Connector;
using Bidding.Infrastructure;
using Bidding.Models;
using Microsoft.Extensions.Configuration;

namespace Bidding.Application
{
    public class EmailApplication : IEmailApplication
    {
        public EmailApplication(IConfiguration configuration,
            IServiceConnector serviceConnector,
            IAccountInfrastructure accountInfrastructure,
            IEmailInfrasutructure emailInfrasutructure
            )
        {
            Configuration = configuration;
            ServiceConnector = serviceConnector;
            AccountInfrastructure = accountInfrastructure;
            EmailInfrastructure = emailInfrasutructure;
            //ProspectInfrasutructure= prospectInfrasutructure;
        }
        public IConfiguration Configuration { get; }

        public IServiceConnector ServiceConnector { get; set; }
        public IAccountInfrastructure AccountInfrastructure { get; set; }
        public IEmailInfrasutructure EmailInfrastructure { get; set; }
        string codetemplate_Createccount = "CreateAccount";
        string codetemplate_ResetPw = "resetpw";
        string required_name = "{{UserName}}";
        string required_link = "{{link}}";
        public static TimeSpan TimeInterval(DateTime RecentModifieddate)
        {
            var today = DateTime.Now.ToUniversalTime();
            var diffOfDates = today.Subtract(RecentModifieddate);
            return diffOfDates;
        }
        public async Task<bool> SendEmailProspect(string email)
        {
            {
                Email message = new Email();

                message.Subject = "Forget Password Email";
                message.Body = "Please reset your password by clicking on this  " + this.Configuration["ResetPasswordUrl"] + " ";
                message.To = email;

                return await this.ServiceConnector.sendEmail(message);


            }


        }

        public async Task<bool> EmailTemplateCreateaccount(int id)
        {

            var resultuser = await this.AccountInfrastructure.GetUserById(id);

            var resulttemplate = await this.EmailInfrastructure.GetTemplateBycode(codetemplate_Createccount);
            {
                Email message = new Email();

                message.Subject = resulttemplate.Subject;
                message.Body = resulttemplate.Body.Replace(required_name, resultuser.FirstName).Replace(required_link, this.Configuration["ResetPasswordUrl"]);//"Please reset your password by clicking on this  " + this.Configuration["ResetPasswordUrl"] + " ";
                message.To = resultuser.Email;

                bool result = await this.ServiceConnector.sendEmail(message);

                return result;
            }



        }

        public async Task<bool> EmailTemplateResetPw(int id)
        {

            var resultuser = await this.AccountInfrastructure.GetUserById(id);

            var resulttemplate = await this.EmailInfrastructure.GetTemplateBycode(codetemplate_ResetPw);
            {
                Email message = new Email();

                message.Subject = resulttemplate.Subject;
                message.Body = resulttemplate.Body.Replace(required_name, resultuser.FirstName).Replace(required_link, this.Configuration["ResetPasswordUrl"]);//"Please reset your password by clicking on this  " + this.Configuration["ResetPasswordUrl"] + " ";
                message.To = resultuser.Email;

                bool result = await this.ServiceConnector.sendEmail(message);
                return result;

            }



        }

    }
}
