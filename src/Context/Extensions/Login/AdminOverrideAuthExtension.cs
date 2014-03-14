using System;
using Dragon.Context.Exceptions;
using Facebook;

namespace Dragon.Context.Extensions.Login
{
    public static class AdminOverrideAuthExtension
    {
        public static bool Impersonate(
            this DragonContext ctx, 
            Guid userID)
        {
            if (ctx.UserStore.Impersonate(userID)) return true;
            return false;
        }
    }
}
