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

            //total volume
            model.SiteTotalVolume = db.Deals.Where(d => d.IsCanceled == false /*&& d.IsDemo == false*/ && d.IsOffer == false).Sum(d => d.AmountUSD).ToInt();
            model.SiteTodayVolume = db.Deals.Where(d => d.IsCanceled == false /*&& d.IsDemo == false*/ && d.IsOffer == false &&
                DbFunctions.TruncateTime(d.OfferingDate) == DbFunctions.TruncateTime(DateTime.Now)).Sum(d => d.AmountUSD).ToInt();
            model.SiteTotalSavings = db.Deals.Where(d => d.IsCanceled == false /*&& d.IsDemo == false*/ && d.IsOffer == false).Sum(d => (double?)d.CustomerTotalProfitUSD).ToInt(0);
            model.SiteTotalNumberOfDeals = db.Deals.Where(d => d.IsCanceled == false /*&& d.IsDemo == false*/ && d.IsOffer == false).Count();
            
            //company volume
            string companyId = user.Companies.First().CompanyId;
            model.CompanyVolume = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId && d.IsCanceled == false /*&& d.IsDemo == false*/ &&
                d.IsOffer == false).Sum(d => (double?)d.AmountUSD).ToInt(0);
            model.CompanyTodayVolume = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId && d.IsCanceled == false /*&& d.IsDemo == false*/ &&
                d.IsOffer == false && DbFunctions.TruncateTime(d.OfferingDate) == DbFunctions.TruncateTime(DateTime.Now)).Sum(d => (double?)d.AmountUSD).ToInt(0); 
            model.CompanySavings = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId && d.IsCanceled == false /*&& d.IsDemo == false*/ &&
                d.IsOffer == false).Sum(d => (double?)d.CustomerTotalProfitUSD).ToInt(0);
            model.CompanyNumberOfDeal = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId && d.IsCanceled == false /*&& d.IsDemo == false*/ &&
                d.IsOffer == false).Count();

            // CompanyDailyVolumeList - 30 days back
            model.CompanyDailyVolumeList = db.Deals.Where(d4 => d4.CompanyAccount.Company.CompanyId == companyId && d4.IsCanceled == false /*&& d.IsDemo == false*/ &&
                d4.IsOffer == false && d4.OfferingDate > DbFunctions.AddDays(DateTime.Now, -30)).GroupBy(d => DbFunctions.TruncateTime(d.OfferingDate))
                .Select(d2 => d2.Sum(d3 => d3.AmountUSD)).ToList();
            // CompanyMonthlyVolumeList - 6 month back
            model.CompanyMonthlyVolumeList = db.Deals.Where(d4 => d4.CompanyAccount.Company.CompanyId == companyId && d4.IsCanceled == false /*&& d.IsDemo == false*/ &&
                d4.IsOffer == false && d4.OfferingDate > DbFunctions.AddMonths(DateTime.Now, -6)).GroupBy(d => d.OfferingDate.Month + "-" + d.OfferingDate.Year).Select(d2 => d2.Sum(d3 => d3.AmountUSD)).ToList();
            return View(model);
        }
        
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}