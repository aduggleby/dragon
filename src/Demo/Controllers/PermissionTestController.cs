using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Dragon.Context.Permissions;
using Dragon.Interfaces;
using Dragon.Interfaces.PermissionInfo;

namespace Demo.Controllers
{
    public class PermissionTestController : Controller
    {
        private readonly IPermissionInfoExtractor _permissionInfoExtractor;

        public PermissionTestController(IPermissionStore permissionStore)
        {
            _permissionInfoExtractor = new PermissionInfoExtractor(permissionStore, new DefaultNameResolver("user_"));
        }

        public ActionResult Index()
        {
            return View(new List<PermissionInfo>());
        }

        [HttpPost]
        public ActionResult Index(String permissionID)
        {
            return View(_permissionInfoExtractor.GetPermissionInfo(new Guid(permissionID)));
        }
    }
}


