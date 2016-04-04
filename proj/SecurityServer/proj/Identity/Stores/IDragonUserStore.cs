using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using IUser = Dragon.SecurityServer.Identity.Models.IUser;

namespace Dragon.SecurityServer.Identity.Stores
{
    public interface IDragonUserStore<TUser> :
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserEmailStore<TUser>,
        //IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser, string>,
        IUserLockoutStore<TUser, string>,
        IUserStore<TUser>
        where TUser : class, IUser
    {
        Task DeleteAllAsync();
        Task ClearCache();
        Task AddServiceToUserAsync(TUser user, string serviceId);
        Task<bool> IsUserRegisteredForServiceAsync(TUser user, string serviceId);
        Task<IEnumerable<string>> GetServicesAsync(TUser user);
    }
}
