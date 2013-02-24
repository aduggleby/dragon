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
    }
}
