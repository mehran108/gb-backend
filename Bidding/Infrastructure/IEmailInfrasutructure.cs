using GoldBank.Models;

namespace GoldBank.Infrastructure
{
    public interface IEmailInfrasutructure
    {
        Task<EmailT> GetTemplateBycode(string code);
        Task<string> GetCodeByStatusid(int emailstatusid);
    }
}
