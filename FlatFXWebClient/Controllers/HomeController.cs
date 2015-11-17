using FlatFXCore.Model.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace FlatFXWebClient.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(string lang)
        {
            if (lang != null)
            {
                FlatFXCookie.SetCookie("lang", lang);
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
                Session["lang"] = lang;
            }
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}