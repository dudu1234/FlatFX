using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.User;

namespace FlatFXWebClient.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(string lang)
        {
            if (lang != null)
            {
                //first check if the lang is valid:
                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo(lang);

                FlatFXCookie.SetCookie("lang", lang);
                Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo(lang);
                Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
                Session["lang"] = lang;

                //set user language
                string userId = User.Identity.GetUserId();
                ApplicationUser user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
                if (lang == "he-IL")
                    user.Language = FlatFXCore.BussinessLayer.Consts.eLanguage.Hebrew;
                else
                    user.Language = FlatFXCore.BussinessLayer.Consts.eLanguage.English;
                db.SaveChanges();
            }
            return View();
        }
    }
}