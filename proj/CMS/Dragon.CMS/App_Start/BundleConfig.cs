using System.Web;
using System.Web.Optimization;
using BundleTransformer.Core.Builders;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;

namespace Dragon.CMS
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

#if !DEBUG
            BundleTable.EnableOptimizations = true;
#endif
            var nullBuilder = new NullBuilder();
            var styleTransformer = new StyleTransformer();
            var nullOrderer = new NullOrderer();

            // 
            // CUSTOM
            //

            var sb = new ScriptBundle("~/bundles/scripts");
            sb.IncludeDirectory("~/assets/js/", "*.js", true);

            bundles.Add(sb);

            var styleBundle = new Bundle("~/bundles/styles");
            styleBundle.IncludeDirectory("~/assets/style/", "*.css", true);
            styleBundle.IncludeDirectory("~/assets/style/", "*.less", true);

            styleBundle.Builder = nullBuilder;
            styleBundle.Transforms.Add(styleTransformer);
            styleBundle.Orderer = nullOrderer;
            bundles.Add(styleBundle);
        }
    }
}
