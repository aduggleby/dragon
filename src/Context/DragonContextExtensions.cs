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
    }
}
