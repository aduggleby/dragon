using System.Web;

namespace Dragon.Context.Helpers
{
    public interface IHttpContextHelper
    {
        HttpContextBase Get();
    }
}
