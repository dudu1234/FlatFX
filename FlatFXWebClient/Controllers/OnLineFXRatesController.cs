using FlatFXCore.BussinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FlatFXWeb.Controllers
{
    public class OnLineFXRatesController : Controller
    {
        //
        // GET: /OnLineFXRates/

        public ActionResult ShowRates()
        {
            CurrencyFeedManager.Instance.GetYahooRates();
            return View();
        }
        /// <summary>
        /// Return the Yahoo current rate
        /// </summary>
        /// <returns></returns>
        public string GetRates()
        {
            try
            {
                return CurrencyFeedManager.Instance.GetYahooRates();
            }
            catch
            {
                return "";
            }
        }
    }
}
