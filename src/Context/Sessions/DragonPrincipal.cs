using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using Dragon.Interfaces;

namespace Dragon.Context.Sessions
{
    public class DragonPrincipal : GenericPrincipal
    {
        public DragonPrincipal(DragonContext ctx, string[] roles):base(new DragonIdentity(ctx), roles)
        {
            
        }
    }
}
