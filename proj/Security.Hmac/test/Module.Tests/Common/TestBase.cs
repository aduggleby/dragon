using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Dragon.Security.Hmac.Core.Service;

namespace Dragon.Security.Hmac.Module.Tests.Common
{
    public class TestBase
    {
        protected static readonly Guid AppId = Guid.NewGuid();
        protected static readonly Guid ServiceId = Guid.NewGuid();
        protected static readonly Guid UserId = Guid.NewGuid();
        protected const string Secret = "%DF47hf*hdf";
        protected const string DefaultSignatureParameterKey = "signature";

        protected static NameValueCollection CreateValidQueryString(string signatureParameterKey = DefaultSignatureParameterKey, bool useHexEncoding = false)
        {
            return CreateValidQueryString(new Dictionary<string, string>(), signatureParameterKey, useHexEncoding);
        }

        protected static NameValueCollection CreateValidQueryString(Dictionary<string, string> parameters, string signatureParameterKey = DefaultSignatureParameterKey, bool useHexEncoding = false, string secret = Secret)
        {
            var queryString = new NameValueCollection
            {
                { "appid", AppId.ToString() },
                { "serviceid", ServiceId.ToString() },
                { "userid", UserId.ToString() },
                { "expiry", DateTime.UtcNow.AddDays(+1).Ticks.ToString() },
            };
            parameters.ToList().ForEach(x =>
            {
                if (queryString[x.Key] != null) queryString.Remove(x.Key);
                queryString.Add(x.Key, x.Value);
            });
            var hmacService = signatureParameterKey == DefaultSignatureParameterKey ?
                new HmacSha256Service { UseHexEncoding = useHexEncoding } :
                new HmacSha256Service { SignatureParameterKey = signatureParameterKey, UseHexEncoding = useHexEncoding };
            queryString.Add(signatureParameterKey, hmacService.CalculateHash(hmacService.CreateSortedQueryString(queryString), secret));
            return queryString;
        }

        protected static NameValueCollection CreateInvalidQueryString()
        {
            return new NameValueCollection { { "id", "23" } };
        }

        protected static string GetValidRawUrl(bool secret = true)
        {
            var dir = !secret ? "public" : "protected";
            return $"http://localhost/{dir}/index.html";
        }

    }
}
