using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Dragon.SecurityServer.Common.Models;

namespace Dragon.SecurityServer.PermissionSTS.Client
{
    public class PermissionSTSClient
    {
        private readonly GenericSTSClient.Client _client;
        private const string ClearCacheAction = "ClearCache";
        private const string AddPermissionAction = "AddPermission";
        private const string GetClaimsAction = "GetClaims";

        public PermissionSTSClient(string serviceUrl)
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

        public async Task AddPermission(string userId, string operation, string obj)
        {
            await _client.PostRequest(AddPermissionAction, new AddPermissionModel { Operation = operation, Object = obj, UserId = userId });
        }

        public async Task<IList<Claim>> GetClaims(string userId)
        {
            return await _client.GetRequest<IList<Claim>>(GetClaimsAction, new Dictionary<string, string> {{"userid", userId}});
        }
    }
}
