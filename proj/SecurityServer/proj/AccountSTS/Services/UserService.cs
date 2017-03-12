using System.Threading.Tasks;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.Identity.Stores;

namespace Dragon.SecurityServer.AccountSTS.Services
{
    public class UserService : IUserService
    {
        private readonly IDragonUserStore<AppMember> _userStore;

        public UserService(IDragonUserStore<AppMember> userStore)
        {
            _userStore = userStore;
        }

        public Task<AppMember> GetUser(string userId)
        {
            return _userStore.FindByIdAsync(userId);
        }

        public async Task AddCurrentServiceIdToUserIfNotAlreadyAdded(AppMember user, string serviceId)
        {
            if (user != null && !await _userStore.IsUserRegisteredForServiceAsync(user, serviceId)) // user == null on registration
            {
                await _userStore.AddServiceToUserAsync(user, serviceId);
            }
        }

        public async Task AddCurrentAppIdToUserIfNotAlreadyAdded(AppMember user, string appId)
        {
            if (user != null && !await _userStore.IsUserRegisteredForAppAsync(user, appId)) // user == null on registration
            {
                await _userStore.AddAppToUserAsync(user, appId);
            }
        }
    }
}