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

namespace FlatFXWebClient.Controllers
{
    public class SimpleCurrencyExchangeController : BaseController
    {
        public async Task<ActionResult> EnterData(string dealId)
        {
            Deal deal = null;
            if (dealId != null)
            {
                deal = await db.Deals.FindAsync(dealId);
            }
            else
            {
                deal = new Deal();
                await deal.InitializeSimpleCurrencyExchangeAsync();
            }

            return View(deal);
        }
        [HttpPost, ActionName("EnterData")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EnterDataPost(string dealId)
        {
            if (dealId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Deal deal = await db.Deals.FindAsync(dealId);
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