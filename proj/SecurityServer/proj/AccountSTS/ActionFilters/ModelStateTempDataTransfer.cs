using System.Web.Mvc;

namespace Dragon.SecurityServer.AccountSTS.ActionFilters
{
    /// <summary>
    /// See <see href="http://weblogs.asp.net/rashid/asp-net-mvc-best-practices-part-1#prg">ASP.NET MVC Best Practices (Part 1)/</see>
    /// </summary>
    public abstract class ModelStateTempDataTransfer : ActionFilterAttribute
    {
        protected static readonly string Key = typeof(ModelStateTempDataTransfer).FullName;
    }
}