using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Dragon.SecurityServer.Identity.Stores;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using ServiceStack;
using StackExchange.Redis;
using StackRedis.AspNet.Identity;
using IUser = Dragon.SecurityServer.Identity.Models.IUser;

namespace Dragon.SecurityServer.Identity.Redis
{
    public class UserStore<TUser> : IDragonUserStore<TUser> where TUser : class, IUser
    {
        private readonly ConnectionMultiplexer _connectionMultiplexer;
        private readonly RedisUserStore<IdentityUser> _userStore;

        public UserStore(RedisUserStore<IdentityUser> userStore, ConnectionMultiplexer connectionMultiplexer)
        {
            _userStore = userStore;
            _connectionMultiplexer = connectionMultiplexer;
            Mapper.CreateMap<TUser, IdentityUser>();
            Mapper.CreateMap<IdentityUser, TUser>();
        }

        public void Dispose()
        {
            _userStore.Dispose();
        }

        public Task CreateAsync(TUser user)
        {
            return _userStore.CreateAsync(Mapper.Map<IdentityUser>(user));
        }

        public Task UpdateAsync(TUser user)
        {
            return _userStore.UpdateAsync(Mapper.Map<IdentityUser>(user));
        }

        public async Task DeleteAsync(TUser user)
        {
            var identityUser = await _userStore.FindByIdAsync(user.Id);
            await _userStore.DeleteAsync(identityUser);
        }

        public async Task<TUser> FindByIdAsync(string userId)
        {
            return await Task.FromResult(MapToTUser(await _userStore.FindByIdAsync(userId)));
        }

        public async Task<TUser> FindByNameAsync(string userName)
        {
            return await Task.FromResult(MapToTUser(await _userStore.FindByNameAsync(userName)));
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            return _userStore.AddLoginAsync(Mapper.Map<IdentityUser>(user), login);
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            return _userStore.RemoveLoginAsync(Mapper.Map<IdentityUser>(user), login);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            return _userStore.GetLoginsAsync(Mapper.Map<IdentityUser>(user));
        }

        public async Task<TUser> FindAsync(UserLoginInfo login)
        {
            return await Task.FromResult(MapToTUser(await _userStore.FindAsync(login)));
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            return _userStore.GetClaimsAsync(Mapper.Map<IdentityUser>(user));
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            return _userStore.AddClaimAsync(Mapper.Map<IdentityUser>(user), claim);
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            return _userStore.RemoveClaimAsync(Mapper.Map<IdentityUser>(user), claim);
        }

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            return _userStore.AddToRoleAsync(Mapper.Map<IdentityUser>(user), roleName);
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            return _userStore.RemoveFromRoleAsync(Mapper.Map<IdentityUser>(user), roleName);
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            return _userStore.GetRolesAsync(Mapper.Map<IdentityUser>(user));
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            return _userStore.IsInRoleAsync(Mapper.Map<IdentityUser>(user), roleName);
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            return _userStore.SetPasswordHashAsync(Mapper.Map<IdentityUser>(user), passwordHash);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            return _userStore.GetPasswordHashAsync(Mapper.Map<IdentityUser>(user));
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            return _userStore.HasPasswordAsync(Mapper.Map<IdentityUser>(user));
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            return _userStore.SetSecurityStampAsync(Mapper.Map<IdentityUser>(user), stamp);
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            return _userStore.GetSecurityStampAsync(Mapper.Map<IdentityUser>(user));
        }

        public IQueryable<TUser> Users {
            get
            {
                return Database.HashValues(ConfigurationManager.AppSettings["aspNet:identity:redis:userHashByIdKey"])
                        .Select(x => JsonConvert.DeserializeObject<TUser>(x)).AsQueryable();
            }
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            return _userStore.SetEmailAsync(Mapper.Map<IdentityUser>(user), email);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            return _userStore.GetEmailAsync(Mapper.Map<IdentityUser>(user));
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return _userStore.GetEmailConfirmedAsync(Mapper.Map<IdentityUser>(user));
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            return _userStore.SetEmailConfirmedAsync(Mapper.Map<IdentityUser>(user), confirmed);
        }

