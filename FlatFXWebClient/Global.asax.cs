﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Core;

namespace FlatFXWebClient
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            DateTime start = DateTime.Now;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            

            //Start application instances
            ApplicationInformation.Instance.Start();
            Config.Instance.Start();
            Logger.Instance.Start();
            CurrencyManager.Instance.Start();
            CurrencyFeedManager.Instance.Start();
            DailyTasks.Instance.Start();
            NotificationManager.Instance.Start();

            Logger.Instance.WriteSystemTrace("Application Start", Consts.eLogOperationStatus.Succeeded, "Application Start. start: " + start.ToString("HH:mm:ss") + ", end: " + DateTime.Now.ToString("HH:mm:ss"));
        }
        protected void Session_Start(object sender, EventArgs e)
        {
            string lang = FlatFXCookie.GetCookieValue("lang");
            if (lang == null)
            {
                lang = "he-IL"; //default

                try
                {
                    // http://ip-api.com/json/208.80.152.201
                    string ip = HttpContext.Current.Request.UserHostAddress;
                    if (ip != null && ip.Length > 7)
                    {
                        System.Net.WebClient web = new System.Net.WebClient();
                        string url = "http://ip-api.com/json/" + ip;
                        string response = web.DownloadString(url);

                        if (!response.Contains("Israel"))
                            lang = "en-US";
                    }
                }
                catch { }

                Session["lang"] = lang;
                FlatFXCookie.SetCookie("lang", lang);
            }

            if (lang != null)
            {
                Session["lang"] = lang;
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(Session["lang"].ToString());
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }

            Logger.Instance.WriteSystemTrace("Session Start", Consts.eLogOperationStatus.Succeeded, "Session Start");
        }
        private void Application_BeginRequest(Object source, EventArgs e)
        {
            string lang = FlatFXCookie.GetCookieValue("lang");
            if (lang != null && lang != Thread.CurrentThread.CurrentCulture.Name)
            {
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            }
        }
    }
}
