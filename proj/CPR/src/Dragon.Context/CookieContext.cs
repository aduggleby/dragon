using System;
using System.Diagnostics;
using Dragon.Context.Helpers;

namespace Dragon.Context
{
    public class CookieContext : IContext
    {
        private const string CookieName = "Dragon.Context.Session.Cookie";

        public Guid CurrentUserID { get; private set; }
        public ICookieHelper CookieHelper { get; set; }

        public void Load()
        {
            CurrentUserID = Guid.Empty;
            try
            {
                var sessionIDString = CookieHelper.Get(CookieName);
                if (sessionIDString == null) return;
                Guid temp;
                if (Guid.TryParse(sessionIDString, out temp))
                {
                    CurrentUserID = temp;
                }
            }
            catch
            {
                // if anything fails fallback to new sessionid
                Debug.WriteLine("Session cookie could not be decrypted!");
                Save(CurrentUserID);
            }
        }

        public void Save(Guid sessionID)
        {
            CookieHelper.Add(CookieName, sessionID.ToString());
        }

        public bool IsAuthenticated()
        {
            return !CurrentUserID.Equals(Guid.Empty);
        }
    }
}
