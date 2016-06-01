using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Dragon.Data.Interfaces;
using Dragon.SecurityServer.Identity.Models;
using Microsoft.AspNet.Identity;
using IUser = Dragon.SecurityServer.Identity.Models.IUser;

namespace Dragon.SecurityServer.Identity.Stores
{
    /// <summary>
    /// See <see href="http://www.asp.net/identity/overview/extensibility/overview-of-custom-storage-providers-for-aspnet-identity">asp.net</see>
    /// </summary>
    public class UserStore<TUser> : IDragonUserStore<TUser> where TUser : class, IUser
    {
        private IRepository<TUser> UserRepository { get; set; }
        private IRepository<IdentityRole> RoleRepository { get; set; }
        private IRepository<IdentityUserRole> UserRoleRepository { get; set; }
        private IRepository<IdentityUserClaim> UserClaimRepository { get; set; }
        private IRepository<IdentityUserLogin> UserLoginRepository { get; set; }
        private IRepository<IdentityService> UserServiceRepository { get; set; }

        public UserStore(IRepository<TUser> userRepository, IRepository<IdentityUserClaim> userClaimRepository, IRepository<IdentityUserLogin> userLoginRepository, IRepository<IdentityService> userServiceRepository)
        {
            UserRepository = userRepository;
            UserLoginRepository = userLoginRepository;
            UserServiceRepository = userServiceRepository;
            UserClaimRepository = userClaimRepository;
        }

        public UserStore(IRepository<TUser> userRepository, IRepository<IdentityRole> roleRepository,
            IRepository<IdentityUserRole> userRoleRepository, IRepository<IdentityUserClaim> userClaimRepository, IRepository<IdentityUserLogin> userLoginRepository, IRepository<IdentityService> userServiceRepository)
        {
            UserRepository = userRepository;
            RoleRepository = roleRepository;
            UserRoleRepository = userRoleRepository;
            UserClaimRepository = userClaimRepository;
            UserLoginRepository = userLoginRepository;
            UserServiceRepository = userServiceRepository;
        }

        public void Dispose()
        {
            // nothing to be done
        }

        public Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            UserRepository.Insert(user);

            return Task.FromResult<object>(null);
        }

        public Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            UserRepository.Update(user);

