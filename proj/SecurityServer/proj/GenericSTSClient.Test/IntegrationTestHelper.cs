using System.Configuration;

namespace Dragon.SecurityServer.GenericSTSClient.Test
{
    public static class IntegrationTestHelper
    {
        public static HmacSettings ReadHmacSettings()
        {
            return new HmacSettings
            {
                UserId = ConfigurationManager.AppSettings["Dragon.Security.Hmac.UserId"],
                AppId = ConfigurationManager.AppSettings["Dragon.Security.Hmac.AppId"],
                ServiceId = ConfigurationManager.AppSettings["Dragon.Security.Hmac.ServiceId"],
                Secret = ConfigurationManager.AppSettings["Dragon.Security.Hmac.Secret"]
            };
        }
    }
}
