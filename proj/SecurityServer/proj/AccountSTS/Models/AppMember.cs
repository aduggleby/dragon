using System.Security.Claims;
using System.Threading.Tasks;
using Dragon.SecurityServer.Identity.Models;
using Microsoft.AspNet.Identity;

namespace Dragon.SecurityServer.AccountSTS.Models
{
    public class AppMember : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<AppMember> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom AppMember claims here
            return userIdentity;
        }
    }
}