            return Task.FromResult<object>(null);
        }

        public Task DeleteAsync(TUser user)
        {
            if (user != null)
            {
                UserRepository.Delete(user);
            }

            return Task.FromResult<object>(null);
        }

        public Task DeleteAllAsync()
        {
            var users = UserRepository.GetAll();
            foreach (var user in users)
            {
                UserRepository.Delete(user);
            }
            return Task.FromResult<object>(null);
        }

        public Task ClearCache()
        {
            return Task.FromResult<object>(null);
        }

        public Task<TUser> FindByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("Null or empty argument: userId");
            }

            var result = UserRepository.Get(userId);
            return result != null ? Task.FromResult(result) : Task.FromResult<TUser>(null);
        }

        public Task<TUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("Null or empty argument: userName");
            }

            var result = UserRepository.GetByWhere(new Dictionary<string, object> { { "UserName", userName } }) as List<TUser>;

            // Should I throw if > 1 user?
            if (result != null && result.Count == 1)
            {
                return Task.FromResult(result[0]);
            }

            return Task.FromResult<TUser>(null);
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            UserLoginRepository.Insert(new IdentityUserLogin {UserId = user.Id, LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey});

            return Task.FromResult<object>(null);
        }

        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            UserLoginRepository.Delete(new IdentityUserLogin{LoginProvider = login.LoginProvider, ProviderKey = login.ProviderKey, UserId = user.Id});

            return Task.FromResult<Object>(null);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var logins = UserLoginRepository.GetByWhere(new Dictionary<string, object>{{"UserId", user.Id}})
                .Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey)).ToList();
            return Task.FromResult<IList<UserLoginInfo>>(logins);
        }

        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            var identityUserLogins = UserLoginRepository.GetByWhere(new Dictionary<string, object>{
                {"ProviderKey", login.ProviderKey},
                {"LoginProvider", login.LoginProvider}
            }).ToList();

            return Task.FromResult(!identityUserLogins.Any() ? null : UserRepository.Get(identityUserLogins.First().UserId));
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            if (UserClaimRepository == null) return Task.FromResult<IList<Claim>>(new List<Claim>());
            var claims = UserClaimRepository.GetByWhere(new Dictionary<string, object> { { "UserId", user.Id } }).Select(x => new Claim(x.ClaimType, x.ClaimValue));
            return Task.FromResult<IList<Claim>>(claims.ToList());
        }

        public Task AddClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            UserClaimRepository.Insert(new IdentityUserClaim { UserId = user.Id, ClaimType = claim.Type, ClaimValue = claim.Value });

            return Task.FromResult<object>(null);
        }

        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (claim == null)
            {
                throw new ArgumentNullException("claim");
            }

            UserClaimRepository.Delete(UserClaimRepository.GetByWhere(new Dictionary<string, object>{
                {"UserId", user.Id},
                {"ClaimType", claim.Type},
                {"ClaimValue", claim.Value}
            }).First());

            return Task.FromResult<object>(null);
        }

        public Task AddToRoleAsync(TUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException("Argument cannot be null or empty: roleName.");
            }

            var role = RoleRepository.GetByWhere(new Dictionary<string, object> { { "Name", roleName } });
            var roleId = role == null ? null : role.First().Id;
            if (!string.IsNullOrEmpty(roleId))
            {
                UserRoleRepository.Insert(new IdentityUserRole { UserId = user.Id, RoleId = roleId });
            }

            return Task.FromResult<object>(null);
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            return Task.FromResult<IList<string>>(FindRolesByUser(user));
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentNullException("roleName");
            }

            return Task.FromResult(FindRolesByUser(user).Contains(roleName));
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;

            return Task.FromResult<object>(null);
        }

        public Task<string> GetPasswordHashAsync(TUser user)
        {
            var passwordHash = UserRepository.Get(user.Id).PasswordHash;

            return Task.FromResult(passwordHash);
        }

        public Task<bool> HasPasswordAsync(TUser user)
        {
            var hasPassword = !string.IsNullOrEmpty(UserRepository.Get(user.Id).PasswordHash);

            return Task.FromResult(bool.Parse(hasPassword.ToString()));
        }

        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            user.SecurityStamp = stamp;
            UserRepository.Update(user);

            return Task.FromResult(0);
        }

        public Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }

        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public IQueryable<TUser> Users
        {
            get { return UserRepository.GetAll().AsQueryable(); }
        }

        public Task SetEmailAsync(TUser user, string email)
        {
            user.Email = email;
            UserRepository.Update(user);

            return Task.FromResult(0);
        }

        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            UserRepository.Update(user);

            return Task.FromResult(0);
        }

        public Task<TUser> FindByEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException("email");
            }

            var result = UserRepository.GetByWhere(new Dictionary<string, object> { { "Email", email } }).ToList();
            return result.Any() ? Task.FromResult(result.First()) : Task.FromResult<TUser>(null);
        }

        //public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        //{
        //    user.PhoneNumber = phoneNumber;
        //    UserRepository.Update(user);

        //    return Task.FromResult(0);
        //}

        //public Task<string> GetPhoneNumberAsync(TUser user)
        //{
        //    return Task.FromResult(user.PhoneNumber);
        //}

        //public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        //{
        //    return Task.FromResult(user.PhoneNumberConfirmed);
        //}

        //public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        //{
        //    user.PhoneNumberConfirmed = confirmed;
        //    UserRepository.Update(user);

        //    return Task.FromResult(0);
        //}

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            UserRepository.Update(user);

            return Task.FromResult(0);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }

        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                 ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                 : new DateTimeOffset());
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            UserRepository.Update(user);

            return Task.FromResult(0);
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount++;
            UserRepository.Update(user);

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task ResetAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount = 0;
            UserRepository.Update(user);

            return Task.FromResult(0);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            UserRepository.Update(user);

            return Task.FromResult(0);
        }

        public Task AddServiceToUserAsync(TUser user, string serviceId)
        {
            if (IsUserRegisteredForService(user, serviceId))
            {
                throw new ArgumentException(string.Format("Service {0} already has been added to user {1}", serviceId, user.Id));
            }
            UserServiceRepository.Insert(new IdentityService {ServiceId = serviceId, UserId = user.Id});
            return Task.FromResult(0); 
        }

        public Task<bool> IsUserRegisteredForServiceAsync(TUser user, string serviceId)
        {
            return Task.FromResult(IsUserRegisteredForService(user, serviceId));
        }
        
        public Task<IEnumerable<string>> GetServicesAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }

            var services = UserServiceRepository.GetByWhere(new Dictionary<string, object>
            {
                {
                    "UserId", user.Id
                }
            }).Select(x => x.ServiceId).ToList();

            return Task.FromResult<IEnumerable<string>>(services);
        }

        #region helper

        public bool IsUserRegisteredForService(TUser user, string serviceId)
        {
            var userList = UserServiceRepository.GetByWhere(new Dictionary<string, object>
            {
                {"UserId", user.Id},
                {"ServiceId", serviceId}
            });
            return userList.Any();
        }

        private List<string> FindRolesByUser(TUser user)
        {
            if (RoleRepository == null) return new List<string>();
            return UserRoleRepository.GetByWhere(
                new Dictionary<string, object> { { "UserId", user.Id } }).Select(x =>
                    RoleRepository.Get(x.RoleId)).Select(x => x.Name).ToList();
        }

        #endregion
    }
}
