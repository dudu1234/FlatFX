using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FlatFXWebClient.Controllers
{
    public class OnLineFXRatesController : BaseController
    {
        /// <summary>
        /// ShowRates
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowRates()
        {
            return View();
        }
        /// <summary>
        /// Return the current rates
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRates()
        {
            try
            {
                IEnumerable<FXRate> Rates = CurrencyManager.Instance.PairRates.Values.Where(r => r.Key != "USDUSD");
                foreach(FXRate rate in Rates)
                {
                    rate.KeyDisplay = FlatFXResources.Resources.ResourceManager.GetString(rate.Key);
                }
                RatesResponse ratesResponse = new RatesResponse(Rates, CurrencyManager.Instance.LastFeedUpdate);
                return Json(ratesResponse, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return null;
            }
        }
    }
    public class RatesResponse
    {
        public DateTime LastFeedUpdate = DateTime.Now;
        public IEnumerable<FXRate> Rates = null;

        public RatesResponse(IEnumerable<FXRate> Rates, DateTime LastFeedUpdate)
        {
            this.Rates = Rates;
            this.LastFeedUpdate = LastFeedUpdate;
        }
    }
}
