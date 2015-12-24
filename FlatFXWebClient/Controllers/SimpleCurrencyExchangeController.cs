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
                await model.Initialize(db);
            }
            else
            {

            }

            return View(model);
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