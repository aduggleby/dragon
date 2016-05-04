using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Dragon.SecurityServer.GenericSTSClient.Models;

namespace Dragon.SecurityServer.ProfileSTS.Client
{
    public class ProfileSTSClient
    {
        private readonly GenericSTSClient.Client _client;
        private const string ClearCacheAction = "ClearCache";
        private const string AddClaimAction = "AddClaim";
        private const string GetClaimsAction = "GetClaims";

        public ProfileSTSClient(string serviceUrl, string realm)
        {
            _client = new GenericSTSClient.Client(serviceUrl);
        }

        public void SetHmacSettings(HmacSettings settings)
        {
            _client.HmacSettings = settings;
        }

        public async Task ClearCache()
        {
            await _client.GetRequest(ClearCacheAction);
        }

        public async Task AddClaim(string userId, string type, string value)
        {
            await _client.PostRequest(AddClaimAction, new AddClaimModel { Type = type, Value = value, UserId = userId });
        }

        public async Task<IList<Claim>> GetClaims(string userId)
        {
            return await _client.GetRequest<IList<Claim>>(GetClaimsAction, new Dictionary<string, string> {{"userid", userId}});
        }
    }
}
