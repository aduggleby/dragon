using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dragon.Data.Interfaces;
using Dragon.SecurityServer.AccountSTS.Models;
using Dragon.SecurityServer.Identity.Models;

namespace Dragon.SecurityServer.AccountSTS.Services.CheckPasswortServices
{
    public class LegacyCheckPasswordService<TUser> : ICheckPasswordService<TUser> where TUser : IdentityUser
    {
        private readonly IRepository<LegacyRegistrationModel> _wavRegistrationRepository; 

        public LegacyCheckPasswordService(IRepository<LegacyRegistrationModel> wavRegistrationRepository)
        {
            _wavRegistrationRepository = wavRegistrationRepository;
        }

        public virtual Task<bool> CheckPasswordAsync(TUser user, string password)
        {
            var registrationModels = _wavRegistrationRepository.GetByWhere(new Dictionary<string, object>() {{"Key", user.Email}}).AsList();
            if (!registrationModels.Any()) return Task.FromResult(false);
            var passwordHash = registrationModels.First().Secret;
            return Task.FromResult(HashUtil.VerifyHash(password, passwordHash));
        }
    }
}