using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using FlatFXCore.Model.User;
using FlatFXWebClient.ViewModels;
using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Core;
using System.Collections.Generic;
using FlatFXCore.Model.Data;
using System.Data.Entity;
using System.Globalization;

namespace FlatFXWebClient.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        
        public DashboardController()
        {
        }

        public ActionResult DashboardIndex()
        {
            DashboardIndexViewModel model = new DashboardIndexViewModel();

            var userId = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }

            model.UserName = user.FullName;
            model.CompanyName = user.Companies.First().CompanyName;

            // Guy To Do : Change all db.Deals.ToList().Where ... IsRealDeal - to a better loading. Now the query can all deals and then perform the Where section.
            // a possible solution is to copy the IsRealDeal login to each where section.

            //total volume
            model.SiteTotalVolume = db.Deals.ToList().Where(d => d.IsRealDeal).Sum(d => d.AmountUSD).ToInt();
            model.SiteTodayVolume = db.Deals.Where(d => DbFunctions.TruncateTime(d.OfferingDate) == DbFunctions.TruncateTime(DateTime.Now)).ToList().Where(d => d.IsRealDeal).
                Sum(d => (double?)d.AmountUSD).ToInt();
            model.SiteTotalSavings = db.Deals.ToList().Where(d => d.IsRealDeal).Sum(d => (double?)d.CustomerTotalProfitUSD).ToInt(0);
            model.SiteTotalNumberOfDeals = db.Deals.ToList().Where(d => d.IsRealDeal).Count();
            
            //company volume
            string companyId = user.Companies.First().CompanyId;
            model.CompanyVolume = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId).ToList().Where(d => d.IsRealDeal).
                Sum(d => (double?)d.AmountUSD).ToInt(0);
            model.CompanyTodayVolume = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId && DbFunctions.TruncateTime(d.OfferingDate) == DbFunctions.TruncateTime(DateTime.Now)).
                ToList().Where(d => d.IsRealDeal).Sum(d => (double?)d.AmountUSD).ToInt(0);
            model.CompanySavings = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId).ToList().Where(d => d.IsRealDeal).
                Sum(d => (double?)d.CustomerTotalProfitUSD).ToInt(0);
            model.CompanyNumberOfDeal = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId).ToList().Where(d => d.IsRealDeal).Count();

            return View(model);
        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public ActionResult GetCompanyVolume()
        {
            try
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user = db.Users.Where(u => u.Id == userId).FirstOrDefault();
                if (user == null)
                {
                    return HttpNotFound();
                }
                string companyId = user.Companies.First().CompanyId;

                // CompanyDailyVolumeList - 14 days back
                // Guy to do example for inner join
                int daysBack = 14;
                List<DateTime> days = GeneralFunction.GetDays(daysBack, true);
                Dictionary<string, int> dicDays = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId && d.OfferingDate > DbFunctions.AddDays(DateTime.Now, -1 * daysBack)).ToList()
                    .Where(d => d.IsRealDeal)
                    .GroupBy(d => d.OfferingDate.ToDateString("dd/MM/yyyy"))
                    .Select(d => new Tuple<DateTime, int>(d.Max(d2 => new DateTime(d2.OfferingDate.Year, d2.OfferingDate.Month, d2.OfferingDate.Day, 0, 0, 0)), d.Sum(d3 => d3.AmountUSD).ToInt()))
                    .ToDictionary(d => d.Item1.ToString("MM/dd"), d => d.Item2);
                foreach (DateTime date in days)
                {
                    if (!dicDays.ContainsKey(date.ToString("MM/dd")))
                        dicDays.Add(date.ToString("MM/dd"), 0);
                }

                // CompanyMonthlyVolumeList - 6 month back
                int monthBack = 6;
                List<DateTime> months = GeneralFunction.GetMonth(monthBack, true);
                Dictionary<string, int> dicMonth = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId && d.OfferingDate > DbFunctions.AddMonths(DateTime.Now, -1 * monthBack)).ToList()
                    .Where(d => d.IsRealDeal)
                    .GroupBy(d => d.OfferingDate.Month + "-" + d.OfferingDate.Year)
                    .Select(d => new Tuple<string, int>(d.Max(d2 => d2.OfferingDate.ToString("MMMM-yy")), d.Sum(d3 => d3.AmountUSD).ToInt()))
                    .ToDictionary(d => d.Item1, d => d.Item2);
                foreach (DateTime dt in months)
                {
                    if (!dicMonth.ContainsKey(dt.ToString("MMMM-yy")))
                        dicMonth.Add(dt.ToString("MMMM-yy"), 0);
                }

                ActionResult res = Json(new {
                    companyDailyVolume = dicDays.OrderBy(d => DateTime.ParseExact(d.Key, "MM/dd", System.Threading.Thread.CurrentThread.CurrentCulture)),
                    companyMonthlyVolume = dicMonth.OrderBy(d => DateTime.ParseExact(d.Key, "MMMM-yy", System.Threading.Thread.CurrentThread.CurrentCulture)) 
                    }, JsonRequestBehavior.AllowGet);
                
                return res;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in GetCompanyVolume", ex);
                return null;
            }
        }

        public ActionResult GetSiteVolume()
        {
            try
            {
                // DailyVolumeList - 14 days back
                int daysBack = 14;
                List<DateTime> days = GeneralFunction.GetDays(daysBack, true);
                Dictionary<string, int> dicDays = db.Deals.Where(d => d.OfferingDate > DbFunctions.AddDays(DateTime.Now, -1 * daysBack)).ToList()
                    .Where(d => d.IsRealDeal)
                    .GroupBy(d => d.OfferingDate.ToDateString("dd/MM/yyyy"))
                    .Select(d => new Tuple<DateTime, int>(d.Max(d2 => new DateTime(d2.OfferingDate.Year, d2.OfferingDate.Month, d2.OfferingDate.Day, 0, 0, 0)), d.Sum(d3 => d3.AmountUSD).ToInt()))
                    .ToDictionary(d => d.Item1.ToString("MM/dd"), d => d.Item2);
                foreach (DateTime date in days)
                {
                    if (!dicDays.ContainsKey(date.ToString("MM/dd")))
                        dicDays.Add(date.ToString("MM/dd"), 0);
                }

                // MonthlyVolumeList - 6 month back
                int monthBack = 6;
                List<DateTime> months = GeneralFunction.GetMonth(monthBack, true);
                Dictionary<string, int> dicMonth = db.Deals.Where(d => d.OfferingDate > DbFunctions.AddMonths(DateTime.Now, -1 * monthBack)).ToList()
                    .Where(d => d.IsRealDeal)
                    .GroupBy(d => d.OfferingDate.Month + "-" + d.OfferingDate.Year)
                    .Select(d => new Tuple<string, int>(d.Max(d2 => d2.OfferingDate.ToString("MMMM-yy")), d.Sum(d3 => d3.AmountUSD).ToInt()))
                    .ToDictionary(d => d.Item1, d => d.Item2);
                foreach (DateTime dt in months)
                {
                    if (!dicMonth.ContainsKey(dt.ToString("MMMM-yy")))
                        dicMonth.Add(dt.ToString("MMMM-yy"), 0);
                }

                ActionResult res = Json(new
                {
                    dailyVolume = dicDays.OrderBy(d => DateTime.ParseExact(d.Key, "MM/dd", System.Threading.Thread.CurrentThread.CurrentCulture)),
                    monthlyVolume = dicMonth.OrderBy(d => DateTime.ParseExact(d.Key, "MMMM-yy", System.Threading.Thread.CurrentThread.CurrentCulture))
                }, JsonRequestBehavior.AllowGet);

                return res;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in GetSiteVolume", ex);
                return null;
            }
        }
    }
}