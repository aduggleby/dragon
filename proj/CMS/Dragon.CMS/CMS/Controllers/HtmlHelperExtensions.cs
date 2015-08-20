using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Dragon.CMS
{
    public static class HtmlHelperExtensions
    {
        public static MvcHtmlString Page(this HtmlHelper html, string url)
        {
            return html.Action("Render", "Page", new { url });
        }
    }
}