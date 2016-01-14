using System.Web;
using System.Web.Optimization;

namespace FlatFXWebClient
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                //"~/Content/bootstrap-theme.css.map",
                //"~/Content/bootstrap.css",
                //"~/Content/bootstrap.css.map",
                      "~/Content/bootstrap.min.css",
                      "~/Content/bootstrap-theme.min.css",
                      "~/Content/examples.css",
                      "~/Content/angular-block-ui.min.css",
                      "~/Content/font-awesome.min.css"
                /*"~/Content/fullpageex.css",
                "~/Content/jquery.fullPage.css"*/
                      ));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.fullPage.js",
                        "~/Scripts/jquery.validate-vsdoc.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js"
                //"~/Scripts/knockout-{version}.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/jquery.noty.packaged.js",
                      "~/Scripts/respond.min.js",
                      "~/Scripts/angular.min.js",
                       "~/Scripts/angular-route.min.js",
                       "~/Scripts/angular-sanitize.min.js",
                       "~/Scripts/angular-ui.min.js",
                       "~/Scripts/angular-ui/ui-bootstrap.min.js",
                       "~/Scripts/angular-ui/ui-bootstrap-tpls.min.js",
                       "~/Scripts/angular-ui.min.js",
                       "~/Scripts/angular-block-ui.js"));

            bundles.Add(new ScriptBundle("~/bundles/noty").Include(
                        "~/Scripts/angular-noty.js"));

            bundles.Add(new StyleBundle("~/bundles/RightToLeft1css").Include(
                        "~/Content/bootstrap.rtl.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/RightToLeft1js").Include(
                        "~/Scripts/bootstrap.rtl.min.js"));


            bundles.Add(new StyleBundle("~/bundles/FFXcss1").Include(
                        "~/Content/fullpageex.css"));

            bundles.Add(new StyleBundle("~/bundles/FFXcss2").Include(
                        "~/Content/Site.css"));

            bundles.Add(new ScriptBundle("~/bundles/FFXjs").Include(
                //"~/JS/Controllers/*.js",
                        "~/JS/FFx.js",
                        "~/JS/Services/Services.js",
                        "~/JS/Functions/NotyFunctions.js"));

            bundles.Add(new ScriptBundle("~/bundles/FFXControllers").IncludeDirectory(
                "~/JS/Controllers", "*.js"));

            bundles.Add(new StyleBundle("~/bundles/FFXcssRtl").Include(
                        "~/Content/rtlCustom.css"));

            bundles.Add(new ScriptBundle("~/bundles/FFXjsRtl").Include(
                        "~/JS/rtlCustom.js"));

        }
    }
}
