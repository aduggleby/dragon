using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Dragon.SecurityServer.AccountSTS.Services
{
    /// <summary>
    /// Allows limiting available providers based on a HTTP query parameter.
    /// </summary>
    public class QueryParameterProviderLimiterService : IProviderLimiterService
    {
        private string _queryParameterName;
        private readonly Dictionary<string, string> _mapping = new Dictionary<string, string>();

        private const string ProviderRestrictionSessionName = "ProviderRestriction";

        public void Init(NameValueCollection appSettings)
        {
            _queryParameterName = appSettings["ProviderLimitation.QueryParameterName"];
            appSettings["ProviderLimitation.Selectors"].Split(",").Select(x => x.Split("=")).ToList().ForEach(x =>
            {
                if (x.Length != 2)
                {
                    throw new ConfigurationErrorsException("Please use the following format to specify selectors: [param1]=[provider1], ...");
                }
                _mapping[x[0]] = x[1];
            });
        }

        public string Select(Dictionary<string, string> queryParameter)
        {
            var key = queryParameter[_queryParameterName];
            return _mapping.ContainsKey(key) ? _mapping[key] : null;
        }

        public List<string> GetBoundProviders()
        {
            return _mapping.Select(x => x.Value).ToList();
        }

        public bool HasProviderRestriction(HttpContextBase httpContext)
        {
            return QueryContainsProvider(httpContext) || SessionContainsProvider(httpContext);
        }

        public void RegisterProviderRestriction(HttpContextBase httpContext)
        {
            if (!SessionContainsProvider(httpContext))
            {
                httpContext.Session[ProviderRestrictionSessionName] = Select(RouteValuesToDictionary(httpContext)) ?? "";
            }
        }

        public string GetProviderRestriction(HttpContextBase httpContext)
        {
            return httpContext.Session[ProviderRestrictionSessionName] as string;
        }

        public bool DoesAnUnregisteredProviderExist(HttpContextBase httpContext)
        {
            return !SessionContainsProvider(httpContext) && QueryContainsProvider(httpContext);
        }

        private string GetQueryParameterName()
        {
            return _queryParameterName;
        }

        private static Dictionary<string, string> RouteValuesToDictionary(HttpContextBase httpContext)
        {
            var routeValues = new Dictionary<string, string>();
            httpContext.Request.QueryString.AllKeys.ToList()
                .ForEach(x => routeValues.Add(x, httpContext.Request.QueryString[x]));
            return routeValues;
        }

        private static bool SessionContainsProvider(HttpContextBase httpContext)
        {
            return !string.IsNullOrWhiteSpace(httpContext.Session[ProviderRestrictionSessionName] as string);
        }

        private bool QueryContainsProvider(HttpContextBase httpContext)
        {
            return !string.IsNullOrWhiteSpace(httpContext.Request.QueryString[GetQueryParameterName()]);
        }
    }
}
