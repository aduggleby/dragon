using System.Threading.Tasks;
using Dragon.SecurityServer.GenericSTSClient.Models;

namespace Dragon.SecurityServer.AccountSTS.Client
{
    public interface IClient
    {
        void SetHmacSettings(HmacSettings settings);
        Task ClearCache();
        string GetManagementUrl(string action, string replyUrl);
        string GetManagementUrl(string action, string data, string replyUrl);
        Task Update<T>(T model);
    }
}