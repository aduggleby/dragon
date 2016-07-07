using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Dragon.SecurityServer.GenericSTSClient.Models;

namespace Dragon.SecurityServer.ProfileSTS.Client
{
    public interface IClient
    {
        void SetHmacSettings(HmacSettings settings);
        Task ClearCache();
        Task AddClaim(string userId, string type, string value);
        Task<IList<Claim>> GetClaims(string userId);
    }
}