using Dragon.SecurityServer.Common;
using Dragon.SecurityServer.Common.Models;

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
