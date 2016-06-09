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

        public ActionResult DashboardIndex(string TabName)
        {
            DashboardIndexViewModel model = new DashboardIndexViewModel();
            if (TabName != null)
                model.TabName = TabName;

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
                DateTime today = DateTime.Today;
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
                DateTime daysBackDT = DateTime.Now.AddDays(-1 * daysBack);
                Dictionary<string, int> dicDays = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId && d.OfferingDate > daysBackDT).ToList()
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
                DateTime monthesBackDT = DateTime.Now.AddMonths(-1 * monthBack);
                Dictionary<string, int> dicMonth = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId && d.OfferingDate > monthesBackDT).ToList()
                    .Where(d => d.IsRealDeal)
                    .GroupBy(d => d.OfferingDate.Month + "-" + d.OfferingDate.Year)
                    .Select(d => new Tuple<string, int>(d.Max(d2 => d2.OfferingDate.ToString("MMMM-yy")), d.Sum(d3 => d3.AmountUSD).ToInt()))
                    .ToDictionary(d => d.Item1, d => d.Item2);
                foreach (DateTime dt in months)
                {
                    if (!dicMonth.ContainsKey(dt.ToString("MMMM-yy")))
                        dicMonth.Add(dt.ToString("MMMM-yy"), 0);
                }

                DashboardStatisticsViewModel model = new DashboardStatisticsViewModel();

                // Guy To Do : Change all db.Deals.ToList().Where ... IsRealDeal - to a better loading. Now the query can all deals and then perform the Where section.
                // a possible solution is to copy the IsRealDeal login to each where section.

                //company volume
                model.CompanyVolume = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId).ToList().Where(d => d.IsRealDeal).
                    Sum(d => (double?)d.AmountUSD).ToInt(0);
                model.CompanyTodayVolume = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId && d.OfferingDate >= today).
                    ToList().Where(d => d.IsRealDeal).Sum(d => (double?)d.AmountUSD).ToInt(0);
                model.CompanySavings = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId).ToList().Where(d => d.IsRealDeal).
                    Sum(d => (double?)d.CustomerTotalProfitUSD).ToInt(0);
                model.CompanyNumberOfDeal = db.Deals.Where(d => d.CompanyAccount.Company.CompanyId == companyId).ToList().Where(d => d.IsRealDeal).Count();


                ActionResult res = Json(new {
                    companyDailyVolume = dicDays.OrderBy(d => DateTime.ParseExact(d.Key, "MM/dd", System.Threading.Thread.CurrentThread.CurrentCulture)),
                    companyMonthlyVolume = dicMonth.OrderBy(d => DateTime.ParseExact(d.Key, "MMMM-yy", System.Threading.Thread.CurrentThread.CurrentCulture)),
                    DashboardStatisticsViewModel = model
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
                DateTime today = DateTime.Today;
                List<DateTime> days = GeneralFunction.GetDays(daysBack, true);
                DateTime daysBackDT = DateTime.Now.AddDays(-1 * daysBack);
                Dictionary<string, int> dicDays = db.Deals.Where(d => d.OfferingDate > daysBackDT).ToList()
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
                DateTime monthesBackDT = DateTime.Now.AddMonths(-1 * monthBack);
                Dictionary<string, int> dicMonth = db.Deals.Where(d => d.OfferingDate > monthesBackDT).ToList()
                    .Where(d => d.IsRealDeal)
                    .GroupBy(d => d.OfferingDate.Month + "-" + d.OfferingDate.Year)
                    .Select(d => new Tuple<string, int>(d.Max(d2 => d2.OfferingDate.ToString("MMMM-yy")), d.Sum(d3 => d3.AmountUSD).ToInt()))
                    .ToDictionary(d => d.Item1, d => d.Item2);
                foreach (DateTime dt in months)
                {
                    if (!dicMonth.ContainsKey(dt.ToString("MMMM-yy")))
                        dicMonth.Add(dt.ToString("MMMM-yy"), 0);
                }

                DashboardStatisticsViewModel model = new DashboardStatisticsViewModel();

                // Guy To Do : Change all db.Deals.ToList().Where ... IsRealDeal - to a better loading. Now the query can all deals and then perform the Where section.
                // a possible solution is to copy the IsRealDeal login to each where section.

                //total volume
                model.SiteTotalVolume = db.Deals.ToList().Where(d => d.IsRealDeal).Sum(d => d.AmountUSD).ToInt();
                model.SiteTodayVolume = db.Deals.Where(d => d.OfferingDate >= today).ToList().Where(d => d.IsRealDeal).
                    Sum(d => (double?)d.AmountUSD).ToInt();
                model.SiteTotalSavings = db.Deals.ToList().Where(d => d.IsRealDeal).Sum(d => (double?)d.CustomerTotalProfitUSD).ToInt(0);
                model.SiteTotalNumberOfDeals = db.Deals.ToList().Where(d => d.IsRealDeal).Count();

                ActionResult res = Json(new
                {
                    dailyVolume = dicDays.OrderBy(d => DateTime.ParseExact(d.Key, "MM/dd", System.Threading.Thread.CurrentThread.CurrentCulture)),
                    monthlyVolume = dicMonth.OrderBy(d => DateTime.ParseExact(d.Key, "MMMM-yy", System.Threading.Thread.CurrentThread.CurrentCulture)),
                    DashboardStatisticsViewModel = model
                }, JsonRequestBehavior.AllowGet);

                return res;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in GetSiteVolume", ex);
                return null;
            }
        }

        public ActionResult GetDeals(bool onlyActiveDeals)
        {
            try
            {
                bool isDemo = ApplicationInformation.Instance.IsDemoUser;
                string userId = ApplicationInformation.Instance.UserID;
                ApplicationUser user = db.Users.Include(u => u.Companies).Where(u => u.Id == userId && u.IsActive == true).FirstOrDefault();
                string companyId = user.Companies.First().CompanyId;

                List<DealItem> deals = db.Deals
                    .Where(d => d.IsDemo == isDemo && d.IsOffer == false && d.CompanyAccount.Company.CompanyId == companyId &&
                        (!onlyActiveDeals || (d.Status == Consts.eDealStatus.New || d.Status == Consts.eDealStatus.CustomerTransfer || d.Status == Consts.eDealStatus.FlatFXTransfer || d.Status == Consts.eDealStatus.Problem)))
                    .ToList()
                    .Where(d => d.MaturityDate.HasValue)
                    .Select(d => new DealItem(d.DealId, d.BuySell, d.AmountToExchangeChargedCurrency, d.AmountToExchangeCreditedCurrency,
                        d.ChargedCurrency, d.CreditedCurrency, d.Commission, d.ContractDate, d.CustomerRate,
                        d.CustomerTotalProfitUSD, d.IsCanceled, d.PvPEnabled, d.EnsureOnLinePrice, d.FastTransferEnabled, d.IsDemo? "" : d.user.UserName, d.Status.ToString(), d.StatusDetails, d.DealProductType))
                    .ToList();

                ActionResult res = Json(new
                {
                    Deals = deals
                }, JsonRequestBehavior.AllowGet);

                return res;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in GetDeals", ex);
                return null;
            }
        }

        public ActionResult GetOrders(bool onlyActiveOrders)
        {
            try
            {
                bool isDemo = ApplicationInformation.Instance.IsDemoUser;
                string userId = ApplicationInformation.Instance.UserID;
                ApplicationUser user = db.Users.Include(u => u.Companies).Where(u => u.Id == userId && u.IsActive == true).FirstOrDefault();
                string companyId = user.Companies.First().CompanyId;

                List<OrderItem> orders = db.Orders
                    .Where(o => o.IsDemo == isDemo && o.Status != Consts.eOrderStatus.None && o.CompanyAccount.Company.CompanyId == companyId && 
                        (!onlyActiveOrders || (o.Status == Consts.eOrderStatus.Problem || o.Status == Consts.eOrderStatus.Triggered ||
                        o.Status == Consts.eOrderStatus.Triggered_partially || o.Status == Consts.eOrderStatus.Waiting)))
                    .ToList()
                    .Select(o => new OrderItem(o.OrderId, o.BuySell, o.AmountCCY1, o.AmountCCY2_Estimation, o.Symbol, o.FlatFXCommissionUSD_Estimation,
                        o.OrderDate, o.CustomerTotalProfitUSD_Estimation, o.IsDemo ? "" : o.user.UserName, o.Status.ToString(), o.StatusDetails,
                        o.MinimalPartnerExecutionAmountCCY1, o.ExpiryDate, o.AmountCCY1_Executed, o.AmountCCY1_Remainder, o.EnsureOnLinePrice, o.PvPEnabled, o.ClearingType, o.MinRate, o.MaxRate))
                    .ToList();

                ActionResult res = Json(new
                {
                    Orders = orders
                }, JsonRequestBehavior.AllowGet);

                return res;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in GetOrders", ex);
                return null;
            }
        }

        public ActionResult Cancel(string type, long id)
        {
            try
            {
                if (type != "Deal" && type != "Order")
                    return Json(new { Error = "Invalid Parameters" }, JsonRequestBehavior.AllowGet);
                
                if (type == "Deal")
                {
                    Deal deal = db.Deals.Where(d => d.DealId == id).SingleOrDefault();
                    deal.IsCanceled = true;
                    deal.Status = Consts.eDealStatus.Canceled;
                    db.SaveChanges();
                }
                else
                {
                    Order order = db.Orders.Where(o => o.OrderId == id).SingleOrDefault();
                    order.Status = Consts.eOrderStatus.Canceled;
                    db.SaveChanges();
                }

                return Json(new { Error = "" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in GetOrders", ex);
                return Json(new { Error = "General Error" }, JsonRequestBehavior.AllowGet);
            }
        }
    }

    public class DealItem
    {
        public Int64 DealId;
        public Consts.eBuySell BuySell;
        public double AmountToExchangeChargedCurrency;
        public double AmountToExchangeCreditedCurrency;
        public string ChargedCurrency;
        public string CreditedCurrency;
        public double? Commission;
        public DateTime? ContractDate;
        public double CustomerRate;
        public double? CustomerTotalProfitUSD;
        public bool IsCanceled;
        public bool PvPEnabled;
        public bool EnsureOnLinePrice;
        public bool FastTransferEnabled;
        public string UserName;
        public string Status;
        public string StatusDetails;
        public string ProductType;

        public DealItem(Int64 DealId, Consts.eBuySell BuySell, double AmountToExchangeChargedCurrency, double AmountToExchangeCreditedCurrency, string ChargedCurrency,
            string CreditedCurrency, double? Commission, DateTime? ContractDate, double CustomerRate, double? CustomerTotalProfitUSD, bool IsCanceled,
            bool PvPEnabled, bool EnsureOnLinePrice, bool FastTransferEnabled, string UserName,
            string Status, string StatusDetails, Consts.eDealProductType productType)
        {
            this.DealId = DealId;
            this.BuySell = BuySell;
            this.AmountToExchangeChargedCurrency = AmountToExchangeChargedCurrency;
            this.AmountToExchangeCreditedCurrency = AmountToExchangeCreditedCurrency;
            this.ChargedCurrency = ChargedCurrency;
            this.CreditedCurrency = CreditedCurrency;
            this.Commission = Commission;
            this.ContractDate = ContractDate;
            this.CustomerRate = CustomerRate;
            this.CustomerTotalProfitUSD = CustomerTotalProfitUSD;
            this.IsCanceled = IsCanceled;
            this.PvPEnabled = PvPEnabled;
            this.EnsureOnLinePrice = EnsureOnLinePrice;
            this.FastTransferEnabled = FastTransferEnabled;    
            this.UserName = UserName;
            this.Status = Status;
            this.StatusDetails = StatusDetails;
            this.ProductType = (productType == Consts.eDealProductType.FxSimpleExchange) ? "Direct Deal" : "Order Match";
        }
    }

    public class OrderItem
    {
        public Int64 OrderId;
        public Consts.eBuySell BuySell;
        public double AmountToExchangeChargedCurrency;
        public double AmountToExchangeCreditedCurrency;
        public string ChargedCurrency;
        public string CreditedCurrency;
        public double? Commission;
        public DateTime? OrderDate;
        public double? CustomerTotalProfitUSD;
        public string UserName;
        public string Status;
        public string StatusDetails;
        public double? MinimalPartnerExecutionAmountCCY1;
        public DateTime? ExpiryDate;
        public double? AmountCCY1_Executed;
        public double? AmountCCY1_Remainder;
        public bool EnsureOnLinePrice;
        public bool PvPEnabled;
        public string ClearingType;
        public double? MinRate;
        public double? MaxRate;

        public OrderItem(Int64 OrderId, Consts.eBuySell BuySell, double AmountCCY1, double AmountCCY2_Estimation, string Pair,
            double? Commission, DateTime? OrderDate, double? CustomerTotalProfitUSD, string UserName, string Status, string StatusDetails, 
            double? MinimalPartnerExecutionAmountCCY1, DateTime? ExpiryDate, double? AmountCCY1_Executed, double? AmountCCY1_Remainder,
            bool EnsureOnLinePrice, bool PvPEnabled, Consts.eClearingType ClearingType, double? minRate, double? maxRate)
        {
            this.OrderId = OrderId;
            this.BuySell = BuySell;
            this.AmountToExchangeCreditedCurrency = (BuySell == Consts.eBuySell.Buy) ? AmountCCY1 : AmountCCY2_Estimation;
            this.AmountToExchangeChargedCurrency = (BuySell == Consts.eBuySell.Buy) ? AmountCCY2_Estimation : AmountCCY1;
            this.CreditedCurrency = (BuySell == Consts.eBuySell.Buy) ? Pair.Substring(0, 3) : Pair.Substring(3, 3); 
            this.ChargedCurrency = (BuySell == Consts.eBuySell.Buy) ? Pair.Substring(3, 3) : Pair.Substring(0, 3);
            this.Commission = Commission;
            this.OrderDate = OrderDate;
            this.CustomerTotalProfitUSD = CustomerTotalProfitUSD;
            this.UserName = UserName;
            this.Status = Status;
            this.StatusDetails = StatusDetails;
            this.MinimalPartnerExecutionAmountCCY1 = MinimalPartnerExecutionAmountCCY1;
            this.ExpiryDate = ExpiryDate;
            this.AmountCCY1_Executed = AmountCCY1_Executed;
            this.AmountCCY1_Remainder = AmountCCY1_Remainder;
            this.PvPEnabled = PvPEnabled;
            this.EnsureOnLinePrice = EnsureOnLinePrice;
            this.ClearingType = FlatFXResources.Resources.ResourceManager.GetString(ClearingType.ToString());
            this.MinRate = minRate;
            this.MaxRate = maxRate;
        }
    }
}