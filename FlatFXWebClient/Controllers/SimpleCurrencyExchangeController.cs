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
    public class SimpleCurrencyExchangeController : BaseController
    {
        public async Task<ActionResult> StartTrade(string key, string direction)
        {
            SimpleCurrencyExchangeViewModel model = new SimpleCurrencyExchangeViewModel();
            await Initialize(model);
            if (direction != "bid")
            {
                model.BuySell = Consts.eBuySell.Buy;
                model.CCY1 = key.Substring(0, 3);
                model.CCY2 = key.Substring(3, 3);
            }
            else
            {
                model.BuySell = Consts.eBuySell.Buy;
                model.CCY2 = key.Substring(0, 3);
                model.CCY1 = key.Substring(3, 3);
            }
            
            return RedirectToAction("EnterData", model);
        }

        public async Task<ActionResult> EnterData(SimpleCurrencyExchangeViewModel model)
        {
            if (model.WorkflowStage <= 0)
            {
                model = new SimpleCurrencyExchangeViewModel();
                await Initialize(model);
            }
            else
            {
                if (model.deal == null)
                    model.deal = model.DealInSession;
            }
            return View(model);
        }
        private async Task<bool> Initialize(SimpleCurrencyExchangeViewModel model)
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

                model.CCY1 = "USD";
                model.CCY2 = "ILS";

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


                //Initialize deal
                Deal deal = new Deal();
                if (providerAccounts.Count > 0 && providerAccounts[0].IsDemoAccount == true)
                    deal.IsDemo = true;
                if (ApplicationInformation.Instance.IsUserInRole(Consts.Role_CompanyDemoUser))
                    deal.IsDemo = true;
                if (providerAccounts.Count > 0)
                {
                    deal.ChargedAccount = providerAccounts[0];
                    deal.CreditedAccount = providerAccounts[0];
                }
                deal.DealProductType = Consts.eDealProductType.FxSimpleExchange;
                deal.DealType = Consts.eDealType.Spot;
                deal.IsOffer = true;
                deal.Status = Consts.eDealStatus.None;
                if (providerAccounts.Count > 0)
                {
                    deal.Provider = providerAccounts[0].Provider;
                    deal.ProviderId = deal.Provider.ProviderId;
                    deal.CompanyAccount = providerAccounts[0].CompanyAccount;
                    deal.CompanyAccountId = deal.CompanyAccount.CompanyAccountId;
                }
                
                deal.user = user;
                deal.UserId = user.Id;

                ApplicationInformation.Instance.Session[model.OrderKey] = deal;

                if (model.InvalidAccountReason.Count == 0)
                    model.InvalidAccountReason = null;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in SimpleCurrencyExchangeController::Initialize", ex);
                TempData["ErrorResult"] += "General Error.";
            }

            return true;
        }
        [HttpPost, ActionName("EnterData")]
        [ValidateAntiForgeryToken]
        public ActionResult EnterDataPost(SimpleCurrencyExchangeViewModel model)
        {
            if (model == null || model.WorkflowStage < 1 || model.WorkflowStage > 2)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            if (model.Comment == null)
                model.Comment = "";
                
            if (model.CCY1 == model.CCY2)
                TempData["ErrorResult"] += "Currencies must be different. ";

            if (model.Amount == 0)
                TempData["ErrorResult"] += "Please select amount. ";

            if (model.Amount < CurrencyManager.MinDealAmountUSD)
                TempData["ErrorResult"] += "invalid amount. amount > " + CurrencyManager.MinDealAmountUSD.ToString() + ". ";

            if (model.InvalidAccountReason != null && model.InvalidAccountReason.Count == 1 && model.InvalidAccountReason[0] == "")
                model.InvalidAccountReason = null;

            Deal deal = model.DealInSession;
            if (deal == null)
            {
                TempData["ErrorResult"] += "Timeout expired. ";
                RedirectToAction("EnterData");
            }

            if (!CurrencyManager.Instance.PairList.ContainsKey(model.CCY1 + model.CCY2) && !CurrencyManager.Instance.PairList.ContainsKey(model.CCY2 + model.CCY1))
                TempData["ErrorResult"] += "Invalid currencies combination. ";

            if (TempData["ErrorResult"] != null)
                return View(model);



            try
            {
                // to do support this fields on next stage
                //deal.ChargedAccount

                deal.BuySell = model.BuySell;

                //check the pair order according to the currencies selection
                bool isOppositeSide = false;
                string symbol = model.CCY1 + model.CCY2;
                Consts.eBidAsk priceBidOrAsk = Consts.eBidAsk.Bid;
                if (CurrencyManager.Instance.PairList.ContainsKey(symbol))
                {
                    deal.Symbol = symbol;
                    if (model.BuySell == Consts.eBuySell.Buy)
                        priceBidOrAsk = Consts.eBidAsk.Ask;
                }
                else
                {
                    isOppositeSide = true;
                    symbol = model.CCY2 + model.CCY1;
                    deal.Symbol = symbol;
                    if (model.BuySell == Consts.eBuySell.Sell)
                        priceBidOrAsk = Consts.eBidAsk.Ask;
                }

                FXRate pairRate = CurrencyManager.Instance.PairRates[symbol];

                if ((DateTime.Now - pairRate.LastUpdate).TotalMinutes > 5 || !pairRate.IsTradable)
                {
                    //the exchange rate is not up to date
                    if (!deal.IsDemo)
                    {
                        TempData["ErrorResult"] += "The Exchange Rate is not up to date. Please contact FlatFX Support in order to get price.";
                        return View(model);
                    }
                }

                deal.OfferingDate = DateTime.Now;
                deal.EnsureOnLinePrice = model.EnsureOnLinePrice;
                deal.PvPEnabled = model.PvPEnabled;
                deal.FastTransferEnabled = model.FastTransferEnabled;
                if (deal.PvPEnabled)
                {
                    model.EnsureOnLinePrice = true;
                    deal.EnsureOnLinePrice = true;
                }


                double Mid = pairRate.Mid;
                double Bid = pairRate.Bid;
                double Ask = pairRate.Ask;

                double extraCharge = 0;
                if (deal.EnsureOnLinePrice)
                    extraCharge += CurrencyManager.ExtraCharge_EnsureOnLinePrice;
                if (deal.PvPEnabled)
                    extraCharge += CurrencyManager.ExtraCharge_PvPEnabled;
                if (deal.FastTransferEnabled)
                    extraCharge += CurrencyManager.ExtraCharge_FastTransferEnabled;

                deal.OfferingMidRate = deal.MidRate;
                if (deal.EnsureOnLinePrice)
                    deal.MidRate = deal.OfferingMidRate;
                else
                    deal.MidRate = null;

                if (extraCharge > 0)
                {
                    Bid = Bid - (Mid * extraCharge);
                    Ask = Ask + (Mid * extraCharge);
                }

                if (priceBidOrAsk == Consts.eBidAsk.Bid)
                {
                    deal.CustomerRate = Bid;
                    deal.BankRate = Mid - (CurrencyManager.BankCommission * Mid);
                }
                else
                {
                    deal.CustomerRate = Ask;
                    deal.BankRate = Mid + (CurrencyManager.BankCommission * Mid);
                }

                if (model.BuySell == Consts.eBuySell.Buy)
                {
                    deal.AmountToExchangeCreditedCurrency = model.Amount;
                    if (isOppositeSide)
                    {
                        deal.AmountToExchangeChargedCurrency = (1 / deal.CustomerRate) * model.Amount;
                    }
                    else
                    {
                        deal.AmountToExchangeChargedCurrency = deal.CustomerRate * model.Amount;
                    }
                    deal.CreditedCurrency = model.CCY1;
                    deal.ChargedCurrency = model.CCY2;
                }
                else
                {
                    deal.AmountToExchangeChargedCurrency = model.Amount;
                    if (isOppositeSide)
                    {
                        deal.AmountToExchangeCreditedCurrency = (1 / deal.CustomerRate) * model.Amount;
                    }
                    else
                    {
                        deal.AmountToExchangeCreditedCurrency = deal.CustomerRate * model.Amount;
                    }
                    deal.CreditedCurrency = model.CCY2;
                    deal.ChargedCurrency = model.CCY1;
                }

                //calculate AmountUSD_Estimation (volume)
                if (deal.CreditedCurrency == "USD")
                    deal.AmountUSD = deal.AmountToExchangeCreditedCurrency;
                else if (deal.ChargedCurrency == "USD")
                    deal.AmountUSD = deal.AmountToExchangeChargedCurrency;
                else
                    deal.AmountUSD = CurrencyManager.Instance.GetAmountUSD(deal.CreditedCurrency, deal.AmountToExchangeCreditedCurrency);

                //calculate profit
                string minorCurrency = model.CCY2;
                if (isOppositeSide)
                    minorCurrency = model.CCY1;
                deal.BankIncomeUSD = Math.Round(CurrencyManager.Instance.GetAmountUSD(minorCurrency, deal.AmountUSD * CurrencyManager.BankCommission * Mid), 2); // 0.5 promil 
                deal.CustomerTotalProfitUSD = Math.Round(CurrencyManager.Instance.GetAmountUSD(minorCurrency, deal.AmountUSD * (CurrencyManager.CustomerProfit - extraCharge) * Mid) - CurrencyManager.TransactionFeeUSD, 2); // 8 promil - Extra
                deal.FlatFXIncomeUSD = Math.Round(CurrencyManager.Instance.GetAmountUSD(minorCurrency, deal.AmountUSD * Math.Abs(deal.CustomerRate - deal.BankRate.Value)), 2); // 2.5 promil + Extra
                deal.Commission = deal.BankIncomeUSD + deal.FlatFXIncomeUSD; // 3 promil + Extra
                

                if (model.Comment == null)
                    model.Comment = "";
                deal.Comment = model.Comment;
                deal.ContractDate = null;
                deal.MaturityDate = null;

                ApplicationInformation.Instance.Session[model.OrderKey] = deal;
                model.deal = deal;

                db.Deals.Attach(deal);
                db.Entry(deal).State = EntityState.Added;
                db.SaveChanges();

                model.DealId = deal.DealId;
                model.WorkflowStage = 2;

                return RedirectToAction("EnterData", model);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in SimpleCurrencyExchangeController::EnterDataPost", ex);
                TempData["ErrorResult"] += "General Error. ";
            }

            return View(model);
        }
        [HttpPost, ActionName("Confim")]
        [ValidateAntiForgeryToken]
        public ActionResult Confim(SimpleCurrencyExchangeViewModel model)
        {
            try
            {
                if (model == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                if (model.InvalidAccountReason != null && model.InvalidAccountReason.Count == 1 && model.InvalidAccountReason[0] == "")
                    model.InvalidAccountReason = null;

                model.deal = model.DealInSession;
                if (model.DealId != model.DealInSession.DealId)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                Deal deal = db.Deals.Find(model.DealInSession.DealId);

                if (deal.OfferingDate.AddSeconds(60) < DateTime.Now)
                {
                    TempData["ErrorResult"] += "Timeout expired. ";
                    return RedirectToAction("EnterData", model);
                }

                deal.ContractDate = DateTime.Now;
                deal.IsOffer = false;
                deal.MaturityDate = DateTime.Now;
                deal.Status = Consts.eDealStatus.New;
                db.SaveChanges();

                model.DealId = deal.DealId;
                model.deal = deal;
                model.WorkflowStage = 3;
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in SimpleCurrencyExchangeController::Confim", ex);
                TempData["ErrorResult"] += "General Error. Please contact FlatFX Team.";
            }

            return RedirectToAction("EnterData", model);
        }
    }
}