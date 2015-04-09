using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Dragon.Security.Hmac.Core.Service
{
    public class HmacSha256Service : IHmacService
    {
        private static readonly UTF8Encoding Encoding = new UTF8Encoding();
        private static readonly Regex ReplaceSpecialCharactersRegex = new Regex(@"[/=\+]"); // needed to avoid mismatches caused by url encoding

        public string CalculateHash(string data, string secret)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(secret))
            {
                throw new HmacInvalidArgumentException("Please provide valid (neither null nor empty) data and secret.");
            }
            var hmac = new HMACSHA256 { Key = Encoding.GetBytes(secret) };
            var hmacSig = hmac.ComputeHash(Encoding.GetBytes(data.ToCharArray()));
            return ReplaceSpecialCharactersRegex.Replace(Convert.ToBase64String(hmacSig), "-");
        }

        public string CreateSortedQueryValuesString(NameValueCollection queryString)
        {
            if (queryString == null || queryString.Count < 1)
            {
                throw new HmacInvalidArgumentException("Please provide a valid queryString that contains some elements.");
            }
            var ignoreKeys = new[] { "signature" /* used by the Hmac Module */, "_" /* used by jQuery to avoid caching */ };
            return queryString.Keys.Cast<string>().Except(ignoreKeys).OrderBy(x => x).Select(x => queryString[x]).Aggregate((a, b) => a + b);
        }
    }
}
