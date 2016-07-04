using System.Web;
using System.Web.Mvc;

namespace FlatFXWebClient
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RequireHttpsAttribute());

            //if (!HttpContext.Current.IsDebuggingEnabled)
            //filters.Add(new RequireHttpsAttribute());
        }
    }
}
