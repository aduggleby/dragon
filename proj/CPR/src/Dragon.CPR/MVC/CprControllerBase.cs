using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Dragon.Data.Interfaces;

namespace Dragon.CPR.MVC
{
    public abstract class CPRControllerBase : 
        ControllerBase
    {
        public CPRControllerBase()
        {

            ViewBag.Title = string.Empty; // Page Title
            ViewBag.IsEditing = false;
        }


        public virtual ActionResult CommandView(object model)
        {
            return View("Command", model);
        }

        public ActionResult Forbidden()
        {
            return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
        }

        public ActionResult NotFound()
        {
            return new HttpStatusCodeResult(HttpStatusCode.NotFound);
        }

        public ActionResult JsonNet(object d)
        {
            return new JsonNetResult()
                {
                    Data = d
                };
        }

        public ActionResult RedirectTo(string action, string controller)
        {
            return RedirectToAction(action, controller);
        }

        public void AddCustomLessCss()
        {
            var currentController = this.ControllerContext.RouteData.Values["controller"] as string ?? "Home";
            var currentAction = this.ControllerContext.RouteData.Values["action"] as string ?? "Index";

            ViewBag.CSS_CUSTOM = Url.Content(string.Format("{0}-{1}.less", currentController, currentAction).ToLower());
        }

        public void AddCustomJs()
        {
            var currentController = this.ControllerContext.RouteData.Values["controller"] as string ?? "Home";
            var currentAction = this.ControllerContext.RouteData.Values["action"] as string ?? "Index";

            ViewBag.JS_CUSTOM = Url.Content(string.Format("{0}-{1}.js", currentController, currentAction).ToLower());
        }

        public void AddCustomControllerLessCss()
        {
            var currentController = this.ControllerContext.RouteData.Values["controller"] as string ?? "Home";

            ViewBag.CSS_CUSTOM_CONTROLLER = Url.Content(string.Format("{0}.less", currentController).ToLower());
        }

        public void AddCustomControllerJs()
        {
            var currentController = this.ControllerContext.RouteData.Values["controller"] as string ?? "Home";

            ViewBag.JS_CUSTOM_CONTROLLER = Url.Content(string.Format("{0}.js", currentController).ToLower());
        }

    }
}
