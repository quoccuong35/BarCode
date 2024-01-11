using System.Web;
using System.Web.Optimization;

namespace WMS
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js/jquery").Include("~/Content/js/bundle.js", "~/Content/js/scripts.js"));
            bundles.Add(new ScriptBundle("~/js/wms").Include("~/Scripts/wms.js"));
            bundles.Add(new ScriptBundle("~/js/login").Include("~/Content/js/bundle.js", "~/Content/js/scripts.js", "~/Scripts/login.js"));
            bundles.Add(new StyleBundle("~/Content/css/site").Include("~/Content/css/wms.css", "~/Content/css/theme.css"));
            bundles.Add(new ScriptBundle("~/js/extreme").Include("~/Scripts/jszip.min.js", "~/Scripts/dx.all.js", "~/Scripts/globalize.min.js"));
            bundles.Add(new StyleBundle("~/Content/css/extreme").Include("~/Content/css/dx.common.css", "~/Content/css/dx.light.css"));
            BundleTable.EnableOptimizations = true;
        }
    }
}
