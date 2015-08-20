using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Dragon.CMS.CMS.Controllers;

namespace Dragon.CMS.Controllers
{
    public class ModuleController : Controller
    {
       
        public ActionResult Nav()
        {
            var s = this.GetPhysicalViewPath(TempData["CurrentView"] as string);

            return PartialView("navbar", This.Site.Navigation);
        }
    }
}