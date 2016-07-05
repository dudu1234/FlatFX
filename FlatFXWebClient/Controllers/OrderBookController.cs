using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Data;
using FlatFXCore.Model.User;
using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace FlatFXWebClient.Controllers
{
    [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser + "," + Consts.Role_CompanyDemoUser)]
    public class OrderBookController : BaseController
    {
        public ActionResult List()
        {
            return View("OrderBookIndex");
        }
        public async Task<ActionResult> LoadData(string key)
        {
            string error = null;
            try
            {
                if (key == null || key == "" || key == "undefined" || key.Length != 6)
                    key = "USDILS";

                bool isDemo = ApplicationInformation.Instance.IsDemoUser;
                string userId = ApplicationInformation.Instance.UserID;
                ApplicationUser user = await db.Users.Include(u => u.Companies).Where(u => u.Id == userId && u.IsActive == true).FirstOrDefaultAsync();
                string companyId = user.Companies.First().CompanyId;

                if (!isDemo && !user.IsApprovedByFlatFX)
                    error = "User is not approved by FlatFX team.";

                bool isAdmin = ApplicationInformation.Instance.IsAdministrator;

                if (User.IsInRole(Consts.Role_Administrator))
                    isAdmin = true;

                double midRate = CurrencyManager.Instance.PairRates[key].Mid;

                List<string> customerProviders = await 
                    (from companyAcc in db.CompanyAccounts
                    join providerAcc in db.ProviderAccounts on companyAcc.CompanyAccountId equals providerAcc.CompanyAccountId 
                    where companyAcc.IsActive == true
                    where providerAcc.IsActive == true
                    where providerAcc.ApprovedBYFlatFX == true
                    select providerAcc.ProviderId).ToListAsync<string>();

                List<Order> ordersToBuy2 = await db.Orders
                    .Where(o => o.Symbol == key && o.BuySell == Consts.eBuySell.Buy && (o.Status == Consts.eOrderStatus.Triggered_partially || o.Status == Consts.eOrderStatus.Waiting) &&
                        o.IsDemo == isDemo && (o.IsDemo || o.CompanyAccount.Company.CompanyId != companyId) && (!o.MinRate.HasValue || o.MinRate <= midRate) && (!o.MaxRate.HasValue || o.MaxRate >= midRate)).ToListAsync();
                List<OrderBookItem> ordersToBuy = ordersToBuy2.Where(o => (!o.ExpiryDate.HasValue || o.ExpiryDate > DateTime.Now) && customerProviders.Contains(o.ProviderId))
                    .Select(o => new OrderBookItem(o.OrderId,
                        o.AmountCCY1_Remainder.HasValue ? o.AmountCCY1_Remainder.Value : 0,
                        o.MinimalPartnerExecutionAmountCCY1.HasValue ? o.MinimalPartnerExecutionAmountCCY1.Value : o.AmountCCY1_Remainder.Value,
                        (isAdmin) ? o.CompanyAccount.Company.CompanyName : "", o.Provider.Name, o.ClearingType, o.MinRate, o.MaxRate, o.ExpiryDate))
                    .ToList();


                List<Order> ordersToSell2 = await db.Orders
                    .Where(o => o.Symbol == key && o.BuySell == Consts.eBuySell.Sell && (o.Status == Consts.eOrderStatus.Triggered_partially || o.Status == Consts.eOrderStatus.Waiting) &&
                        o.IsDemo == isDemo && (o.IsDemo || o.CompanyAccount.Company.CompanyId != companyId) && (!o.MinRate.HasValue || o.MinRate <= midRate) && (!o.MaxRate.HasValue || o.MaxRate >= midRate)).ToListAsync();
                List<OrderBookItem> ordersToSell = ordersToSell2.Where(o => (!o.ExpiryDate.HasValue || o.ExpiryDate > DateTime.Now) && customerProviders.Contains(o.ProviderId))
                    .Select(o => new OrderBookItem(o.OrderId,
                        o.AmountCCY1_Remainder.HasValue ? o.AmountCCY1_Remainder.Value : 0,
                        o.MinimalPartnerExecutionAmountCCY1.HasValue ? o.MinimalPartnerExecutionAmountCCY1.Value : o.AmountCCY1_Remainder.Value,
                        (isAdmin) ? o.CompanyAccount.Company.CompanyName : "", o.Provider.Name, o.ClearingType, o.MinRate, o.MaxRate, o.ExpiryDate))
                    .ToList();

                Dictionary<string, string> pairs = CurrencyManager.Instance.PairRates.Values
                    .Where(pr => pr.IsActive && pr.IsActiveForSimpleTrading && pr.IsTradable)
                    .ToDictionary(pr => pr.Key, pr => FlatFXResources.Resources.ResourceManager.GetString(pr.Key, FlatFXResources.Resources.Culture));
                
                string keyDisplay = FlatFXResources.Resources.ResourceManager.GetString(key, FlatFXResources.Resources.Culture);

                if (error != null)
                {
                    return Json(new { Error = error }, JsonRequestBehavior.AllowGet);
                }

                ActionResult res = null;
                if (pairs.Count == 0)
                {
                    res = Json(new
                    {
                        Pairs = pairs,
                        Key = "",
                        MidRate = 0,
                        KeyDisplay = "",
                        OrdersToBuy = new List<OrderBookItem>(),
                        OrdersToSell = new List<OrderBookItem>()
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    res = Json(new
                    {
                        Pairs = pairs,
                        Key = key,
                        MidRate = midRate,
                        KeyDisplay = keyDisplay,
                        OrdersToBuy = ordersToBuy,
                        OrdersToSell = ordersToSell
                    }, JsonRequestBehavior.AllowGet);
                }

                return res;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in GetOrderBook", ex);
                return null;
            }
        }
    }
    public class OrderBookItem
    {
        public long OrderId;
        public string CompanyName;
        public double MinAmount;
        public double MaxAmount;
        public string BankName;
        public string ClearingTypeTxt;
        public string RateRangeTxt;
        public string ExpiredTxt;

        public OrderBookItem(long OrderId, double MaxAmount, double MinAmount, string CompanyName, string BankName, Consts.eClearingType ClearingType, double? minRate, double? maxRate, DateTime? Expired)
        {
            this.OrderId = OrderId;
            this.CompanyName = CompanyName;
            this.MinAmount = MinAmount;
            this.MaxAmount = MaxAmount;
            this.BankName = BankName;
            this.ClearingTypeTxt = FlatFXResources.Resources.ResourceManager.GetString(ClearingType.ToString());
            string RateRangeTxt = "-";
            if (minRate.HasValue && maxRate.HasValue)
                RateRangeTxt = minRate + " - " + maxRate;
            else if (minRate.HasValue)
                RateRangeTxt = minRate + " < Rate";
            else if (maxRate.HasValue)
                RateRangeTxt = maxRate + " > Rate";

            ExpiredTxt = (Expired.HasValue ? Expired.Value.ToString("dd/MM/yyyy HH:mm") : "GTC");
        }
    }
}