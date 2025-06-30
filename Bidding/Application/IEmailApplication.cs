using GoldBank.Models;

namespace GoldBank.Application
{
    public interface IEmailApplication
    {
     
        Task<bool> SendEmailProspect(string email);
        
         Task<bool> EmailTemplateCreateaccount(int id);
        Task<bool> EmailTemplateResetPw(int id);
     }

}
