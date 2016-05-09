using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Dragon.Security.Hmac.Core.Service;
using Dragon.SecurityServer.GenericSTSClient.Models;

namespace Dragon.SecurityServer.GenericSTSClient
{
    public class HmacHelper : IHmacHelper
    {

        public IHmacService HmacService { get; set; }

        public string CalculateHash(Dictionary<string, string> parameters, string secret)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            foreach (var parameter in parameters)
            {
                queryString[parameter.Key] = parameter.Value;
            }
            return HmacService.CalculateHash(HmacService.CreateSortedQueryString(queryString), secret);
        }

        public static HmacSettings ReadHmacSettings()
        {
            return new HmacSettings
            {
                UserId = ConfigurationManager.AppSettings["Dragon.Security.Hmac.GuestUserId"],
                AppId = ConfigurationManager.AppSettings["Dragon.Security.Hmac.AppId"],
                ServiceId = ConfigurationManager.AppSettings["Dragon.Security.Hmac.ServiceId"],
                Secret = ConfigurationManager.AppSettings["Dragon.Security.Hmac.Secret"]
            };
        }

        public Dictionary<string, string> CreateHmacRequestParametersFromConfig(Dictionary<string, string> parameters)
        {
            var hmacSettings = ReadHmacSettings();

            var hmacParameters = new Dictionary<string, string>
            {
                { "expiry", DateTime.UtcNow.AddMinutes(+15).Ticks.ToString() },
                { "serviceid", hmacSettings.ServiceId },
                { "appid", hmacSettings.AppId },
                { "userid", hmacSettings.UserId },
            };

            var allParameters = hmacParameters.Concat(parameters).ToDictionary(x => x.Key, x => x.Value);

            allParameters.Add("signature", CalculateHash(allParameters, hmacSettings.Secret));

            return allParameters;
        }

        public Dictionary<string, string> CreateHmacRequestParametersFromConfig()
        {
            var hmacSettings = ReadHmacSettings();

            var parameters = new Dictionary<string, string>
            {
                { "expiry", DateTime.UtcNow.AddMinutes(+15).Ticks.ToString() },
                { "serviceid", hmacSettings.ServiceId },
                { "appid", hmacSettings.AppId },
                { "userid", hmacSettings.UserId },
            };

            parameters.Add("signature", CalculateHash(parameters, hmacSettings.Secret));

            return parameters;
        }
    }
}
