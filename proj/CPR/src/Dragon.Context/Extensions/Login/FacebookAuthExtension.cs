using System;
using Dragon.Context.Exceptions;
using Facebook;

namespace Dragon.Context.Extensions.Login
{
    public static class FacebookAuthExtension
    {
        private const string SERVICE_ID = "FACEBOOK";

     

        public static bool ConnectWithFacebook(
            this DragonContext ctx, 
            string key, 
            string secret)
        {
            // check with facebook if these credentials are valid
            var client = new FacebookClient(secret);
            dynamic me = client.Get("me");

            if (me.id != key) return false;

            if (ctx.CurrentUserID == Guid.Empty)
            {
                Guid? userID;

                if (ctx.UserStore.HasUserByKey(SERVICE_ID, key, out userID))
                {
                    ctx.UserStore.TryLogin(SERVICE_ID, key, (s) => true);
                    ctx.UserStore.UpdateSecret(SERVICE_ID, key, secret);
                }
                else
                {
                    ctx.UserStore.Register(SERVICE_ID, key, secret);
                }
                return true;
            }
            else
            {
                Guid? userID;
                // user is logged-in, first check if this is registered 
                if (ctx.UserStore.HasUserByKey(SERVICE_ID, key, out userID))
                {
                    if (userID.HasValue && userID.Value != ctx.CurrentUserID)
                    {
                        throw new UserKeyAlreadyExistsForThisServiceException();
                    }
                    else
                    {
                        // auth is already attached to this user
                        return (ctx.UserStore.TryLogin(SERVICE_ID, key, (s) => s.Equals(secret)));
                    }
                }
                else
                {
                    // attach auth to this user
                    ctx.UserStore.Register(SERVICE_ID, key, secret);
                    return true;
                }
            }
        }
    }
}
