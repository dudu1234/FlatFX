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
                string pair = model.CCY1 + model.CCY2;
                if (CurrencyManager.Instance.PairList.ContainsKey(pair))
                {
                    deal.Amount1 = model.Amount;
                    //deal.Amount2 = 
                }
                else
                {
                    pair = model.CCY2 + model.CCY1;
                }

                if (model.BuySell == Consts.eBuySell.Buy)
                {
                    deal.AmountToExchangeCreditedCurrency = model.Amount;
                }
                else
                {
                    deal.AmountToExchangeChargedCurrency = model.Amount;
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in SimpleCurrencyExchangeController::EnterDataPost", ex);
                TempData["ErrorResult"] += "General Error. ";
            }

            return View(deal);
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