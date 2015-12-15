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
            if (model.deal == null)
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
        public async Task<ActionResult> EnterDataPost(SimpleCurrencyExchangeViewModel model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Deal deal = await db.Deals.FindAsync(model.deal.DealId);
            try
            {
                string[] whiteList = new string[] { "AccountName", "IsActive", "IsDefaultAccount", "Balance", "Equity", "DailyPNL", "GrossPNL" };
                if (TryUpdateModel(deal, "", whiteList))
                {
                    try
                    {
                        db.SaveChanges();
                        ViewBag.Result = "Update succeeded";
                        return RedirectToAction("Confim");
                    }
                    catch (DataException /* dex */)
                    {
                        //Log the error (uncomment dex variable name and add a line here to write a log.
                        ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                    }
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
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