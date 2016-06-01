using System.Threading.Tasks;
using Dragon.SecurityServer.Identity.Models;
using Microsoft.AspNet.Identity;

namespace Dragon.SecurityServer.AccountSTS.Services.CheckPasswortServices
{
    public class OwinCheckPasswordService<TUser> : ICheckPasswordService<TUser> where TUser : IdentityUser
    {
        private readonly UserManager<TUser> _userManager;

        public OwinCheckPasswordService(UserManager<TUser> userManager)
        {
            _userManager = userManager;
        }

        public Task<bool> CheckPasswordAsync(TUser user, string password)
        {
            return _userManager.CheckPasswordAsync(user, password);
        }
    }
}