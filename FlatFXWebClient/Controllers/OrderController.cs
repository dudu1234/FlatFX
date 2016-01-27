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
        public const double BankProfitInPromil = 0.001;
        public const double FlatFXProfitInPromil = 0.002;
        public const double CustomerProfitInPromil = 0.008;

        public async Task<ActionResult> OrderIndex(OrderViewModel model)
        {
            if (model.WorkflowStage <= 0)
            {
                model = new OrderViewModel();
                await Initialize(model);
            }
            else
            {
                if (model.order == null)
                    model.order = model.OrderInSession;
            }
            return View(model);
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
                    order.ChargedProviderAccount = providerAccounts[0];
                    order.CreditedProviderAccount = providerAccounts[0];
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

                order.DealProductType = Consts.eDealProductType.FxPromilOrder;
                order.DealType = Consts.eDealType.Spot;
                order.IsClosed = false;
                order.IsWaiting = false;

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
                model.PromilRequired = 0;
                order.PromilRequired = model.PromilRequired;

                order.IsConfirmed = false;
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
        [HttpPost, ActionName("OrderIndex")]
        [ValidateAntiForgeryToken]
        public ActionResult OrderIndexPost(OrderViewModel model)
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
                RedirectToAction("OrderIndex");
            }

            if (!CurrencyManager.Instance.PairList.ContainsKey(model.Symbol))
                TempData["ErrorResult"] += "Invalid currencies combination. ";

            if (TempData["ErrorResult"] != null)
                return View(model);

            try
            {
                // to do support this fields on next stage
                //deal.ChargedProviderAccount

                order.BuySell = model.BuySell;
                order.Symbol = model.Symbol;
                FXRate pairRate = CurrencyManager.Instance.PairRates[order.Symbol];
                order.OrderDate = DateTime.Now; //Request Date
                order.AmountCCY1 = model.AmountCCY1;
                order.AmountCCY1_Executed = 0;
                order.AmountCCY1_Remainder = order.AmountCCY1;
                order.Comment = model.Comment;
                order.ExpiryDate = model.ExpiryDate;
                order.MinimalPartnerExecutionAmountCCY1 = model.MinimalPartnerExecutionAmountCCY1;
                order.PromilRequired = model.PromilRequired;

                //calculate AmountUSD_Estimation (volume)
                if (order.CCY1 == "USD")
                    order.AmountUSD_Estimation = order.AmountCCY1;
                else
                    order.AmountUSD_Estimation = CurrencyManager.Instance.GetAmountUSD(order.CCY1, order.AmountCCY1);
                order.AmountCCY2_Estimation = CurrencyManager.Instance.TranslateAmount(order.AmountCCY1, order.CCY1, order.CCY2);
                
                //calculate profit
                string minorCurrency = order.CCY2;
                // 0.001 * 2 - constant FlatFX commision
                // 0.011 - constant Bank full commission
                // 0.001 * order.PromilRequired - Promil required
                //order.CustomerTotalProfitUSD_Estimation = Math.Round(CurrencyManager.Instance.GetAmountUSD(minorCurrency, (order.AmountUSD_Estimation * (0.011 - (0.001 * order.PromilRequired) - (0.001 * 2)) * pairRate.Mid) - 11), 2);
                order.CustomerTotalProfitUSD_Estimation = Math.Round( (order.AmountUSD_Estimation * (0.011 - (0.001 * order.PromilRequired) - (0.001 * 2))) - 11, 2);
                order.FlatFXCommissionUSD_Estimation = Math.Round( (order.AmountUSD_Estimation * (FlatFXProfitInPromil + BankProfitInPromil)), 2); // 3 promil
                
                model.order = order;

                db.Orders.Attach(order);
                db.Entry(order).State = EntityState.Added;
                db.SaveChanges();

                model.OrderId = order.OrderId;
                model.WorkflowStage = 2;

                ApplicationInformation.Instance.Session[model.OrderKey] = order;
                return RedirectToAction("OrderIndex", model);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in PromilOrderController::OrderIndexPost", ex);
                TempData["ErrorResult"] += "General Error. ";
            }

            return View(model);
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

                order.IsWaiting = true;
                order.IsConfirmed = true;
                db.SaveChanges();

                model.OrderId = order.OrderId;
                model.order = order;
                model.WorkflowStage = 3;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in PromilOrderController::Confim", ex);
                TempData["ErrorResult"] += "General Error. Please contact FlatFX Team.";
            }

            return RedirectToAction("OrderIndex", model);
        }
    }
}