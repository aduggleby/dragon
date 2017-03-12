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
        public const string DefaultSettingsPrefix = "Dragon.Security.Hmac";

        public string CalculateHash(Dictionary<string, string> parameters, string secret)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            foreach (var parameter in parameters)
            {
                queryString[parameter.Key] = parameter.Value;
            }
            return HmacService.CalculateHash(HmacService.CreateSortedQueryString(queryString), secret);
        }

        public static HmacSettings ReadHmacSettings(string settingsPrefix)
        {
            return ReadHmacSettings(settingsPrefix, null);
        }

        public static HmacSettings ReadHmacSettings(string settingsPrefix, string userId)
        {
            return new HmacSettings
            {
                UserId = string.IsNullOrWhiteSpace(userId) ? ConfigurationManager.AppSettings[settingsPrefix + ".GuestUserId"] : userId,
                AppId = ConfigurationManager.AppSettings[settingsPrefix + ".AppId"],
                ServiceId = ConfigurationManager.AppSettings[settingsPrefix + ".ServiceId"],
                Secret = ConfigurationManager.AppSettings[settingsPrefix + ".Secret"]
            };
        }

        public static HmacSettings ReadHmacSettings()
        {
            return ReadHmacSettings(DefaultSettingsPrefix);
        }

        public Dictionary<string, string> CreateHmacRequestParametersFromConfig(Dictionary<string, string> parameters, string settingsPrefix)
        {
            var hmacSettings = ReadHmacSettings(settingsPrefix);

            var hmacParameters = new Dictionary<string, string>
            {
                { "expiry", DateTime.UtcNow.AddMinutes(+15).Ticks.ToString() },
                { "serviceid", hmacSettings.ServiceId },
                { "appid", hmacSettings.AppId },
                { "userid", hmacSettings.UserId },
            };

            var allParameters = parameters.Where(x => !hmacParameters.Keys.Contains(x.Key)).Concat(hmacParameters).ToDictionary(x => x.Key, x => x.Value);

            allParameters.Add("signature", CalculateHash(allParameters, hmacSettings.Secret));

            return allParameters;
        }

        public Dictionary<string, string> CreateHmacRequestParametersFromConfig(Dictionary<string, string> parameters)
        {
            return CreateHmacRequestParametersFromConfig(parameters, DefaultSettingsPrefix);
        }

        public Dictionary<string, string> CreateHmacRequestParametersFromConfig()
        {
            return CreateHmacRequestParametersFromConfig(new Dictionary<string, string>());
        }

        public Dictionary<string, string> CreateHmacRequestParametersFromConfig(string settingsPrefix)
        {
            return CreateHmacRequestParametersFromConfig(new Dictionary<string, string>(), settingsPrefix);
        }
    }
}
