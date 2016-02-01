using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;

namespace Dragon.Context.Sessions
{
    public class DragonIdentity : GenericIdentity
    {
        public DragonIdentity(IContext context):base(context.CurrentUserID.ToString(), "Dragon")
        {
        }
    }
}
