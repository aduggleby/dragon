using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Dragon.Security.Hmac.Core.Service;
using Dragon.SecurityServer.GenericSTSClient;
using Dragon.SecurityServer.GenericSTSClient.Models;

namespace Dragon.SecurityServer.AccountSTS.Client
{
    public class AccountSTSClient : IClient
    {
        private readonly string _apiUrl;
        private readonly string _federationUrl;
        private readonly string _realm;
        private readonly GenericSTSClient.Client _client;

        private const string ClearCacheAction = "ClearCache";
        private const string UpdateAction = "Update";
        private const string ResetPasswordAction = "ResetPassword";
        private const string DeleteAction = "Delete";
        private static readonly HmacHelper HmacHelper = new HmacHelper { HmacService = new HmacSha256Service() };

        public AccountSTSClient(string apiUrl, string federationUrl, string realm)
        {
            _apiUrl = apiUrl;
            _realm = realm;
            _federationUrl = federationUrl;
            _client = new GenericSTSClient.Client(apiUrl);
        }

        public void SetHmacSettings(HmacSettings settings)
        {
            _client.HmacSettings = settings;
        }

        public async Task ClearCache()
        {
            await _client.GetRequest(ClearCacheAction);
        }

        public string GetFederationUrl(string action, string replyUrl)
        {
            var parameters = new Dictionary<string, string>
            {
                { "action", action },
            };
            return CreateSignedUrl(_federationUrl, parameters, replyUrl); ;
        }

        public string GetFederationUrl(string action, string data, string replyUrl)
        {
            var parameters = new Dictionary<string, string>
            {
                { "action", action },
                { "data", data },
            };
            return CreateSignedUrl(_federationUrl, parameters, replyUrl);
        }

        public string GetApiUrl(string action, string replyUrl)
        {
            var parameters = new Dictionary<string, string>
            {
                { "action", action },
            };
            return CreateSignedUrl(_apiUrl, parameters, replyUrl); ;
        }

        public string GetApiUrl(string action, string data, string replyUrl)
        {
            var parameters = new Dictionary<string, string>
            {
                { "action", action },
                { "data", data }
            };
            return CreateSignedUrl(_apiUrl, parameters, replyUrl);
        }

        private string CreateSignedUrl(string url, Dictionary<string, string> parameters, string replyUrl)
        {
            var rst = new SignInRequestMessage(new Uri(url), _realm, replyUrl);
            var encodedReplyUrl = HttpUtility.UrlEncode(replyUrl);
            rst.Context = $"rm=0&id=passive&ru={encodedReplyUrl}";
            rst.CurrentTime = DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK");
            parameters.ToList().ForEach(rst.Parameters.Add);
            var hmacParameters = new Dictionary<string, string>
            {
                {"expiry", DateTime.UtcNow.AddMinutes(+15).Ticks.ToString()},
                {"serviceid", _client.HmacSettings.ServiceId},
                {"appid", _client.HmacSettings.AppId},
                {"userid", _client.HmacSettings.UserId},
            };
            hmacParameters.Add("signature", HmacHelper.CalculateHash(hmacParameters, _client.HmacSettings.Secret));
            hmacParameters.ToList().ForEach(rst.Parameters.Add);
            var requestUrl = rst.RequestUrl;
            return requestUrl;
        }

        public async Task Update<T>(T model)
        {
            await _client.PostRequest(UpdateAction, model);
        }

        public async Task Delete(string userId)
        {
            await _client.PostRequest(DeleteAction, userId);
        }

        public async Task ResetPassword<T>(T model)
        {
            await _client.PostRequest(ResetPasswordAction, model);
        }
    }
}