        public async Task<TUser> FindByEmailAsync(string email)
        {
            return await Task.FromResult(MapToTUser(await _userStore.FindByEmailAsync(email)));
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            return _userStore.SetTwoFactorEnabledAsync(Mapper.Map<IdentityUser>(user), enabled);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return _userStore.GetTwoFactorEnabledAsync(Mapper.Map<IdentityUser>(user));
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            return _userStore.GetLockoutEndDateAsync(Mapper.Map<IdentityUser>(user));
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            return _userStore.SetLockoutEndDateAsync(Mapper.Map<IdentityUser>(user), lockoutEnd);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            return _userStore.IncrementAccessFailedCountAsync(Mapper.Map<IdentityUser>(user));
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            return _userStore.ResetAccessFailedCountAsync(Mapper.Map<IdentityUser>(user));
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return _userStore.GetAccessFailedCountAsync(Mapper.Map<IdentityUser>(user));
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return _userStore.GetLockoutEnabledAsync(Mapper.Map<IdentityUser>(user));
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            return _userStore.SetLockoutEnabledAsync(Mapper.Map<IdentityUser>(user), enabled);
        }

        public async Task DeleteAllAsync()
        {
            var transaction = Database.CreateTransaction();
            var tasks = new List<Task>();
            foreach (var user in Users.ToList())
            {
                foreach (var service in await GetServicesAsync(user).ConfigureAwait(false))
                {
                    tasks.Add(transaction.SetRemoveAsync(string.Format(UserServiceSetKey, user.Id), service));
                    tasks.Add(transaction.HashDeleteAsync(UserServiceHashKey, user.Id));
                }
            }

            if (await transaction.ExecuteAsync().ConfigureAwait(false))
            {
                Task.WaitAll(tasks.ToArray());
            }
            else
            {
                throw new StorageException();
            }
            foreach (var user in Users.ToList())
            {
                await DeleteAsync(user).ConfigureAwait(false); // TODO: use current transaction
            }
        }

        public Task ClearCache()
        {
            // nothing tbd
            return Task.FromResult<object>(null);
        }

        public async Task AddServiceToUserAsync(TUser user, string serviceId)
        {
            Contract.Requires(serviceId != null && !serviceId.IsEmpty());

            var transaction = Database.CreateTransaction();

            var setTask = transaction.SetAddAsync
                (
                    string.Format(UserServiceSetKey, user.Id), serviceId
                );

            var hashTask = transaction.HashSetAsync
                (
                    UserServiceHashKey, new []{new HashEntry(string.Format("{0}:{1}", serviceId, user.Id), true)}
                );

            if (await transaction.ExecuteAsync())
            {
                await Task.WhenAll(setTask, hashTask);
            }
            else
            {
                throw new StorageException();
            }
        }

        public async Task<bool> IsUserRegisteredForServiceAsync(TUser user, string serviceId)
        {
            var isRegistered = (await Database.HashGetAsync(UserServiceHashKey, string.Format("{0}:{1}", serviceId, user.Id)));
            return isRegistered.HasValue && (bool)isRegistered;
        }

        public Task AddAppToUserAsync(TUser user, string serviceId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsUserRegisteredForAppAsync(TUser user, string serviceId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveServiceRegistrations(TUser user)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAppRegistrations(TUser user)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> GetServicesAsync(TUser user)
        {
            return await Task.FromResult((await Database.SetMembersAsync(string.Format(UserServiceSetKey, user.Id))).Select(x => x.ToString()));
        }

        private string UserServiceSetKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:userServiceSetKey"]; } }
        private string UserServiceHashKey { get { return ConfigurationManager.AppSettings["aspNet:identity:redis:userServiceHashKey"]; } }

        #region internal 

        protected virtual IDatabase Database
        {
            get
            {
                return _connectionMultiplexer.GetDatabase(int.Parse(ConfigurationManager.AppSettings["aspNet:identity:redis:db"]));
            }
        }

        private static TUser MapToTUser(IdentityUser identityUser)
        {
            return identityUser == null ? null : Mapper.Map<TUser>(identityUser);
        }

        #endregion
    }
}