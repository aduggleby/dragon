using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Dragon.CMS.Controllers
{
    public class CustomViewLocationRazorViewEngine: RazorViewEngine
{
        public CustomViewLocationRazorViewEngine()
    {
        ViewLocationFormats = new[] 
        {
            "~/pages/{0}.cshtml",
        };

        MasterLocationFormats = new[] 
        {
            "~/layouts/{0}.cshtml"
        };

        PartialViewLocationFormats = new[] 
        {
           "~/modules/{0}.cshtml", 
        };
    }
}
}