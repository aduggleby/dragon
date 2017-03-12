using System.Web.Mvc;

namespace Dragon.SecurityServer.AccountSTS.ActionFilters
{
    /// <summary>
    /// See <see href="http://weblogs.asp.net/rashid/asp-net-mvc-best-practices-part-1#prg">ASP.NET MVC Best Practices (Part 1)/</see>
    /// </summary>
    public class ImportModelStateFromTempData : ModelStateTempDataTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ModelStateDictionary modelState = filterContext.Controller.TempData[Key] as ModelStateDictionary;

            if (modelState != null)
            {
                //Only Import if we are viewing
                if (filterContext.Result is ViewResult)
                {
                    filterContext.Controller.ViewData.ModelState.Merge(modelState);
                }
                else
                {
                    //Otherwise remove it.
                    filterContext.Controller.TempData.Remove(Key);
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}