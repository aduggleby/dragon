using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Dragon.Interfaces;
using StructureMap;

namespace Dragon.Context.Sessions
{
    public class DragonIdentityInjector : ActionFilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            var pre = filterContext.HttpContext.User;

            if (!(pre is DragonPrincipal))
            {
                var ctx = ObjectFactory.GetInstance<DragonContext>();

                if (ctx != null)
                {
                    var permissionStore = ObjectFactory.GetInstance<IPermissionStore>();

                    string[] rights = new string[0];

                    if (permissionStore.HasNode(Guid.Empty))
                    {
                        rights = permissionStore.GetRightsOnNodeWithInherited(Guid.Empty)
                                                    .Where(x => x.SubjectID.Equals(ctx.CurrentUserID))
                                                    .Select(x => x.Spec)
                                                    .ToArray();
                    }

                    filterContext.HttpContext.User = new DragonPrincipal(ctx, rights);
                }
            }
        }
    }
}
