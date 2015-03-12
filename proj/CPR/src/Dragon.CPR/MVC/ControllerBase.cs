using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web.Mvc;
using Dragon.Data.Interfaces;

namespace Dragon.CPR.MVC
{
    public abstract class ControllerBase : Controller
    {
        public const string VIEW_LINKS = "VIEW_LINKS";

        private void ShowInfoException(Exception ex)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("DEBUG OUTPUT\r\n\r\n");

            Exception cur = ex;

            while (cur != null)
            {
                sb.AppendLine(cur.ToString());
                sb.AppendLine();
                sb.AppendLine("-------------------------------------------------------------------------");
                sb.AppendLine();
                cur = cur.InnerException;
            }

            ShowInfo(sb.ToString());
        }

        protected void ShowWarning(string msg)
        {
            TempData["Message.Warning"] = msg;
        }

        protected void ShowInfo(string msg)
        {
            TempData["Message.Info"] = msg;
        }

        protected void ShowError(string msg)
        {
            TempData["Message.Error"] = msg;
        }

        protected void ShowSuccess(string msg)
        {
            TempData["Message.Success"] = msg;
        }

        protected void AddViewLink(string url, string title)
        {
            if (ViewData[VIEW_LINKS] == null)
            {
                ViewData[VIEW_LINKS] = new List<ViewLink>();
            }

            var list = ViewData[VIEW_LINKS] as List<ViewLink>;
            list.Add(new ViewLink() { Url = url, Title = title });
        }
    }


    public class ViewLink
    {
        public string Url { get; set; }
        public string Title { get; set; }
    }
}
