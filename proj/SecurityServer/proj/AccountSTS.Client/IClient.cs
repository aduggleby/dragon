using System.Threading.Tasks;

namespace Dragon.SecurityServer.AccountSTS.Client
{
    public interface IClient
    {
        Task ClearCache();
        string GetManagementUrl(string action, string replyUrl);
        string GetManagementUrl(string action, string data, string replyUrl);
        Task Update<T>(T model);
    }
}