﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web;
using Dragon.Common.Util;
using Dragon.Interfaces;

namespace Dragon.Context.Sessions
{
    public class CookieSession : ISession
    {
        private const string CONFIG_PREFIX = "Dragon.Context.Session.";
        private const string CONFIG_COOKIENAME = CONFIG_PREFIX + "Cookie";
        private const string CONFIG_SSLONLY = CONFIG_PREFIX + "SSLOnly";
        private const string VAR_XFORWARDEDFOR = "HTTP_X_FORWARDED_FOR";

        private string CookieName { get; set; }

        private HttpContext m_httpCtx;
        
        internal CookieSession()
        {
            CookieName = ConfigUtil.GetAndEnsureValueFromWebConfig(CONFIG_COOKIENAME);

            CheckAndSetHttpContext();
            SetVariablesFromHttpContext();
            LoadFromCookie();
            SaveToCookie();
        }

        ///////////////////////////////////////////////////////////////////////

        public string ForwardedForAddress { get; internal set; }
        public string IPAddress { get; internal set; }
        public Guid ID { get; internal set; }

        public override bool Equals(object obj)
        {
            var cmp = obj as ISession;
            if (cmp == null) return false;
            return
                (IPAddress??string.Empty).Equals((cmp.IPAddress??string.Empty), StringComparison.CurrentCultureIgnoreCase) &&
                (ForwardedForAddress??string.Empty).Equals(cmp.ForwardedForAddress??string.Empty, StringComparison.CurrentCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ForwardedForAddress != null ? ForwardedForAddress.GetHashCode() : 0) * 397) ^ (IPAddress != null ? IPAddress.GetHashCode() : 0);
            }
        }
        
        ///////////////////////////////////////////////////////////////////////

        private bool SSLOnly
        {
            get { return ConfigUtil.IsTrue(CONFIG_SSLONLY); }
        }

        private void CheckAndSetHttpContext()
        {
            m_httpCtx = HttpContext.Current;
            if (m_httpCtx == null)
            {
                throw new Exception("CookieSessionStore must be instantiated during from within an Httpcontext");
            }
        }
        
        private void SaveToCookie()
        {
            // set return cookie
            var cookie = new HttpCookie(CookieName);
            cookie.HttpOnly = false;
            cookie.Expires = DateTime.UtcNow.AddYears(1);
            cookie.Secure = SSLOnly;
            cookie.Value = CryptUtil.Encrypt(ID.ToString());
            m_httpCtx.Response.Cookies.Add(cookie);
        }

        private void LoadFromCookie()
        {
            ID = Guid.NewGuid();
            var cookie = m_httpCtx.Request.Cookies[CookieName];

            // decrypt existing and set sessionid
            if (cookie != null)
            {
                try
                {
                    string sessionIDString = CryptUtil.Decrypt(cookie.Value);
                    Guid temp;
                    if (Guid.TryParse(sessionIDString, out temp))
                    {
                        ID = temp;
                    }
                }
                catch
                {
                    // if anything fails fallback to new sessionid
                    Debug.WriteLine("Session cookie could not be decrypted!");
                    SaveToCookie();
                }
            }
        }

        private void SetVariablesFromHttpContext()
        {
            var serverVars = m_httpCtx.Request.ServerVariables;
            var keyList = new List<string>(serverVars.AllKeys);
            if (keyList.Contains(VAR_XFORWARDEDFOR))
            {
                ForwardedForAddress = serverVars[VAR_XFORWARDEDFOR];
            }

            IPAddress = m_httpCtx.Request.UserHostAddress;
        }
    }
}