using System.Web;
using Dragon.Security.Hmac.Module;
using Dragon.Security.Hmac.Module.Modules;

[assembly: PreApplicationStartMethod(typeof(PreApplicationStart), "Start")]

namespace Dragon.Security.Hmac.Module
{
    public class PreApplicationStart
    {
        public static void Start()
        {
            Microsoft.Web.Infrastructure.DynamicModuleHelper.DynamicModuleUtility.RegisterModule(typeof(HmacHttpModule));
        }
    }
}