using System;
using System.Diagnostics;
using Dragon.Context.Helpers;

namespace Dragon.Context
{
    /// <summary>
    /// Use the CookieContextAuthorizationFilter to autoload the context.
    /// </summary>
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
                var userIDStr = CookieHelper.Get(CookieName);
                if (userIDStr == null)
                {
                    Debug.WriteLine("Unable to read cookie!");
                    Save(CurrentUserID);
                    return;
                }
                Guid temp;
                if (Guid.TryParse(userIDStr, out temp))
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

        public void Save(Guid userID)
        {
            CookieHelper.Add(CookieName, userID.ToString());
            CurrentUserID = userID;
        }

        public bool IsAuthenticated()
        {
            return !CurrentUserID.Equals(Guid.Empty);
        }
    }
}
