using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.User;
using System.Threading.Tasks;
using FlatFXWebClient.ViewModels;
using System.Net;
using FlatFXCore.Model.Data;
using System.Data;
using System.Data.Entity;
using FlatFXCore.BussinessLayer;

namespace FlatFXWebClient.Controllers
{
    [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser + "," + Consts.Role_CompanyDemoUser)]
    public class OrderController : BaseController
    {
        public async Task<ActionResult> EditOrder(long orderId)
        {
            OrderViewModel model = new OrderViewModel();
            Order order = db.Orders.Where(o => o.OrderId == orderId).SingleOrDefault();
            if (order == null)
                return RedirectToAction("CreateOrderIndex", model);

            await Initialize(model);
            model.AmountCCY1 = order.AmountCCY1;
            model.BuySell = order.BuySell;
            model.Comment = order.Comment;
            model.ExpiryDate = order.ExpiryDate;
            model.MinimalPartnerExecutionAmountCCY1 = order.MinimalPartnerExecutionAmountCCY1;
            model.PvPEnabled = true; // order.PvPEnabled;
            model.EnsureOnLinePrice = true; // order.EnsureOnLinePrice;
            model.order = order;
            model.OrderId = order.OrderId;
            //model.SelectedAccount = 
            model.Symbol = order.Symbol;
            model.IsEdit = true;
            //model.WorkflowStage = 
            return RedirectToAction("CreateOrderIndex", model);
        }

        public async Task<ActionResult> CreateOrderIndex(OrderViewModel model)
        {
            if (model == null || model.WorkflowStage <= 0)
            {
                model = new OrderViewModel();
                await Initialize(model);
            }
            else
            {
                if (model.order == null)
                    model.order = model.OrderInSession;
            }
            return View("OrderWorkflow", model);
        }
        private async Task<bool> Initialize(OrderViewModel model)
        {
            model.WorkflowStage = 1;
            string error;

            try
            {
                Provider flatFXProvider = db.Providers.Where(p => p.Name == "FlatFX").SingleOrDefault();
                if (flatFXProvider == null)
                    model.InvalidAccountReason.Add("System error. Failed to find provider: 'FlatFX'.");
                else if (!flatFXProvider.IsSimpleExchangeEnabled(out error))
                    model.InvalidAccountReason.Add(error);

                //get User
                string userId = ApplicationInformation.Instance.UserID;
                ApplicationUser user = await db.Users.Include(u => u.Companies).Where(u => u.Id == userId && u.IsActive == true).FirstOrDefaultAsync();
                if (user == null)
                    return false;
                if (!ApplicationInformation.Instance.IsDemoUser && !user.IsApprovedByFlatFX)
                    model.InvalidAccountReason.Add("User is not approved by FlatFX team.");

                //get Company
                List<string> userCompaniesIdList = user.Companies.Where(comp => comp.IsActive == true).Select(c => c.CompanyId).ToList();
                List<string> userCompaniesIdListPlusWhere = user.Companies.Where(comp => comp.IsActive == true && comp.IsSignOnRegistrationAgreement == true)
                    .Select(c => c.CompanyId).ToList();
                if (userCompaniesIdList.Count > 0 && userCompaniesIdListPlusWhere.Count == 0)
                    model.InvalidAccountReason.Add("Company is not signed on registration agreement.");

                //get Company Account
                List<string> companyAccountsIdList = await db.CompanyAccounts.Where(ca => userCompaniesIdList.Contains(ca.CompanyId) && ca.IsActive == true)
                    .Select(ca => ca.CompanyAccountId).ToListAsync();

                //get Provider Accounts
                List<ProviderAccount> providerAccounts = await db.ProviderAccounts.Include(pa => pa.CompanyAccount).Include(p => p.Provider)
                    .Where(pa => companyAccountsIdList.Contains(pa.CompanyAccountId) && pa.IsActive == true).ToListAsync();
                List<ProviderAccount> toRemove = new List<ProviderAccount>();
                foreach (ProviderAccount pa in providerAccounts)
                {
                    if (!pa.ApprovedBYFlatFX)
                    {
                        model.InvalidAccountReason.Add("Account is not approved by FlatFX team.");
                        toRemove.Add(pa);
                        continue;
                    }
                }
                foreach (ProviderAccount pa in toRemove)
                    providerAccounts.Remove(pa);
                ApplicationInformation.Instance.Session["UserBankAccounts"] = providerAccounts.Select(pa => new SelectListItem
                {
                    Value = pa.ProviderId + "_" + pa.CompanyAccountId,
                    Text = pa.CompanyAccount.Company.CompanyName + " - " + pa.AccountName
                });
                model.SelectedAccount = model.UserBankAccounts.First().Value;


                //Initialize order
                Order order = new Order();
                if (providerAccounts.Count > 0 && providerAccounts[0].IsDemoAccount == true)
                    order.IsDemo = true;
                if (ApplicationInformation.Instance.IsUserInRole(Consts.Role_CompanyDemoUser))
                    order.IsDemo = true;
                if (providerAccounts.Count > 0)
                {
                    order.ChargedAccount = providerAccounts[0];
                    order.CreditedAccount = providerAccounts[0];
                }

                if (providerAccounts.Count > 0)
                {
                    order.Provider = providerAccounts[0].Provider;
                    order.ProviderId = order.Provider.ProviderId;
                    order.CompanyAccount = providerAccounts[0].CompanyAccount;
                    order.CompanyAccountId = order.CompanyAccount.CompanyAccountId;
                }

                order.user = user;
                order.UserId = user.Id;

                order.DealProductType = Consts.eDealProductType.FxMidRateOrder;
                order.DealType = Consts.eDealType.Spot;
                order.Status = Consts.eOrderStatus.None;

                model.AmountCCY1 = 0;
                order.AmountCCY1 = model.AmountCCY1;
                model.BuySell = Consts.eBuySell.Buy;
                order.BuySell = model.BuySell;
                model.Symbol = "USDILS";
                order.Symbol = model.Symbol;
                model.ExpiryDate = null;
                order.ExpiryDate = model.ExpiryDate;
                model.MinimalPartnerExecutionAmountCCY1 = null;
                order.MinimalPartnerExecutionAmountCCY1 = model.MinimalPartnerExecutionAmountCCY1;
                model.PvPEnabled = true;
                order.PvPEnabled = true;
                model.EnsureOnLinePrice = true;
                order.EnsureOnLinePrice = true;
                order.MinimalPartnerTotalVolumeUSD = null;
                order.PartnerMinScore = null;
                order.AmountCCY1_Executed = null;
                order.AmountCCY1_Remainder = null;


                ApplicationInformation.Instance.Session[model.OrderKey] = order;

                if (model.InvalidAccountReason.Count == 0)
                    model.InvalidAccountReason = null;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in PromilOrderController::Initialize", ex);
                TempData["ErrorResult"] += "General Error.";
            }

            return true;
        }
        [HttpPost, ActionName("CreateOrderIndex")]
        [ValidateAntiForgeryToken]
        public ActionResult CreateOrderIndexPost(OrderViewModel model)
        {
            if (model == null || model.WorkflowStage < 1 || model.WorkflowStage > 2)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (model.Comment == null)
                model.Comment = "";

            if (model.InvalidAccountReason != null && model.InvalidAccountReason.Count == 1 && model.InvalidAccountReason[0] == "")
                model.InvalidAccountReason = null;

            Order order = model.OrderInSession;
            if (order == null)
            {
                TempData["ErrorResult"] += "Timeout expired. ";
                RedirectToAction("CreateOrderIndex");
            }

            if (!CurrencyManager.Instance.PairList.ContainsKey(model.Symbol))
                TempData["ErrorResult"] += "Invalid currencies combination. ";

            if (model.AmountCCY1 == 0)
                TempData["ErrorResult"] += "Please select amount. ";

            if (model.AmountCCY1 < CurrencyManager.MinDealAmountUSD)
                TempData["ErrorResult"] += "invalid amount. amount > " + CurrencyManager.MinDealAmountUSD.ToString() + ". ";

            if (model.MinimalPartnerExecutionAmountCCY1 != null && model.MinimalPartnerExecutionAmountCCY1 < CurrencyManager.MinDealAmountUSD)
                TempData["ErrorResult"] += "Invalid minimal partner execution amount. amount > " + CurrencyManager.MinDealAmountUSD.ToString() + ". ";

            if (model.MinimalPartnerExecutionAmountCCY1 != null && model.AmountCCY1 < model.MinimalPartnerExecutionAmountCCY1)
                TempData["ErrorResult"] += "Invalid minimal partner execution amount. ";

            if (model.MatchOrderId > 0 && (model.MatchMaxAmount < model.AmountCCY1 || model.MatchMinAmount > model.AmountCCY1))
                TempData["ErrorResult"] += "Invalid partner match amount. Amount range is " + model.MatchMinAmount + " - " + model.MatchMaxAmount;

            if (TempData["ErrorResult"] != null)
                return View("OrderWorkflow", model);

            try
            {
                // to do support this fields on next stage
                //deal.ChargedAccount
                if (model.OrderId > 0) //Edit mode
                {
                    //change only the fields that are relevant for editing order
                    order = db.Orders.Where(o => o.OrderId == model.OrderId).SingleOrDefault();
                    if (order == null)
                    {
                        TempData["ErrorResult"] += "Invalid minimal partner execution amount. ";
                        return View("OrderWorkflow", model);
                    }
                    if (order.Status != Consts.eOrderStatus.Waiting)
                    {
                        TempData["ErrorResult"] += "This order is already triggered and can not be edited. Only 'Waiting' orders can be edited. ";
                        return View("OrderWorkflow", model);
                    }

                    order.BuySell = model.BuySell;
                    order.Symbol = model.Symbol;
                    string txt = order.CompanyAccount.Company.CompanyName;
                    txt = order.ChargedAccount.Provider.FullName;
                    txt = order.ChargedAccount.AccountName;
                    txt = order.user.FullName;
                    order.OrderDate = DateTime.Now; //Request Date

                    if (order.AmountCCY1 != model.AmountCCY1)
                    {
                        order.AmountCCY1 = model.AmountCCY1;
                        order.AmountCCY1_Executed = 0;
                        order.AmountCCY1_Remainder = order.AmountCCY1;
                    }

                    order.Comment = model.Comment;
                    order.ExpiryDate = model.ExpiryDate;
                    order.MinimalPartnerExecutionAmountCCY1 = model.MinimalPartnerExecutionAmountCCY1;
                    order.PvPEnabled = true; // model.PvPEnabled;
                    order.EnsureOnLinePrice = true; // model.EnsureOnLinePrice;
                }
                else // new order or match order
                {
                    //change only the fields that are relevant for match order
                    if (model.MatchOrderId <= 0)
                    {
                        order.BuySell = model.BuySell;
                        order.Symbol = model.Symbol;
                        order.ExpiryDate = model.ExpiryDate;
                        order.MinimalPartnerExecutionAmountCCY1 = model.MinimalPartnerExecutionAmountCCY1;
                    }
                    
                    order.OrderDate = DateTime.Now; //Request Date
                    order.AmountCCY1 = model.AmountCCY1;
                    order.AmountCCY1_Executed = 0;
                    order.AmountCCY1_Remainder = order.AmountCCY1;
                    order.Comment = model.Comment;
                    order.PvPEnabled = true; // model.PvPEnabled;
                    order.EnsureOnLinePrice = true; // model.EnsureOnLinePrice;
                }

                double extraCharge = 0;
                //if (order.EnsureOnLinePrice)
                //    extraCharge += CurrencyManager.ExtraCharge_EnsureOnLinePrice;
                if (order.PvPEnabled)
                    extraCharge += CurrencyManager.ExtraCharge_PvPEnabled;

                //calculate AmountUSD_Estimation (volume)
                if (order.CCY1 == "USD")
                    order.AmountUSD_Estimation = order.AmountCCY1;
                else
                    order.AmountUSD_Estimation = CurrencyManager.Instance.GetAmountUSD(order.CCY1, order.AmountCCY1);
                order.AmountCCY2_Estimation = CurrencyManager.Instance.TranslateAmount(order.AmountCCY1, order.CCY1, order.CCY2);

                //calculate profit
                string minorCurrency = order.CCY2;
                order.CustomerTotalProfitUSD_Estimation = Math.Round((order.AmountUSD_Estimation * (CurrencyManager.CustomerOrderProfit - extraCharge)) - CurrencyManager.TransactionFeeUSD, 2); // 9 promil
                order.FlatFXCommissionUSD_Estimation = Math.Round((order.AmountUSD_Estimation * CurrencyManager.Instance.GetOrderCommission(order.AmountUSD_Estimation)), 2); // 2 promil

                /*if (order.BuySell == Consts.eBuySell.Buy)
                    order.AmountCCY2_Estimation += CurrencyManager.Instance.TranslateAmount(order.FlatFXCommissionUSD_Estimation.Value, "USD", order.CCY2);
                else
                    order.AmountCCY2_Estimation -= CurrencyManager.Instance.TranslateAmount(order.FlatFXCommissionUSD_Estimation.Value, "USD", order.CCY2);
                */
                model.order = order;

                if (model.OrderId == 0)
                {
                    db.Orders.Attach(order);
                    db.Entry(order).State = EntityState.Added;
                }
                db.SaveChanges();

                model.OrderId = order.OrderId;
                model.WorkflowStage = 2;
                ApplicationInformation.Instance.Session[model.OrderKey] = order;

                FXRate pairRate = CurrencyManager.Instance.PairRates[order.Symbol];
                model.MatchMidRate = pairRate.Mid;
                
                return RedirectToAction("CreateOrderIndex", model);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in PromilOrderController::OrderIndexPost", ex);
                TempData["ErrorResult"] += "General Error. ";
            }

            return View("OrderWorkflow", model);
        }
        [HttpPost, ActionName("Confim")]
        [ValidateAntiForgeryToken]
        public ActionResult Confim(OrderViewModel model)
        {
            try
            {
                if (model == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (model.InvalidAccountReason != null && model.InvalidAccountReason.Count == 1 && model.InvalidAccountReason[0] == "")
                    model.InvalidAccountReason = null;

                model.order = model.OrderInSession;
                if (model.OrderId != model.OrderInSession.OrderId)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Order order = db.Orders.Find(model.OrderInSession.OrderId);

                if (DateTime.Now > order.OrderDate.AddSeconds(60))
                {
                    TempData["ErrorResult"] = "Timeout expired";
                    return RedirectToAction("CreateOrderIndex", model);
                }

                order.Status = Consts.eOrderStatus.Waiting;
                db.SaveChanges();

                model.OrderId = order.OrderId;
                model.order = order;
                model.WorkflowStage = 3;

                if (model.MatchOrderId > 0)
                {
                    //Match 2 orders
                    OrderMatch match = new OrderMatch();
                    match.MidRate = model.MatchMidRate;
                    
                    bool isValid = InitializeMatch(match, order.OrderId, model.MatchOrderId, Consts.eMatchTriggerSource.Order1);
                    if (!isValid || match.ErrorMessage != null)
                    {
                        TempData["ErrorResult"] += "Match falied. Please contact FlatFX Team. Details: " + match.ErrorMessage;
                    }
                    else
                    {
                        db.OrderMatches.Add(match);
                        db.SaveChanges();
                    }

                    if (order.IsDemo)
                    {
                        model.MatchMyName = match.Deal1.user.FullName + " @ Demo Company";
                        model.MatchPartnerAccount = match.Deal2.Provider.Name + " - 222-222222";
                        model.MatchPartnerName = " Anonymous @ Demo Company";
                    }
                    else
                    {
                        model.MatchMyName = match.Deal1.user.FullName + " @ " + match.Deal1.CompanyAccount.Company.CompanyName;
                        model.MatchPartnerAccount = match.Deal2.Provider.Name + " - " + match.Deal2.ChargedAccount.BankBranchNumber + "-" + match.Deal2.ChargedAccount.BankAccountNumber;
                        model.MatchPartnerName = match.Deal2.user.FullName + " @ " + match.Deal2.CompanyAccount.Company.CompanyName;
                    }

                    TempData["Deal"] = match.Deal1;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in PromilOrderController::Confim", ex);
                TempData["ErrorResult"] += "General Error. Please contact FlatFX Team.";
            }

            return RedirectToAction("CreateOrderIndex", model);
        }
        public ActionResult OrderData(string mode)
        {
            if (mode == "OrderHistory")
                TempData["mode"] = "OrderHistory";
            else
                TempData["mode"] = "OpenOrders";
            return View();
        }
        public async Task<ActionResult> NewOrderWithMatchMin(long matchOrderId)
        {
            return await NewOrderWithMatch(matchOrderId, 0);
        }
        public async Task<ActionResult> NewOrderWithMatchMax(long matchOrderId)
        {
            return await NewOrderWithMatch(matchOrderId, 1);
        }
        private async Task<ActionResult> NewOrderWithMatch(long matchOrderId, int action)
        {
            OrderViewModel model = new OrderViewModel();
            Order matchedOrder = await db.Orders.Where(o => o.OrderId == matchOrderId).SingleOrDefaultAsync();
            if (matchedOrder == null)
                return RedirectToAction("CreateOrderIndex", model);

            await Initialize(model);
            if (action == 0) //min
                model.AmountCCY1 = matchedOrder.MinimalPartnerExecutionAmountCCY1.HasValue ? matchedOrder.MinimalPartnerExecutionAmountCCY1.Value : matchedOrder.AmountCCY1_Remainder.Value;
            else
                model.AmountCCY1 = matchedOrder.AmountCCY1_Remainder.Value;
            model.BuySell = (matchedOrder.BuySell == Consts.eBuySell.Buy) ? Consts.eBuySell.Sell : Consts.eBuySell.Buy;
            model.Symbol = matchedOrder.Symbol;
            model.ExpiryDate = DateTime.Now.AddHours(2);
            (ApplicationInformation.Instance.Session[model.OrderKey] as Order).BuySell = model.BuySell;
            (ApplicationInformation.Instance.Session[model.OrderKey] as Order).Symbol = model.Symbol;
            (ApplicationInformation.Instance.Session[model.OrderKey] as Order).ExpiryDate = model.ExpiryDate;

            model.MatchOrderId = matchOrderId;
            model.MatchMinAmount = matchedOrder.MinimalPartnerExecutionAmountCCY1.HasValue ? matchedOrder.MinimalPartnerExecutionAmountCCY1.Value : matchedOrder.AmountCCY1_Remainder.Value;
            model.MatchMaxAmount = matchedOrder.AmountCCY1_Remainder.Value;

            return RedirectToAction("CreateOrderIndex", model);
        }

        #region Order Match
        public bool InitializeMatch(OrderMatch match, long orderId1, long orderId2, Consts.eMatchTriggerSource source)
        {
            try
            {
                match.ErrorMessage = null;

                match.Order1 = db.Orders.Find(orderId1);
                match.Order2 = db.Orders.Find(orderId2);

                if (match.Order1 == null)
                    match.ErrorMessage += "Order " + match.Order1.OrderId + " not exists" + Environment.NewLine;
                if (match.Order2 == null)
                    match.ErrorMessage += "Order " + match.Order2.OrderId + " not exists" + Environment.NewLine;

                match.TriggerSource = source;

                //validate the order types
                if (match.Order1.Symbol != match.Order2.Symbol)
                    match.ErrorMessage += "Orders symbol is different." + Environment.NewLine;
                if (match.Order1.BuySell == match.Order2.BuySell)
                    match.ErrorMessage += "Orders Buy/Sell is same direction." + Environment.NewLine;

                //validate the orders state
                if (match.Order1.Status != Consts.eOrderStatus.Waiting && match.Order1.Status != Consts.eOrderStatus.Triggered_partially)
                    match.ErrorMessage += "Order " + match.Order1.OrderId + "(" + match.Order1.BuySell.ToString() + " " + match.Order1.AmountCCY1_Remainder + " " + match.Order1.CCY1 + ") is not longer valid." + Environment.NewLine;

                if (match.Order2.Status != Consts.eOrderStatus.Waiting && match.Order2.Status != Consts.eOrderStatus.Triggered_partially)
                    match.ErrorMessage += "Order " + match.Order2.OrderId + "(" + match.Order2.BuySell.ToString() + " " + match.Order2.AmountCCY1_Remainder + " " + match.Order2.CCY1 + ") is not longer valid." + Environment.NewLine;

                //validate the orders amount
                if (match.TriggerSource != Consts.eMatchTriggerSource.Automatic)
                {
                    Order activeOrder = (match.TriggerSource == Consts.eMatchTriggerSource.Order1) ? match.Order1 : match.Order2;
                    Order passiveOrder = (match.TriggerSource == Consts.eMatchTriggerSource.Order1) ? match.Order2 : match.Order1;
                    if (activeOrder.AmountCCY1_Remainder > passiveOrder.AmountCCY1_Remainder)
                        match.ErrorMessage += "Order " + activeOrder.OrderId + "(" + activeOrder.BuySell.ToString() + " " + activeOrder.AmountCCY1_Remainder + " " + activeOrder.CCY1 + ") amount is too high." + Environment.NewLine;
                    if (activeOrder.AmountCCY1_Remainder < passiveOrder.MinimalPartnerExecutionAmountCCY1)
                        match.ErrorMessage += "Order " + activeOrder.OrderId + "(" + activeOrder.BuySell.ToString() + " " + activeOrder.AmountCCY1_Remainder + " " + activeOrder.CCY1 + ") amount is too low." + Environment.NewLine;
                }

                //Validate the match is not exists
                OrderMatch temp = db.OrderMatches.Where(m => m.Order1.OrderId == match.Order1.OrderId && m.Order2.OrderId == match.Order2.OrderId && (m.Status == Consts.eMatchStatus.New || m.Status == Consts.eMatchStatus.Opened)).SingleOrDefault();
                if (temp != null)
                    match.ErrorMessage += "Match already exists." + Environment.NewLine;

                match.TriggerDate = DateTime.Now;
                match.MaturityDate = ApplicationInformation.Instance.NextBussinessDay(12, 0);
                match.Status = Consts.eMatchStatus.New;

                FXRate pairRate = CurrencyManager.Instance.PairRates[match.Order1.Symbol];
                if (!match.Order1.IsDemo && ((DateTime.Now - pairRate.LastUpdate).TotalMinutes > 5 || !pairRate.IsTradable)) //the exchange rate is not up to date
                    match.ErrorMessage += "The Exchange Rate is not up to date. Please contact FlatFX Support.";

                //check the MidRate was not changed manually
                if (match.MidRate.Value > (pairRate.Mid * 1.001) || match.MidRate.Value < (pairRate.Mid * 0.999))
                    match.ErrorMessage += "The Exchange Rate is not up to date. Please contact FlatFX Support.";

                match.Deal1 = GenerateDeal(match, match.Order1, match.MidRate.Value, match.Order1.AmountCCY1);
                match.Deal2 = GenerateDeal(match, match.Order2, match.MidRate.Value, match.Order1.AmountCCY1);

                //Change Orders info
                if (match.TriggerSource != Consts.eMatchTriggerSource.Automatic)
                {
                    Order activeOrder = (match.TriggerSource == Consts.eMatchTriggerSource.Order1) ? match.Order1 : match.Order2;
                    Order passiveOrder = (match.TriggerSource == Consts.eMatchTriggerSource.Order1) ? match.Order2 : match.Order1;

                    activeOrder.AmountCCY1_Executed = activeOrder.AmountCCY1_Remainder;
                    activeOrder.AmountCCY1_Remainder = 0;
                    activeOrder.Status = Consts.eOrderStatus.Triggered;

                    if (passiveOrder.AmountCCY1_Remainder == activeOrder.AmountCCY1)
                    {
                        passiveOrder.AmountCCY1_Executed += activeOrder.AmountCCY1;
                        passiveOrder.AmountCCY1_Remainder = 0;
                        passiveOrder.Status = Consts.eOrderStatus.Triggered;
                    }
                    else if (passiveOrder.AmountCCY1_Remainder > activeOrder.AmountCCY1)
                    {
                        double AmountCCY1_Remainder = passiveOrder.AmountCCY1_Remainder.Value - activeOrder.AmountCCY1;
                        if (AmountCCY1_Remainder > CurrencyManager.MinDealAmountUSD && passiveOrder.MinimalPartnerExecutionAmountCCY1.HasValue &&
                            passiveOrder.MinimalPartnerExecutionAmountCCY1.Value < AmountCCY1_Remainder)
                        {
                            passiveOrder.AmountCCY1_Executed += activeOrder.AmountCCY1;
                            passiveOrder.AmountCCY1_Remainder = AmountCCY1_Remainder;
                            passiveOrder.Status = Consts.eOrderStatus.Triggered_partially;
                        }
                        else
                        {
                            //not all the amount was executed but the rest is less than minimum
                            passiveOrder.AmountCCY1_Executed += activeOrder.AmountCCY1;
                            passiveOrder.AmountCCY1_Remainder = passiveOrder.AmountCCY1 - passiveOrder.AmountCCY1_Executed;
                            passiveOrder.Status = Consts.eOrderStatus.Triggered;

                            //send email that the order was closed automatically ??? to do ???
                        }
                    }
                    else
                    {
                        //Error
                        Logger.Instance.WriteError("Failed in OrderMatch::Initialize. passiveOrder.AmountCCY1_Remainder < activeOrder.AmountCCY1. Order Id: " + orderId1);
                    }
                }

                if (match.ErrorMessage != null)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in OrderMatch::Initialize", ex);
                match.ErrorMessage += "General Error";
                match.MatchId = 0;
                return false;
            }
        }
        private Deal GenerateDeal(OrderMatch match, Order order, double matchMidRate, double AmountCCY1)
        {
            try
            {
                //Initialize deal
                Deal deal = new Deal();
                deal.IsDemo = order.IsDemo;
                deal.DealProductType = Consts.eDealProductType.FxMidRateOrder;
                deal.DealType = Consts.eDealType.Spot;
                deal.IsOffer = false;
                deal.Status = Consts.eDealStatus.New;
                deal.EnsureOnLinePrice = true;
                deal.PvPEnabled = true;
                deal.FastTransferEnabled = false;
                deal.ChargedAccount = order.ChargedAccount;
                deal.CreditedAccount = order.CreditedAccount;
                deal.Provider = order.Provider;
                deal.ProviderId = order.Provider.ProviderId;
                deal.CompanyAccount = order.CompanyAccount;
                deal.CompanyAccountId = order.CompanyAccount.CompanyAccountId;
                deal.user = order.user;
                deal.UserId = order.user.Id;

                deal.BuySell = order.BuySell;
                deal.Symbol = order.Symbol;
                //Consts.eBidAsk priceBidOrAsk = Consts.eBidAsk.Bid;
                //if (deal.BuySell == Consts.eBuySell.Buy)
                //    priceBidOrAsk = Consts.eBidAsk.Ask;

                deal.OfferingDate = DateTime.Now;
                deal.OfferingMidRate = matchMidRate;
                deal.MidRate = matchMidRate;

                //deal.EnsureOnLinePrice = (order.PvPEnabled) ? true : order.EnsureOnLinePrice;
                //deal.PvPEnabled = order.PvPEnabled;
                //deal.FastTransferEnabled = false;

                double extraCharge = 0;
                //if (deal.EnsureOnLinePrice)
                //    extraCharge += CurrencyManager.ExtraCharge_EnsureOnLinePrice;
                if (deal.PvPEnabled)
                    extraCharge += CurrencyManager.ExtraCharge_PvPEnabled;

                double amountUSD = 0;
                if (order.CCY1 == "USD")
                    amountUSD = AmountCCY1;
                else
                    amountUSD = CurrencyManager.Instance.GetAmountUSD(order.CCY1, AmountCCY1);

                deal.CustomerRate = matchMidRate;
                deal.BankRate = 0;

                /*if (priceBidOrAsk == Consts.eBidAsk.Bid)
                {
                    deal.CustomerRate = matchMidRate -((CurrencyManager.Instance.GetOrderCommission(amountUSD) + extraCharge) * matchMidRate);
                    deal.BankRate = 0; // matchMidRate - (CurrencyManager.BankCommission * matchMidRate);
                }
                else
                {
                    deal.CustomerRate = matchMidRate + ((CurrencyManager.Instance.GetOrderCommission(amountUSD) + extraCharge) * matchMidRate);
                    deal.BankRate = 0; // matchMidRate + (CurrencyManager.BankCommission * matchMidRate);
                }*/

                if (order.BuySell == Consts.eBuySell.Buy)
                {
                    deal.AmountToExchangeCreditedCurrency = AmountCCY1;
                    deal.AmountToExchangeChargedCurrency = deal.CustomerRate * AmountCCY1;
                    deal.CreditedCurrency = order.CCY1;
                    deal.ChargedCurrency = order.CCY2;
                }
                else
                {
                    deal.AmountToExchangeChargedCurrency = AmountCCY1;
                    deal.AmountToExchangeCreditedCurrency = deal.CustomerRate * AmountCCY1;
                    deal.CreditedCurrency = order.CCY2;
                    deal.ChargedCurrency = order.CCY1;
                }

                //calculate AmountUSD (volume)
                if (deal.CreditedCurrency == "USD")
                    deal.AmountUSD = deal.AmountToExchangeCreditedCurrency;
                else if (deal.ChargedCurrency == "USD")
                    deal.AmountUSD = deal.AmountToExchangeChargedCurrency;
                else
                    deal.AmountUSD = CurrencyManager.Instance.GetAmountUSD(deal.CreditedCurrency, deal.AmountToExchangeCreditedCurrency);

                //calculate profit
                string minorCurrency = order.CCY2;
                deal.BankIncomeUSD = 0; // Math.Round(CurrencyManager.Instance.GetAmountUSD(minorCurrency, deal.AmountUSD * CurrencyManager.BankCommission * matchMidRate), 2); // 0.5 promil
                deal.CustomerTotalProfitUSD = Math.Round(CurrencyManager.Instance.GetAmountUSD(minorCurrency, deal.AmountUSD * (CurrencyManager.CustomerOrderProfit - extraCharge) * matchMidRate) - CurrencyManager.TransactionFeeUSD, 2); // 9 promil
                deal.FlatFXIncomeUSD = Math.Round((deal.AmountUSD * CurrencyManager.Instance.GetOrderCommission(deal.AmountUSD)), 2); // 2 promil
                deal.Commission = deal.BankIncomeUSD + deal.FlatFXIncomeUSD; // 2 Promil

                deal.Comment = order.Comment;
                deal.ContractDate = DateTime.Now;
                deal.MaturityDate = ApplicationInformation.Instance.NextBussinessDay(12, 0);
                deal.Status = Consts.eDealStatus.New;
                return deal;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in OrderMatch::GenerateDeal", ex);
                match.ErrorMessage += "Failed to generate Deal from Order.";
                return null;
            }
        }
        #endregion
    }
}