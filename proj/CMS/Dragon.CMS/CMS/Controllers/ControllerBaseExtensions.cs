using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragon.CMS.Controllers
{
    public static class ControllerBaseExtensions
    {
        public static string GetPhysicalViewPath(this ControllerBase controller, string viewName = null)
        {
            if (controller == null)
            {
                throw new ArgumentNullException("controller");
            }

            var context = controller.ControllerContext;

            if (string.IsNullOrEmpty(viewName))
            {
                viewName = context.RouteData.GetRequiredString("action");
            }

            var result = ViewEngines.Engines.FindView(context, viewName, null);
            var compiledView = result.View as BuildManagerCompiledView;

            if (compiledView != null)
            {
                string virtualPath = compiledView.ViewPath;
                return context.HttpContext.Server.MapPath(virtualPath);
            }
            else
            {
                return null;
            }
        }
    }
}