using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Dragon.SecurityServer.GenericSTSClient.Models;
using Dragon.SecurityServer.ProfileSTS.Shared.Models;

namespace Dragon.SecurityServer.ProfileSTS.Client
{
    public class ProfileSTSClient : IClient
    {
        private readonly GenericSTSClient.Client _client;
        private const string ClearCacheAction = "ClearCache";
        private const string AddClaimAction = "AddClaim";
        private const string RemoveClaimAction = "RemoveClaim";
        private const string UpdateClaimAction = "UpdateClaim";
        private const string AddOrUpdateClaimsAction = "AddOrUpdateClaims";
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

        public async Task RemoveClaim(string userId, string type)
        {
            await _client.PostRequest(RemoveClaimAction, new RemoveClaimModel { Type = type, UserId = userId });
        }

        public async Task UpdateClaim(string userId, string type, string value)
        {
            await _client.PostRequest(UpdateClaimAction, new UpdateClaimModel { Type = type, Value = value, UserId = userId });
        }

        public async Task AddOrUpdateClaims(string userId, IList<Claim> claims)
        {
            await _client.PostRequest(AddOrUpdateClaimsAction, new AddOrUpdateClaimsModel {Claims = claims, UserId = userId});
        }

        public async Task<IList<Claim>> GetClaims(string userId)
        {
            return await _client.GetRequest<IList<Claim>>(GetClaimsAction, new Dictionary<string, string> {{"userid", userId}});
        }
    }
}
