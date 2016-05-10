using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FlatFXCore.Model.Core
{
    public class FlatFXCookie
    {
        public const string CookieName = "FlatFX";
        
        public FlatFXCookie()
        {
        }

        public static void SetCookie(string key, string value)
        {
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[CookieName] ?? new HttpCookie(CookieName);
            myCookie.Values[key] = value;
            myCookie.Expires = DateTime.Now.AddDays(365);
            HttpContext.Current.Response.Cookies.Add(myCookie);
        }

        public static string GetCookieValue(string key)
        {
            HttpCookie myCookie = HttpContext.Current.Request.Cookies[CookieName];
            if (myCookie != null)
                return myCookie.Values[key];
            else
                return null;
        }


    }
}
