using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dragon.SecurityServer.Identity.Models;

namespace Dragon.SecurityServer.AccountSTS.Services.CheckPasswortServices
{
    public class ChainedCheckPasswordService<TUser> : ICheckPasswordService<TUser> where TUser : IdentityUser
    {
        private readonly IList<ICheckPasswordService<TUser>> _signInServices;

        public ChainedCheckPasswordService(IList<ICheckPasswordService<TUser>> signInServices)
        {
            _signInServices = signInServices;
        }

        public Task<bool> CheckPasswordAsync(TUser user, string password)
        {
            foreach (var status in _signInServices.Select(service => service.CheckPasswordAsync(user, password)).Where(status => status.Result))
            {
                return status;
            }
            return Task.FromResult(false);
        }
    }
}