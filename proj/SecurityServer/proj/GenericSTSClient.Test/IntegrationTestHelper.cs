using Dragon.SecurityServer.Common;
using Dragon.SecurityServer.GenericSTSClient.Models;

namespace Dragon.SecurityServer.GenericSTSClient.Test
{
    public static class IntegrationTestHelper
    {
        public static HmacSettings ReadHmacSettings()
        {
            return HmacHelper.ReadHmacSettings();
        }
    }
}
