using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace Dragon.SecurityServer.AccountSTS.Services
{
    /// <summary>
    /// Allows limiting the available providers per request.
    /// </summary>
    public interface IProviderLimiterService
    {
        void Init(NameValueCollection appSettings);
        bool IsEnabled();
        string Select(Dictionary<string, string> input);
        List<string> GetBoundProviders();
        bool HasProviderRestriction(HttpContextBase httpContext);
        bool DoesAnUnregisteredProviderExist(HttpContextBase httpContext);
        void RegisterProviderRestriction(HttpContextBase httpContext);
        string GetProviderRestriction(HttpContextBase httpContext);
    }
}