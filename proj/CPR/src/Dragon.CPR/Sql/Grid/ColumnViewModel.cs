using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Dragon.Data.Attributes;

namespace Dragon.CPR.Sql.Grid
{

    public class ColumnViewModel<T> 
    {
        public string PropertyName { get; set; }
        public string DisplayName { get; set; }
        public string Column { get; set; }
        public string NullValue { get; set; }
        public string CellClasses { get; set; }
        public Func<ViewContext, HtmlHelper, T, IHtmlString> HtmlDisplayValue { get; set; }
        public Func<ViewContext, HtmlHelper, T, string> DisplayValue { get; set; }
        public Func<T, string> Tooltip { get; set; }
        public string DisplayValueFormatter { get; set; }
        public bool Visible { get; set; }
        public PropertyInfo PropertyInfo { get; set; }
        public bool Sortable { get; set; }
        public bool Searchable { get; set; }
        public bool IsRowIdentifier { get; set; }

        public ColumnViewModel(string displayName)
        {
            Sortable = true;
            Searchable = false;
            Visible = true;
            DisplayName = DisplayName;
            Tooltip = (o) => null;
        }

        public ColumnViewModel(PropertyInfo pi)
            : this(pi.Name)
        {

            PropertyInfo = pi;
            Tooltip = (o) => null;
            Visible = true;
            DisplayName = pi.Name; // TODO pi.GetDisplayName();

            var keyAtts = pi.GetCustomAttributes(typeof(KeyAttribute), true);
            if (keyAtts != null && keyAtts.Count() > 0)
            {
                IsRowIdentifier = true;
            }

            // If this is a get/set property we can fetch it from the DB
            if (pi.CanRead && pi.CanWrite)
            {
                Column = PropertyInfo.Name;
            }

            PropertyName = PropertyInfo.Name;

            DisplayValue = (vc, html, obj) => GetValue(obj);
            HtmlDisplayValue = (vc, html, o) => new HtmlString(HttpUtility.HtmlEncode(DisplayValue(vc, html, o)));

        }

        public DateTime? GetValueAsDateTime(T obj)
        {
            var outS = string.Empty;
            var v = PropertyInfo.GetValue(obj, null);
            return v as DateTime?;
        }

        public string GetValue(T obj)
        {
            var outS = string.Empty;
            var v = PropertyInfo.GetValue(obj, null);

            if (v == null)
            {
                outS = NullValue;
            }
            else
            {
                if (string.IsNullOrEmpty(DisplayValueFormatter))
                {
                    Type u = Nullable.GetUnderlyingType(PropertyInfo.PropertyType);
                    if ((u != null) && u.IsEnum)
                    {
                        var e = Enum.Parse(u, v.ToString());
                        outS = (e as Enum).GetDisplayName();
                    }
                    else
                    {
                        outS = v.ToString();
                    }
                }
                else
                {
                    outS = string.Format(DisplayValueFormatter, v);
                }
            }

            return outS;
        }
    }
}
