using System.Threading.Tasks;
using Dragon.SecurityServer.Identity.Models;

namespace Dragon.SecurityServer.AccountSTS.Services.CheckPasswortServices
{
    public interface ICheckPasswordService<in TUser> where TUser : IdentityUser
    {
        Task<bool> CheckPasswordAsync(TUser user, string password);
    }
}