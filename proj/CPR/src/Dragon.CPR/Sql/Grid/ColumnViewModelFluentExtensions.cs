using System;
using System.Web;

namespace Dragon.CPR.Sql.Grid
{
    public static class ColumnViewModelFluentExtensions
    {
        public static ColumnViewModel<T> DisplayName<T>(this ColumnViewModel<T> col, string displayName)
        {
            col.DisplayName = displayName;
            return col;
        }

        public static ColumnViewModel<T> NullValue<T>(this ColumnViewModel<T> col, string nullValue)
        {
            col.NullValue = nullValue;
            return col;
        }

        public static ColumnViewModel<T> NotSortable<T>(this ColumnViewModel<T> col)
        {
            col.Sortable = false;
            return col;
        }

        public static ColumnViewModel<T> Searchable<T>(this ColumnViewModel<T> col)
        {
            col.Searchable = true;
            return col;
        }

        public static ColumnViewModel<T> WithHeaderName<T>(this ColumnViewModel<T> col, string name)
        {
            col.DisplayName = name;
            return col;
        }


        public static ColumnViewModel<T> CellClasses<T>(this ColumnViewModel<T> col, string cellClasses)
        {
            col.CellClasses = cellClasses;
            return col;
        }

        public static ColumnViewModel<T> DisplayValueFormatter<T>(this ColumnViewModel<T> col, string displayValueFormatter)
        {
            col.DisplayValueFormatter = displayValueFormatter;
            return col;
        }

        public static ColumnViewModel<T> Hide<T>(this ColumnViewModel<T> col)
        {
            col.Visible = false;
            return col;
        }


        public static ColumnViewModel<T> WithTooltip<T>(this ColumnViewModel<T> col, Func<T, string> tooltip)
        {
            col.Tooltip = tooltip;
            return col;
        }

        public static ColumnViewModel<T> WithDisplayValue<T>(this ColumnViewModel<T> col, Func<ColumnViewData<T>, string> displayValue)
        {
            col.DisplayValue = (vc, html, o) => displayValue(new ColumnViewData<T>(vc, html, o));
            return col;
        }

        public static ColumnViewModel<T> WithHtmlDisplayValue<T>(this ColumnViewModel<T> col, Func<ColumnViewData<T>, IHtmlString> htmlDisplayValue)
        {
            col.HtmlDisplayValue = (vc, html, o) => htmlDisplayValue(new ColumnViewData<T>(vc, html, o));
            return col;
        }

        public static ColumnViewModel<T> WithDisplayValueFormat<T>(this ColumnViewModel<T> col, string format)
        {
            col.DisplayValueFormatter = format;
            return col;
        }

    }


}
