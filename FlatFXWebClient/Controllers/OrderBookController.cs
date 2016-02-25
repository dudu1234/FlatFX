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

namespace FlatFXWebClient.Controllers
{
    [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser + "," + Consts.Role_CompanyDemoUser)]
    public class OrderBookController : BaseController
    {
        public ActionResult OrderBookIndex(string key)
        {
            string error = null;
            try
            {
                if (key == null || key == "" || key == "undefined" || key.Length != 6)
                    key = "USDILS";

                bool isDemo = ApplicationInformation.Instance.IsDemoUser;
                string userId = ApplicationInformation.Instance.UserID;
                ApplicationUser user = db.Users.Include(u => u.Companies).Where(u => u.Id == userId && u.IsActive == true).FirstOrDefault();
                string companyId = user.Companies.First().CompanyId;

                if (!isDemo && !user.IsApprovedByFlatFX)
                    error = "User is not approved by FlatFX team.";

                List<OrderBookItem> ordersToBuy = db.Orders
                    .Where(o => o.Symbol == key && o.BuySell == Consts.eBuySell.Buy && (o.Status == Consts.eOrderStatus.Triggered_partially || o.Status == Consts.eOrderStatus.Waiting) &&
                        o.IsDemo == isDemo && (o.IsDemo || o.CompanyAccount.Company.CompanyId != companyId))
                    .ToList()
                    .Where(o => (!o.ExpiryDate.HasValue || o.ExpiryDate <= DateTime.Now))
                    .Select(o => new OrderBookItem(o.OrderId,
                        o.AmountCCY1_Remainder.HasValue ? o.AmountCCY1_Remainder.Value : 0,
                        o.MinimalPartnerExecutionAmountCCY1.HasValue ? o.MinimalPartnerExecutionAmountCCY1.Value : o.AmountCCY1_Remainder.Value, 
                        o.CompanyAccount.Company.CompanyName))
                    .ToList();

                List<OrderBookItem> ordersToSell = db.Orders
                    .Where(o => o.Symbol == key && o.BuySell == Consts.eBuySell.Sell && (o.Status == Consts.eOrderStatus.Triggered_partially || o.Status == Consts.eOrderStatus.Waiting) &&
                        o.IsDemo == isDemo && (o.IsDemo || o.CompanyAccount.Company.CompanyId != companyId))
                    .ToList()
                    .Where(o => (!o.ExpiryDate.HasValue || o.ExpiryDate <= DateTime.Now))
                    .Select(o => new OrderBookItem(o.OrderId,
                        o.AmountCCY1_Remainder.HasValue ? o.AmountCCY1_Remainder.Value : 0,
                        o.MinimalPartnerExecutionAmountCCY1.HasValue ? o.MinimalPartnerExecutionAmountCCY1.Value : o.AmountCCY1_Remainder.Value, 
                        o.CompanyAccount.Company.CompanyName))
                    .ToList();

                Dictionary<string, string> pairs = CurrencyManager.Instance.PairRates.Values
                    .Where(pr => pr.IsActive && pr.IsActiveForSimpleTrading && pr.IsTradable)
                    .ToDictionary(pr => pr.Key, pr => FlatFXResources.Resources.ResourceManager.GetString(pr.Key, FlatFXResources.Resources.Culture));
                double midRate = CurrencyManager.Instance.PairRates[key].Mid;
                string keyDisplay = FlatFXResources.Resources.ResourceManager.GetString(key, FlatFXResources.Resources.Culture);

                if (error != null)
                {
                    return Json(new { Error = error }, JsonRequestBehavior.AllowGet);
                }

                ActionResult res = Json(new
                {
                    Pairs = pairs,
                    Key = key,
                    MidRate = midRate,
                    KeyDisplay = keyDisplay,
                    OrdersToBuy = ordersToBuy,
                    OrdersToSell = ordersToSell
                }, JsonRequestBehavior.AllowGet);

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

        public OrderBookItem(long OrderId, double MaxAmount, double MinAmount, string CompanyName)
        {
            this.OrderId = OrderId;
            this.CompanyName = CompanyName;
            this.MinAmount = MinAmount;
            this.MaxAmount = MaxAmount;
        }
    }
}