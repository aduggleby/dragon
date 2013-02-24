using System;
using System.Collections.Specialized;
using System.Web;

namespace System 
{
    public static class StringHtmlExtensions
    {
        public static IHtmlString AsLink(this string s)
        {
            return new HtmlString(string.Format("<a href=\"{0}\">{0}</a>", s));
        }

        public static IHtmlString WithLink(this string s, string link)
        {
            return new HtmlString(string.Format("<a href=\"{1}\">{0}</a>", s, link));
        }

        
    }
}
