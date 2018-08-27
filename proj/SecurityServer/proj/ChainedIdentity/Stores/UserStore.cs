using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dragon.SecurityServer.Common;
using Dragon.SecurityServer.Identity.Stores;
using Microsoft.AspNet.Identity;
using IUser = Dragon.SecurityServer.Identity.Models.IUser;

namespace Dragon.SecurityServer.ChainedIdentity.Stores
{
    public class UserStore<TUser> : IDragonUserStore<TUser> where TUser : class, IUser
    {
        private readonly List<IDragonUserStore<TUser>> _userStores;

        public UserStore(List<IDragonUserStore<TUser>> userStores)
        {
            _userStores = userStores;
        }

        public Task DeleteAllAsync()
        {
            _userStores.ForEach(x => x.DeleteAllAsync());
            return Task.FromResult<object>(null);
        }

        public Task ClearCache()
        {
            if (!UsesCache()) return Task.FromResult<object>(null);
            var cacheUserStore = _userStores.First();
            var userStore = _userStores[1];
            cacheUserStore.DeleteAllAsync();
            var tasks = new List<Task>();
            foreach (var user in userStore.Users)
            {
                cacheUserStore.CreateAsync(user);
                tasks.AddRange(AsyncRunner.Run(cacheUserStore.GetServicesAsync(user)).Select(serviceId => cacheUserStore.AddServiceToUserAsync(user, serviceId)));
                tasks.AddRange(AsyncRunner.Run(cacheUserStore.GetClaimsAsync(user)).Select(claim => cacheUserStore.AddClaimAsync(user, claim)));
                tasks.AddRange(AsyncRunner.Run(cacheUserStore.GetLoginsAsync(user)).Select(login => cacheUserStore.AddLoginAsync(user, login)));
                tasks.AddRange(AsyncRunner.Run(cacheUserStore.GetRolesAsync(user)).Select(role => cacheUserStore.AddToRoleAsync(user, role)));
            }
            Task.WaitAll(tasks.ToArray());
            return Task.FromResult<object>(null);
        }

        public Task AddServiceToUserAsync(TUser user, string serviceId)
        {
            _userStores.ForEach(x => x.AddServiceToUserAsync(user, serviceId));
            return Task.FromResult<object>(null);
        }

        public Task<bool> IsUserRegisteredForServiceAsync(TUser user, string serviceId)
        {
            return _userStores.First().IsUserRegisteredForServiceAsync(user, serviceId);
        }

        public Task AddAppToUserAsync(TUser user, string appId)
        {
            _userStores.ForEach(x => x.AddAppToUserAsync(user, appId));
            return Task.FromResult<object>(null);
        }

        public Task<bool> IsUserRegisteredForAppAsync(TUser user, string appId)
        {
            return _userStores.First().IsUserRegisteredForAppAsync(user, appId);
        }

        public Task RemoveServiceRegistrations(TUser user)
        {
            _userStores.ForEach(x => x.RemoveServiceRegistrations(user));
            return Task.FromResult<object>(null);
        }

        public Task RemoveAppRegistrations(TUser user)
        {
            _userStores.ForEach(x => x.RemoveAppRegistrations(user));
            return Task.FromResult<object>(null);
        }

        public Task<IEnumerable<string>> GetServicesAsync(TUser user)
        {
            return _userStores.First().GetServicesAsync(user);
        }

        private bool UsesCache()
        {
            return _userStores.Count > 1;
        }

        public void Dispose()
        {
            _userStores.ForEach(x => x.Dispose());
        }

        public Task CreateAsync(TUser user)
        {
            _userStores.ForEach(x => x.CreateAsync(user));
            return Task.FromResult<object>(null);
        }

        public Task UpdateAsync(TUser user)
        {
            _userStores.ForEach(x => x.UpdateAsync(user));
            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TUser user)
        {
            _userStores.ForEach(x => x.DeleteAsync(user));
            return Task.FromResult<object>(null);
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            return _userStores.First().FindByIdAsync(userId);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            return _userStores.First().FindByNameAsync(userName);
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            _userStores.ForEach(x => x.AddLoginAsync(user, login));
            return Task.FromResult<object>(null);
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            _userStores.ForEach(x => x.RemoveLoginAsync(user, login));
            return Task.FromResult<object>(null);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            return _userStores.First().GetLoginsAsync(user);
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            return _userStores.First().FindAsync(login);
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            return _userStores.First().GetClaimsAsync(user);
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            _userStores.ForEach(x => x.AddClaimAsync(user, claim));
            return Task.FromResult<object>(null);
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            _userStores.ForEach(x => x.RemoveClaimAsync(user, claim));
            return Task.FromResult<object>(null);
        }

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            _userStores.ForEach(x => x.AddToRoleAsync(user, roleName));
            return Task.FromResult<object>(null);
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            _userStores.ForEach(x => x.RemoveFromRoleAsync(user, roleName));
            return Task.FromResult<object>(null);
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            return _userStores.First().GetRolesAsync(user);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            return _userStores.First().IsInRoleAsync(user, roleName);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            _userStores.ForEach(x => x.SetPasswordHashAsync(user, passwordHash));
            return Task.FromResult<object>(null);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            return _userStores.First().GetPasswordHashAsync(user);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            return _userStores.First().HasPasswordAsync(user);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            _userStores.ForEach(x => x.SetSecurityStampAsync(user, stamp));
            return Task.FromResult<object>(null);
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            return _userStores.First().GetSecurityStampAsync(user);
        }

        public IQueryable<TUser> Users { get { return _userStores.First().Users; } }

        public Task SetEmailAsync(TUser user, string email)
        {
            _userStores.ForEach(x => x.SetEmailAsync(user, email));
            return Task.FromResult<object>(null);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            return _userStores.First().GetEmailAsync(user);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return _userStores.First().GetEmailConfirmedAsync(user);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            _userStores.ForEach(x => x.SetEmailConfirmedAsync(user, confirmed));
            return Task.FromResult<object>(null);
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            return _userStores.First().FindByEmailAsync(email);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            _userStores.ForEach(x => x.SetTwoFactorEnabledAsync(user, enabled));
            return Task.FromResult<object>(null);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return _userStores.First().GetTwoFactorEnabledAsync(user);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            return _userStores.First().GetLockoutEndDateAsync(user);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            _userStores.ForEach(x => x.SetLockoutEndDateAsync(user, lockoutEnd));
            return Task.FromResult<object>(null);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            var results = _userStores.Select(x => x.IncrementAccessFailedCountAsync(user));
            return results.First();
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            _userStores.ForEach(x => x.ResetAccessFailedCountAsync(user));
            return Task.FromResult<object>(null);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return _userStores.First().GetAccessFailedCountAsync(user);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return _userStores.First().GetLockoutEnabledAsync(user);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            _userStores.ForEach(x => x.SetLockoutEnabledAsync(user, enabled));
            return Task.FromResult<object>(null);
        }
    }
}
