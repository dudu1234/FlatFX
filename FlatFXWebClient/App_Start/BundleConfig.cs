using System.Web;
using System.Web.Optimization;

namespace FlatFXWebClient
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.fullPage.js",
                        "~/Scripts/jquery.validate-vsdoc.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js",
                        "~/Scripts/knockout-{version}.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.min.js"));

            //// Styles for RTL
            //bundles.Add(new StyleBundle("~/bundles/styleRTL").Include(
            //           "~/Content/bootstrap.rtl.min.css",
            //           "~/Content/rtlCustom.css"
            //           ));

            //bundles.Add(new ScriptBundle("~/bundles/scriptRTL").Include(
            //           "~/Scripts/bootstrap.rtl.min.js",
            //           "~/Scripts/rtlCustom.js"
            //           ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap-theme.min.css",
                      "~/Content/bootstrap-theme.css.map",
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap.css.map",
                      "~/Content/bootstrap.min.css",
                      "~/Content/examples.css",
                      "~/Content/fullpageex.css",
                      "~/Content/jquery.fullPage.css",
                      "~/Content/Site.css"
                      ));
        }
    }
}
