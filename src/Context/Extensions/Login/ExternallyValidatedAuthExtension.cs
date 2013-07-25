using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Dragon.Common.Util;
using Dragon.Context.Exceptions;

namespace Dragon.Context.Extensions.Login
{
    public static class ExternallyValidatedAuthExtension
    {
        private const string SERVICE_ID = "EXTERNAL";

        private static string PrefixedService(string service)
        {
            return SERVICE_ID + "_" + service;
        }

        public static void EstablishExternallyValidatedAccount(this DragonContext ctx, string service, string identifier)
        {
            if (!ctx.UserStore.HasUserByKey(service, identifier))
            {
                // create account
                ctx.UserStore.Register(PrefixedService(service), identifier, string.Empty);
            }
        }

        public static bool ExistsAccountForExternallyValidatedAccount(
          this DragonContext ctx, string service, string identifier)
        {
            var success = ctx.UserStore.TryLogin(PrefixedService(service), identifier, (s) => true);
            if (success)
            {
                DragonContext.ProfileStore.SetProperty(ctx.CurrentUserID,
                                                               DragonContext.PROFILEKEY_SERVICE,
                                                               SERVICE_ID);
            }
            return success;
        }


        public static bool IsExternallyValidatedUser(this DragonContext ctx)
        {
            return DragonContext.ProfileStore.GetProperty<string>(ctx.CurrentUserID, DragonContext.PROFILEKEY_SERVICE) == SERVICE_ID;
        }
    }
}
