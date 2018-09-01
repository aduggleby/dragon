using System.Threading.Tasks;
using Dragon.SecurityServer.GenericSTSClient.Models;

namespace Dragon.SecurityServer.AccountSTS.Client
{
    public interface IClient
    {
        void SetHmacSettings(HmacSettings settings);
        Task ClearCache();
        string GetFederationUrl(string action, string replyUrl);
        string GetFederationUrl(string action, string data, string replyUrl);
        string GetApiUrl(string action, string replyUrl);
        string GetApiUrl(string action, string data, string replyUrl);
        Task Update<T>(T model);
        Task Delete(string userId);
    }
}