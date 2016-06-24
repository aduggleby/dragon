using System.Threading.Tasks;
using Dragon.SecurityServer.Identity.Models;

namespace Dragon.SecurityServer.AccountSTS.Services.CheckPasswortServices
{
    public class LegacyCheckPasswordService<TUser> : ICheckPasswordService<TUser> where TUser : IdentityUser
    {
        private const string LegacyPasswordPrefix = "OLD_";

        public virtual Task<bool> CheckPasswordAsync(TUser user, string password)
        {
            var passwordHash = user.PasswordHash.Substring(LegacyPasswordPrefix.Length);
            return Task.FromResult(HashUtil.VerifyHash(password, passwordHash));
        }
    }
}