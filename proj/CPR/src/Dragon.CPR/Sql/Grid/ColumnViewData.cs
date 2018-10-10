using System.Web.Mvc;

namespace Dragon.CPR.Sql.Grid
{
    public class ColumnViewData<T>
    {
        public ColumnViewData(ViewContext ctx, HtmlHelper html, T data)
        {
            Ctx = ctx;
            Html = html;
            Data = data;
        }

        public ViewContext Ctx { get; set; } 
        public HtmlHelper Html { get; set; } 
        public T Data { get; set; }        
    }
}
