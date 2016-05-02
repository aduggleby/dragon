using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using Dragon.Security.Hmac.Core.Service;
using Dragon.SecurityServer.Common.Models;

namespace Dragon.SecurityServer.Common
{
    public class HmacHelper : IHmacHelper
    {
        public const string HmacSectionName = "dragon/security/hmac";
        public static string[] ParameterKeys = { "appid", "serviceid", "userid", "expiry", "signature" };

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
