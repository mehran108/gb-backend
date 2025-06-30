using GoldBank.Models;

namespace GoldBank.Connector
{
    public interface IServiceConnector
    {
        Task<bool> sendEmail(Email email);
    }
}
