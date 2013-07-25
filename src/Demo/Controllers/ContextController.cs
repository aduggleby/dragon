using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Web;
using System.Web.Mvc;
using Demo.Models;
using Dragon.Common.Objects.Tree;
using Dragon.Context;
using Dragon.Context.Exceptions;
using Dragon.Context.Extensions.Login;
using Dragon.Interfaces;
using StructureMap;

namespace Demo.Controllers
{
    public class ContextController : Controller
    {
        private IPermissionStore m_permissionStore;

        public ContextController(IPermissionStore permissionStore)
        {
            m_permissionStore = permissionStore;
        }

        public DragonContext Ctx { get; set; }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FacebookJS()
        {
            return File(Dragon.Context.Embedded.Files.FacebookJS(), "text/javascript");
        }

        public ActionResult Logout()
        {
            Ctx.Logout();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult LoginUsernameAndPassword(string username, string password)
        {
            if (Ctx.TryLoginWithUsernamePassword(username, password))
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Incorrect username or password.";
            }


            return View("Index");
        }

        [HttpGet]
        public ActionResult RegisterUsernameAndPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterUsernameAndPassword(string username, string password)
        {
            try
            {
                Ctx.RegisterUsernamePassword(username, password);
                return RedirectToAction("Index");
            }
            catch (UserKeyAlreadyExistsForThisServiceException ex)
            {
                TempData["Error"] = "Username already exists.";
            }
            catch (ServiceAlreadyConnectedToUserException ex)
            {
                TempData["Error"] = "You are already connected to this authentication service.";
            }
            return View();

        }

        [HttpPost]
        public ActionResult LoginFacebook(string key, string secret)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(secret))
            {
                return Content("alert#Invalid facebook login request.");
            }

            Guid currentUserID = Ctx.CurrentUserID;

            try
            {
                if (Ctx.ConnectWithFacebook(key, secret))
                {
                    if (!Ctx.CurrentUserID.Equals(currentUserID))
                    {
                        return Content("redirect#" + Url.Action("Index"));
                    }

                    return Content("nop");
                }
                else
                {
                    return Content("alert#There was a a problem with your facebook connection.");
                }
            }
            catch (Exception ex)
            {
                return Content("alert#" + ex.Message);
            }

        }

        public ActionResult Rights()
        {
            return View(m_permissionStore.Tree);
        }

        public ActionResult AddRights()
        {
            var s = Guid.NewGuid();

            var n1 = Guid.NewGuid();
            var n1_1 = Guid.NewGuid();
            var n1_2 = Guid.NewGuid();
            var n1_2_1 = Guid.NewGuid();
            var n1_2_2 = Guid.NewGuid();

            m_permissionStore.AddNode(n1, n1_1);
            m_permissionStore.AddNode(n1, n1_2);
            m_permissionStore.AddNode(n1_2, n1_2_1);
            m_permissionStore.AddNode(n1_2, n1_2_2);

            m_permissionStore.AddRight(n1, s, "read", true);
            m_permissionStore.AddRight(n1_2, s, "write", false);
            m_permissionStore.AddRight(n1_1, s, "write", true);

            return RedirectToAction("Rights");
        }
    }
}


