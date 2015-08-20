using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Dragon.CMS.CMS.Controllers;
using Newtonsoft.Json;

namespace Dragon.CMS.Controllers
{
    public class PageController : Controller
    {
        static Regex s_validUrlPattern = new Regex(@"^[0-9A-Za-z_\-/]*$", RegexOptions.Compiled);
        const string MASTER_NAME = "master";
        private const string CSHTML = ".cshtml";

        public ActionResult Render(string url)
        {
            var x = This.Site.Navigation;

            if (string.IsNullOrWhiteSpace(url))
            {
                return View("default", masterName: MASTER_NAME);
            }
            else
            {
                if (!s_validUrlPattern.IsMatch(url))
                {
                    return new HttpNotFoundResult();
                }

                dynamic model = new object();

                if (ViewEngines.Engines.FindView(this.ControllerContext, url, MASTER_NAME).View == null)
                {
                    // check if we have a json
                    var jsonFile = string.Empty;
                    if (!url.EndsWith("/")) //-  /bob/bah=>/bob/bah.json or /bob/bah/default.json
                    {
                        jsonFile = Server.MapPath("~/pages/" + url + ".json");
                        ViewBag.RootPath = "~/pages/" + url.Substring(0, url.LastIndexOf('/')) + "/";
                    }

                    if (!System.IO.File.Exists(jsonFile))
                    {
                        url = url.TrimEnd('/') + "/";
                        jsonFile = Server.MapPath("~/pages/" + url + "/default.json");
                        ViewBag.RootPath = "~/pages/" + url + "/";
                    }

                    if (!string.IsNullOrWhiteSpace(jsonFile) && System.IO.File.Exists(jsonFile))
                    {
                        model = JsonConvert.DeserializeObject<dynamic>(System.IO.File.ReadAllText(jsonFile));

                        var page = (string)model.page;
                        if (page.StartsWith(".."))
                        {
                            var dummy = new Uri("http://example.org");
                            var it = new Uri(dummy, url.Substring(0, url.LastIndexOf('/')) + "/" + page);
                            url = it.AbsolutePath.TrimStart('/');

                            if (url.ToLower().EndsWith(CSHTML)) url = url.Substring(0, url.Length - CSHTML.Length);
                        }
                        else
                        {
                            url = page;
                        }
                    }
                    else
                    {
                        var mdFile = string.Empty;
                        if (!url.EndsWith("/")) //-  /bob/bah=>/bob/bah.md or /bob/bah/default.md
                        {
                            mdFile = Server.MapPath("~/pages/" + url + ".md");
                            jsonFile = Server.MapPath("~/pages/" + url.Substring(0,url.LastIndexOf('/')) + "/md.json");
                            ViewBag.RootPath = "~/pages/" + url.Substring(0, url.LastIndexOf('/')) + "/";
                        }

                        if (!string.IsNullOrWhiteSpace(mdFile) && System.IO.File.Exists(mdFile))
                        {
                            var pagemodel = JsonConvert.DeserializeObject<dynamic>(System.IO.File.ReadAllText(jsonFile));
                            
                            var md = new MarkdownDeep.Markdown();
                            model = md.Transform(System.IO.File.ReadAllText(mdFile));

                            var page = (string)pagemodel.page;
                            if (page.StartsWith(".."))
                            {
                                var dummy = new Uri("http://example.org");
                                var it = new Uri(dummy, url.Substring(0, url.LastIndexOf('/')) + "/" + page);
                                url = it.AbsolutePath.TrimStart('/');

                                if (url.ToLower().EndsWith(CSHTML)) url = url.Substring(0, url.Length - CSHTML.Length);
                            }
                            else
                            {
                                url = page;
                            }
                        }
                        else
                        {
                            // check if we have a default
                            url = url.TrimEnd('/') + "/default";
                        }
                    }
                }

                // still can't find it
                var view = ViewEngines.Engines.FindView(this.ControllerContext, url, MASTER_NAME);
                if (view.View == null)
                {
                    return new HttpNotFoundResult();
                }

                TempData["CurrentView"] = url;

                ViewBag.Layout = MASTER_NAME;

                return View(url, model);
            }
        }
    }
}