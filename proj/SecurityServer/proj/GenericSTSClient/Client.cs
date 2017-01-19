using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Dragon.Security.Hmac.Core.Service;
using Dragon.SecurityServer.GenericSTSClient.Models;

namespace Dragon.SecurityServer.GenericSTSClient
{
    public class Client
    {
        public HmacSettings HmacSettings { get; set; }

        private readonly string _serviceUrl;
        private readonly IHmacService _hmacService;

        public Client(string serviceUrl)
        {
            _serviceUrl = serviceUrl;
            _hmacService = new HmacSha256Service();
        }

        public async Task GetRequest(string action)
        {
            using (var client = new HttpClient())
            {
                InitClient(client);
                var result = await client.GetAsync(GenerateUrl(action));
                if (!result.IsSuccessStatusCode)
                {
                    throw new Exception(await result.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task<T> GetRequest<T>(string action, Dictionary<string, string> parameters)
        {
            using (var client = new HttpClient())
            {
                HttpResponseMessage result;
                try
                {
                    InitClient(client);
                    result = await client.GetAsync(GenerateUrl(action, parameters)).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    throw new ApiException(e.Message);
                }
                if (!result.IsSuccessStatusCode)
                {
                    throw new ApiException(await result.Content.ReadAsStringAsync());
                }
                return await result.Content.ReadAsAsync<T>();
            }
        }

        /*
        public string GetManagementUrl(string action, string replyUrl)
        {
            var rst = new SignInRequestMessage(new Uri(_serviceUrl), _realm, replyUrl);
            var encodedReplyUrl = HttpUtility.UrlEncode(replyUrl);
            rst.Context = string.Format("rm=0&id=passive&ru={0}", encodedReplyUrl);
            rst.CurrentTime = DateTime.Now.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ssK");
            rst.Parameters.Add("action", action);
            var requestUrl = rst.RequestUrl;
            return requestUrl;
        }
        */

        public async Task PostRequest<T>(string action, T model)
        {
            using (var client = new HttpClient())
            {
                InitClient(client);
                var response = await client.PostAsJsonAsync(GenerateUrl(action), model).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiException(await response.Content.ReadAsStringAsync());
                }
            }
        }

        public async Task<TU> PostRequest<T, TU>(string action, T model)
        {
            using (var client = new HttpClient())
            {
                InitClient(client);
                var response = await client.PostAsJsonAsync(GenerateUrl(action), model).ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    throw new ApiException(await response.Content.ReadAsStringAsync());
                }
                return await response.Content.ReadAsAsync<TU>();
            }
        }

        #region helpers

        private string GenerateUrl(string action)
        {
            return GenerateUrl(action, new Dictionary<string, string>());
        }

        private string GenerateUrl(string action, Dictionary<string, string> parameters)
        {
            return $"{_serviceUrl}/{action}/{CreateHmacAwareParametersDictionary(parameters)}";
        }

        private static void InitClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Hmac parameter from the HmacSettings can be overridden.
        private string CreateHmacAwareParametersDictionary(Dictionary<string, string> parameters)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            if (HmacSettings != null)
            {
                queryString["appid"] = queryString["appid"] ?? HmacSettings.AppId;
                queryString["serviceid"] = queryString["serviceid"] ?? HmacSettings.ServiceId;
                queryString["userid"] = queryString["userid"] ?? HmacSettings.UserId;
                queryString["expiry"] = queryString["expiry"] ?? DateTime.UtcNow.AddDays(+1).Ticks.ToString();
                parameters.ToList().ForEach(x => queryString[x.Key] = x.Value);
                queryString["signature"] = queryString["signature"] ?? _hmacService.CalculateHash(_hmacService.CreateSortedQueryString(queryString), HmacSettings.Secret);
            }
            else
            {
                parameters.ToList().ForEach(x => queryString[x.Key] = queryString[x.Value]);
            }
            if (queryString.Count < 1) return "";
            return "?" + queryString;
        }

        #endregion
    }
}
