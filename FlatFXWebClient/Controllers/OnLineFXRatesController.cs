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
        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetHistoricalRates(string Symbol, DateTime? date1)
        {
            DateTime date = date1.HasValue ? date1.Value : DateTime.Now;
            int IntervalInMinutes = 30;
            if (Symbol == null || Symbol.Length != 6)
                Symbol = "USDILS";

            try
            {
                DateTime start = date.AddMinutes(-1 * IntervalInMinutes);
                DateTime end = date.AddMinutes(IntervalInMinutes);
                IEnumerable<DailyFXRate> Rates = db.DailyFXRates.Where(d => d.Key == Symbol && d.Time > start && d.Time < end).ToList();
                start = new DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, 0).AddMinutes(-5);
                end = start.AddMinutes(6);
                DailyFXRate Rate = Rates.Where(d => d.Time > start && d.Time < end).FirstOrDefault();
                DailyRatesResponse ratesResponse = new DailyRatesResponse(Rates, (Rate != null) ? Rate.Mid : -1);
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
        public string LastFeedUpdate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        public IEnumerable<FXRate> Rates = null;

        public RatesResponse(IEnumerable<FXRate> Rates, DateTime LastFeedUpdate)
        {
            this.Rates = Rates;
            this.LastFeedUpdate = LastFeedUpdate.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
    public class DailyRatesResponse
    {
        public double Rate = -1;
        public IEnumerable<DailyFXRate> Rates = null;

        public DailyRatesResponse(IEnumerable<DailyFXRate> Rates, double Rate)
        {
            this.Rate = Rate;
            this.Rates = Rates;
        }
    }
}
