using System.Web;
using System.Web.Optimization;

namespace FlatFXWebClient
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/csszz").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/bootstrap-theme.min.css",
                      "~/Content/bootstrap-toggle.min.css",
                      "~/Content/angular-block-ui.min.css",
                      "~/Content/font-awesome.min.css",
                      "~/Content/jquery-ui.min.css",
                      "~/Content/ng-table.css"
                      ));

            bundles.Add(new ScriptBundle("~/Scripts/jqueryzz").Include(
                        "~/Scripts/jquery-2.2.0.js",
                        "~/Scripts/jquery-ui-1.11.4.min.js",
                        "~/Scripts/jquery.fullPage.js",
                        "~/Scripts/jquery.validate-vsdoc.js",
                        "~/Scripts/jquery.validate.min.js",
                        "~/Scripts/jquery.validate.unobtrusive.min.js"
                        ));

            bundles.Add(new ScriptBundle("~/Scripts/modernizrzz").Include(
                        "~/Scripts/modernizr-*"));
            
            bundles.Add(new ScriptBundle("~/Scripts/bootstrapzz").Include(
                      "~/Scripts/bootstrap.min.js",
                      "~/Scripts/respond.min.js"));

            bundles.Add(new ScriptBundle("~/Scripts/angularzz1").Include(
                       "~/Scripts/angular.js"
                       ));

            bundles.Add(new ScriptBundle("~/Scripts/angularzz2").Include(
                       "~/Scripts/angular-ui.min.js"
                       ));

            bundles.Add(new ScriptBundle("~/Scripts/angularzz3").Include(
                       "~/Scripts/angular-route.min.js",
                       "~/Scripts/angular-sanitize.min.js",
                       "~/Scripts/angular-block-ui.min.js"
                       ));

            bundles.Add(new ScriptBundle("~/Scripts/angular-ui/angularzz4").Include(
                       "~/Scripts/angular-ui/ui-bootstrap.min.js"
                       ));

            bundles.Add(new ScriptBundle("~/Scripts/diff0zz").Include(
                        "~/Scripts/jquery.noty.packaged.js"
                        )); 
            
            bundles.Add(new ScriptBundle("~/Scripts/diffzz").Include(
                        "~/Scripts/angular-noty.js",
                        "~/Scripts/ui-bootstrap-tpls-0.12.0.min.js",
                        "~/Scripts/bootstrap-confirmation.js",
                        "~/Scripts/bootstrap-toggle.min.js",
                        "~/Scripts/ng-table.min.js",
                        "~/Scripts/ngStorage.min.js",
                        "~/Scripts/Chart.min.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/RightToLeft1csszz").Include(
                        "~/Content/bootstrap.rtl.min.css"));

            bundles.Add(new ScriptBundle("~/Scripts/RightToLeft1jszz").Include(
                        "~/Scripts/bootstrap.rtl.min.js"));


            bundles.Add(new StyleBundle("~/Content/FFXcss1zz").Include(
                        "~/Content/fullpageex.css"));

            bundles.Add(new StyleBundle("~/Content/FFXcss2zz").Include(
                        "~/Content/Site.css"));

            bundles.Add(new ScriptBundle("~/JS/Functions/FFXjszz").Include(
                        "~/JS/Functions/Charts.js",
                        "~/JS/Functions/fcsaNumber.js",
                        "~/JS/Functions/NotyFunctions.js"
                        )); 
            
            bundles.Add(new ScriptBundle("~/JS/FFXjs1zz").Include(
                        "~/JS/FFx.js"
                        ));

            bundles.Add(new ScriptBundle("~/JS/Services/FFXjs2zz").Include(
                        "~/JS/Services/Services.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/FFXcssRtlzz").Include(
                        "~/Content/rtlCustom.css"));

            bundles.Add(new ScriptBundle("~/JS/FFXjsRtlzz").Include(
                        "~/JS/rtlCustom.js"));

            bundles.Add(new ScriptBundle("~/JS/Controllers/FFXControllerszz").Include(
                        "~/JS/Controllers/AllControllers.js"));
        }
    }
}
