using FlatFXCore.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace FlatFXWebClient
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        private void Application_BeginRequest(Object source, EventArgs e)
        {
            
            HttpApplication application = (HttpApplication)source;
            HttpContext context = application.Context;

            string lang = FlatFXCookie.GetCookieValue("lang");
            if (lang != null && lang != Thread.CurrentThread.CurrentCulture.Name)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(FlatFXCookie.GetCookieValue("lang"));
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }
        }
        protected void Session_Start(object sender, EventArgs e)
        {
            string lang = FlatFXCookie.GetCookieValue("lang");
            if (lang != null)
                Session["lang"] = lang;
            else
                Session["lang"] = "en-US";
            
        }
    }
}
