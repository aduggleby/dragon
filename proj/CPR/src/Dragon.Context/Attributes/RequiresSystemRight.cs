
using System.Linq;
using System;
using System.Diagnostics;
using System.Web.Mvc;
using Dragon.Context.Interfaces;
using StructureMap;

namespace Dragon.Context.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true,
       AllowMultiple = false)]
    public class RequiresSystemRight : RequiresAuthentication
    {
        private const string CONFIG_LOGINURL = "Dragon.Context.LoginUrl";

        private string[] m_systemRights;

        public RequiresSystemRight(params string[] systemRights)
        {
            m_systemRights = systemRights;
        }

        protected override bool AuthorizeCore(System.Web.HttpContextBase httpContext)
        {
            if (base.AuthorizeCore(httpContext))
            {
                var permissionStore = ObjectFactory.GetInstance<IPermissionStore>();
                
                var hasAnyRequiredRights =  m_systemRights.Any(x => permissionStore.HasRight(Guid.Empty /* SYSTEM */, UserID, x));
                Debug.WriteLine(string.Format("User {0} does not have any of the required rights for this page: {1}",
                                              UserID.ToString(),
                                              string.Join(",", m_systemRights)));
                return hasAnyRequiredRights;
            }
            else
            {
                return false;
            }
        }
    }
}
