using FlatFXCore.BussinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace FlatFXWeb
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            try
            {
                Logger.Instance.WriteSystemTrace("Application Starting", Consts.eLogOperationStatus.Succeeded, "start");

                AreaRegistration.RegisterAllAreas();

                WebApiConfig.Register(GlobalConfiguration.Configuration);
                FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
                RouteConfig.RegisterRoutes(RouteTable.Routes);
                BundleConfig.RegisterBundles(BundleTable.Bundles);
                AuthConfig.RegisterAuth();

                Logger.Instance.WriteSystemTrace("Application Starting", Consts.eLogOperationStatus.Succeeded, "end");
            }
            catch(Exception ex)
            {
                try
                {
                    Logger.Instance.WriteError("Application Starting Failed", ex);
                    Logger.Instance.WriteSystemTrace("Application Starting", Consts.eLogOperationStatus.Failed, "end");
                }
                catch { }
            }
        }
        void Application_End(object sender, EventArgs e)
        {
            Logger.Instance.WriteSystemTrace("Application End", Consts.eLogOperationStatus.Succeeded, "");
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Logger.Instance.WriteError("Application General Failed");
        }
        protected void Session_Start(object sender, EventArgs e)
        {
        }
        protected void Session_End(object sender, EventArgs e)
        {
        }
        void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        void Application_EndRequest(object sender, EventArgs e)
        {
        }
    }
}