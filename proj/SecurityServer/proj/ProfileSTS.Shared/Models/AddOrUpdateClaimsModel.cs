using System.Collections.Generic;
using System.Security.Claims;

namespace Dragon.SecurityServer.ProfileSTS.Shared.Models
{
    public class AddOrUpdateClaimsModel
    {
        public string UserId { get; set; }
        public IList<Claim> Claims { get; set; }
    }
}
