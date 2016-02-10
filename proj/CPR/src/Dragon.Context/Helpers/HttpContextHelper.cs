using System.Web;

namespace Dragon.Context.Helpers
{
    public class HttpContextHelper : IHttpContextHelper
    {
        public HttpContextBase Get()
        {
            return new HttpContextWrapper(HttpContext.Current);
        }
    }
}
