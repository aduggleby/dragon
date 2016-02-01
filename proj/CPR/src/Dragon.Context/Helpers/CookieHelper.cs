using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Dragon.Common.Util;
using Newtonsoft.Json;

namespace Dragon.Context.Helpers
{
    public class CookieHelper : ICookieHelper
    {
        public HttpContext HttpContext { get; set; }

        private readonly bool _sslOnly;
        private static readonly UTF8Encoding Encoding = new UTF8Encoding();

        public CookieHelper(bool sslOnly)
        {
            _sslOnly = sslOnly;
        }

        public void Add(string key, string value)
        {
            var expires = DateTime.UtcNow.AddYears(1);
            var cookie = new HttpCookie(key)
            {
                HttpOnly = false,
                Expires = expires,
                Secure = _sslOnly,
                Value = CryptUtil.Encrypt(value + "|" + expires.ToBinary())
            };
            Debug.WriteLine("Adding cookie '{0}' with value '{1}'", key, cookie.Value);
            HttpContext.Response.Cookies.Add(cookie);
        }

        public string Get(string key)
        {
            var cookie = HttpContext.Request.Cookies[key];
            if (cookie == null)
            {
                return null;
            }

            var data = CryptUtil.Decrypt(cookie.Value).Split('|');
            var expiry = DateTime.FromBinary(long.Parse(data[1]));
            if (DateTime.UtcNow > expiry) return null; // TODO: throw?
            return data[0];
        }
    }
}
