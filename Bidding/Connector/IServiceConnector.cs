using Bidding.Models;

namespace Bidding.Connector
{
    public interface IServiceConnector
    {
        Task<bool> sendEmail(Email email);
    }
}
