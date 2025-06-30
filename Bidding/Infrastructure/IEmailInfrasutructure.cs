using Bidding.Models;

namespace Bidding.Infrastructure
{
    public interface IEmailInfrasutructure
    {
        Task<EmailT> GetTemplateBycode(string code);
        Task<string> GetCodeByStatusid(int emailstatusid);
    }
}
