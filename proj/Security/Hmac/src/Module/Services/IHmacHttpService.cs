using System.Collections.Specialized;

namespace Dragon.Security.Hmac.Module.Services
{
    public enum StatusCode
    {
        Authorized,
        ParameterMissing,
        InvalidParameterFormat,
        InvalidExpiryOrExpired,
        InvalidOrDisabledAppId,
        InvalidOrDisabledServiceId,
        InvalidOrDisabledUserId,
        InvalidSignature
    }

    public interface IHmacHttpService
    {
        StatusCode IsRequestAuthorized(string rawUrl, NameValueCollection queryString);
    }
}
