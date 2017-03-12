using System.Threading.Tasks;
using Dragon.SecurityServer.AccountSTS.Models;

namespace Dragon.SecurityServer.AccountSTS.Services
{
    public interface IUserService
    {
        Task<AppMember> GetUser(string userId);
        Task AddCurrentServiceIdToUserIfNotAlreadyAdded(AppMember user, string serviceId);
        Task AddCurrentAppIdToUserIfNotAlreadyAdded(AppMember user, string appId);
    }
}
