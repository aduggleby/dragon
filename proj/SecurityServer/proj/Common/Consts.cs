namespace Dragon.SecurityServer.Common
{
    public class Consts
    {
        public const string ManagementUrlType = "http://whataventure.com/schemas/identity/claims/account/managementUrl";
        public const string ManagementConnectedAccountType = "http://whataventure.com/schemas/identity/claims/account/connectedAccountType";
        public const string ManagementDisconnectedAccountType = "http://whataventure.com/schemas/identity/claims/account/disconnectedAccountType";
        public const string RegisteredServiceType = "http://whataventure.com/schemas/identity/claims/account/registeredService";

        public const string QueryStringParameterNameServiceId = "serviceid";
        public const string QueryStringParameterNameReturnUrl = "ReturnUrl";

        public static string[] QueryStringHmacParameterNames = { "appid", "serviceid", "userid", "expiry", "signature" };
        public const string HmacSectionName = "dragon/security/hmac";

        public const string DefaultClaimNamespace = "http://whataventure.com/schemas/identity/claims/general/";
    }
}
