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
        public async Task<ActionResult> EnterData(SimpleCurrencyExchangeViewModel model)
        {
            if (model.SelectedAccount == null)
            {
                Session["SimpleCurrencyExchangeStep"] = 1;
                model = new SimpleCurrencyExchangeViewModel();
                await Initialize(db, model);
            }
            else
            {

            }

            return View(model);
        }
        private async Task<bool> Initialize(ApplicationDBContext db, SimpleCurrencyExchangeViewModel model)
        {
            string error;
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
                deal.ChargedProviderAccount = providerAccounts[0];
                deal.CreditedProviderAccount = providerAccounts[0];
            }
            deal.DealProductType = Consts.eDealProductType.FxSimpleExchange;
            deal.DealType = Consts.eDealType.Spot;
            deal.IsOffer = true;
            if (providerAccounts.Count > 0)
            {
                deal.Provider = providerAccounts[0].Provider;
                deal.CompanyAccount = providerAccounts[0].CompanyAccount;
            }
            deal.user = user;

            ApplicationInformation.Instance.Session[model.OrderKey] = deal;

            if (model.InvalidAccountReason.Count == 0)
                model.InvalidAccountReason = null;

            return true;
        }
        [HttpPost, ActionName("EnterData")]
        [ValidateAntiForgeryToken]
        public ActionResult EnterDataPost(SimpleCurrencyExchangeViewModel model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (model.CCY1 == model.CCY2)
                TempData["ErrorResult"] += "Currencies must be different. ";

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
                deal.BuySell = model.BuySell;
                
                //check the pair order according to the currencies selection
                string symbol = model.CCY1 + model.CCY2;
                bool isMainCurrencyFirst = true; //USDILS or ILSUSD
                Consts.eBidAsk priceBidOrAsk = Consts.eBidAsk.Bid; 
                if (CurrencyManager.Instance.PairList.ContainsKey(symbol))
                {
                    deal.Symbol = symbol;
                    if (model.BuySell == Consts.eBuySell.Buy)
                        priceBidOrAsk = Consts.eBidAsk.Ask;
                }
                else
                {
                    isMainCurrencyFirst = false;
                    symbol = model.CCY2 + model.CCY1;
                    if (model.BuySell == Consts.eBuySell.Sell)
                        priceBidOrAsk = Consts.eBidAsk.Ask;
                }

                FXRate pairRate = CurrencyManager.Instance.PairRates[symbol];

                if ((DateTime.Now - pairRate.LastUpdate).TotalMinutes > 5)
                {
                    //the exchange rate is not up to date
                    TempData["ErrorResult"] += "The Exchange Rate is not up to date. Please contact FlatFX Support in order to get price.";
                    return View(model);
                }

                deal.OfferingDate = DateTime.Now;
                deal.MidRate = pairRate.Mid;
                if (priceBidOrAsk == Consts.eBidAsk.Bid)
                {
                    deal.CustomerRate = pairRate.Bid;
                    deal.BankRate = pairRate.Mid - (0.0005 * pairRate.Mid);
                }
                else
                {
                    deal.CustomerRate = pairRate.Ask;
                    deal.BankRate = pairRate.Mid + (0.0005 * pairRate.Mid);
                    //deal.BankTotalProfitUSD
                }

                if (model.BuySell == Consts.eBuySell.Buy)
                {
                    deal.AmountToExchangeCreditedCurrency = model.Amount;
                    deal.AmountToExchangeChargedCurrency = deal.CustomerRate * model.Amount;
                    deal.CreditedCurrency = model.CCY1; 
                    deal.ChargedCurrency = model.CCY2;
                }
                else
                {
                    deal.AmountToExchangeChargedCurrency = model.Amount;
                    deal.AmountToExchangeCreditedCurrency = deal.CustomerRate * model.Amount;
                    deal.CreditedCurrency = model.CCY2;
                    deal.ChargedCurrency = model.CCY1;
                }

                // Calculate AmountUSD
                if (deal.CreditedCurrency == "USD")
                    deal.AmountUSD = deal.AmountToExchangeCreditedCurrency;
                else if (deal.ChargedCurrency == "USD")
                    deal.AmountUSD = deal.AmountToExchangeChargedCurrency;
                else
                    deal.AmountUSD = deal.AmountToExchangeCreditedCurrency * CurrencyManager.Instance.GetFXRateVsUSD(deal.CreditedCurrency).Mid;

            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in SimpleCurrencyExchangeController::EnterDataPost", ex);
                TempData["ErrorResult"] += "General Error. ";
            }

            return View(model);
        }
        public async Task<ActionResult> Confim(string dealId)
        {
            if (dealId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Deal deal = await db.Deals.FindAsync(dealId);
            return View(deal);
        }
    }
}