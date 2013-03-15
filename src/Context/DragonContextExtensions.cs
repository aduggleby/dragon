using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dragon.Context
{
    public static class DragonContextExtensions
    {
        public static bool IsAuthenticated(this DragonContext ctx)
        {
            return !ctx.CurrentUserID.Equals(Guid.Empty);
        }

        public static IEnumerable<Guid> GetNodesCurrentUserHasRightsOn(this DragonContext ctx, string spec)
        {
            return DragonContext.PermissionStore.GetNodesWithRight(ctx.CurrentUserID, spec);
        }

        public static void SetProfilePropertyForCurrentUser(this DragonContext ctx, string key, object value)
        {
            DragonContext.ProfileStore.SetProperty(ctx.CurrentUserID, key, value);
        }

        public static T GetProfilePropertyForCurrentUser<T>(this DragonContext ctx, string key, object value)
        {
            return DragonContext.ProfileStore.GetProperty<T>(ctx.CurrentUserID, key);
        }
    }
}
