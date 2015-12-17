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
        public string SignatureParameterKey { get; set; }
        public bool UseHexEncoding { get; set; }
        private static readonly UTF8Encoding Encoding = new UTF8Encoding();
        private static readonly Regex ReplaceSpecialCharactersRegex = new Regex(@"[/=\+]"); // needed to avoid mismatches caused by url encoding

        public HmacSha256Service()
        {
            SignatureParameterKey = "signature";
            UseHexEncoding = false;
        }

        public string CalculateHash(string data, string secret)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(secret))
            {
                throw new HmacInvalidArgumentException("Please provide valid (neither null nor empty) data and secret.");
            }
            var hmac = new HMACSHA256 { Key = Encoding.GetBytes(secret) };
            var hmacSig = hmac.ComputeHash(Encoding.GetBytes(data.ToCharArray()));
            return ReplaceSpecialCharactersRegex.Replace(UseHexEncoding ? ToHex(hmacSig) : ToBase64(hmacSig), "-");
        }

        private static string ToBase64(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        private static string ToHex(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "");
        }

        public string CreateSortedQueryString(NameValueCollection queryString)
        {
            if (queryString == null || queryString.Count < 1)
            {
                throw new HmacInvalidArgumentException("Please provide a valid queryString that contains some elements.");
            }
            var ignoreKeys = new[] { SignatureParameterKey /* used by the Hmac Module */, "_" /* used by jQuery to avoid caching */ };
            return queryString.Keys.Cast<string>().Except(ignoreKeys).OrderBy(x => x).Select(x => x + "=" + queryString[x]).Aggregate((a, b) => a + "&" + b);
        }
    }
}
