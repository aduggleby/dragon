using System;
using System.Web;

namespace Dragon.CPR.Sql.Grid
{
    public static class ColumnViewModelUtilityExtensions
    {
        public static ColumnViewModel<T> Shorten<T>(this ColumnViewModel<T> col, int max)
        {
            col.Shorten(max, "...");
            return col;
        }

        public static ColumnViewModel<T> ShortenWithFullTooltip<T>(this ColumnViewModel<T> col, int max)
        {
            col.Shorten(max, "...");
            col.WithTooltip((o) => col.GetValue(o));
            return col;
        }


        public static ColumnViewModel<T> Shorten<T>(this ColumnViewModel<T> col, int max, string maxString)
        {
            col.WithDisplayValue(x => col.GetValue(x.Data).Length > max ? col.GetValue(x.Data).Substring(0, max) + maxString : col.GetValue(x.Data));
            return col;
        }

        public static ColumnViewModel<T> WithLink<T>(this ColumnViewModel<T> col, Func<T, string> link)
        {
            col.HtmlDisplayValue = (vc, html, x) => new HtmlString(
                string.IsNullOrWhiteSpace(link(x)) ? HttpUtility.HtmlEncode(col.GetValue(x)) :
                string.Format("<a href=\"{0}\">{1}</a>", link(x), HttpUtility.HtmlEncode(col.GetValue(x))));

            return col;
        }

        public static ColumnViewModel<T> WithLinkInNewWindow<T>(this ColumnViewModel<T> col, Func<T, string> link)
        {
            return WithLinkInNewWindow(col, link, string.Empty);
        }

        public static ColumnViewModel<T> WithLinkInNewWindow<T>(this ColumnViewModel<T> col, Func<T, string> link, string cssclasses)
        {
            return WithLinkInNewWindow(col, link, string.Empty, (x) => string.Empty);
        }

        public static ColumnViewModel<T> WithLinkInNewWindow<T>(this ColumnViewModel<T> col, Func<T, string> link, string cssclasses, Func<T, string> refFunc)
        {
            col.HtmlDisplayValue = (vc, html, x) => new HtmlString(
                string.Format("<a href=\"{0}\" class=\"{2}\" ref=\"{3}\" target=\"_blank\">{1}</a>",
                link(x), HttpUtility.HtmlEncode(col.GetValue(x)), cssclasses, refFunc(x)));

            return col;
        }


        public static ColumnViewModel<T> IsLink<T>(this ColumnViewModel<T> col)
        {
            col.HtmlDisplayValue = (vc, html, x) => new HtmlString(
                string.Format("<a href=\"{0}\">{1}</a>",
                col.GetValue(x), HttpUtility.HtmlEncode(col.GetValue(x))));

            return col;
        }

        public static ColumnViewModel<T> AsLink<T>(this ColumnViewModel<T> col)
        {
            col.HtmlDisplayValue = (vc, html, x) => new HtmlString(
                string.Format("<a href=\"{0}\">{0}</a>",
                col.GetValue(x)));

            return col;
        }

        public static ColumnViewModel<T> AsLink<T>(this ColumnViewModel<T> col, string linkText)
        {
            col.HtmlDisplayValue = (vc, html, x) => new HtmlString(
                string.Format("<a href=\"{0}\">{1}</a>",
                col.GetValue(x), HttpUtility.HtmlEncode(linkText)));

            return col;
        }

        public static ColumnViewModel<T> AsYesNo<T>(this ColumnViewModel<T> col)
        {
            col.WithDisplayValue(x => (col.GetValue(x.Data) ?? string.Empty)
                .Equals(true.ToString(), StringComparison.InvariantCultureIgnoreCase) ? "Yes" : "No");
            return col;
        }

        public static ColumnViewModel<T> AsReversedYesNo<T>(this ColumnViewModel<T> col)
        {
            col.WithDisplayValue(x => (col.GetValue(x.Data) ?? string.Empty)
                .Equals(false.ToString(), StringComparison.InvariantCultureIgnoreCase) ? "Yes" : "No");
            return col;
        }

        public static ColumnViewModel<T> AsDate<T>(this ColumnViewModel<T> col)
        {
            col.WithDisplayValueFormat("{0:dd.MM.yy}");
            return col;
        }

        public static ColumnViewModel<T> AsDateAndShortTime<T>(this ColumnViewModel<T> col)
        {
            col.WithDisplayValueFormat("{0:dd.MM.yy HH:mm}");
            return col;
        }

        //public static ColumnViewModel<T> AsCleverDate<T>(this ColumnViewModel<T> col)
        //{

        //    col.WithDisplayValue(o => col.GetValueAsDateTime(o.Data)
        //        .ToShortDateStringOrToday());

        //    return col;
        //}

        //public static ColumnViewModel<T> AsCleverDateAndShortTime<T>(this ColumnViewModel<T> col)
        //{
        //    col.WithDisplayValue(o => col.GetValueAsDateTime(o.Data)
        //        .ToShortDateTimeStringOrToday());
        //    return col;
        //}

        //public static ColumnViewModel<T> AsCleverDateAndLongTime<T>(this ColumnViewModel<T> col)
        //{
        //    col.WithDisplayValue(o => col.GetValueAsDateTime(o.Data)
        //        .ToLongDateTimeStringOrToday());


        //    return col;
        //}





    }
}
