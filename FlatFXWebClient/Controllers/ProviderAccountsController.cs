using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;
using FlatFXCore.BussinessLayer;

namespace FlatFXWebClient.Controllers
{
    [Authorize(Roles = Consts.Role_Administrator)]
    public class ProviderAccountsController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: ProviderAccounts
        public async Task<ActionResult> Index()
        {
            var providerAccounts = db.ProviderAccounts.Include(p => p.CompanyAccount).Include(p => p.Provider);
            return View(await providerAccounts.ToListAsync());
        }

        // GET: ProviderAccounts/Edit/5
        public async Task<ActionResult> Edit(string companyAccountId, string providerId)
        {
            if (companyAccountId == null || providerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProviderAccount providerAccount = await db.ProviderAccounts.Where(pa => pa.ProviderId == providerId && pa.CompanyAccountId == companyAccountId).SingleOrDefaultAsync();
            if (providerAccount == null)
            {
                return HttpNotFound();
            }
            //ViewBag.ProviderId = new SelectList(db.Providers, "ProviderId", "ShortName", providerAccount.ProviderId);
            return View(providerAccount);
        }

        // POST: ProviderAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(string companyAccountId, string providerId)
        {
            if (companyAccountId == null || providerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProviderAccount providerAccount = await db.ProviderAccounts.Where(pa => pa.ProviderId == providerId && pa.CompanyAccountId == companyAccountId).SingleOrDefaultAsync();
            if (providerAccount == null)
            {
                return HttpNotFound();
            }

            try
            {
                string[] whiteList = new string[] { "BankAccountName","BankAccountFullName","BankBranchNumber","BankAccountNumber","BankAddress","IBAN","SWIFT",
                    "AllowToTradeDirectlly","ApprovedBYFlatFX","ApprovedBYProvider","UserKeyInProviderSystems","IsActive","IsDemoAccount","QuoteResponse_IsBlocked",
                    "QuoteResponse_CustomerPromil" };
                if (TryUpdateModel(providerAccount, "", whiteList))
                {
                    try
                    {
                        providerAccount.LastUpdate = DateTime.Now;
                        providerAccount.LastUpdateBy = User.Identity.GetUserName();

                        db.SaveChanges();
                        ViewBag.Result = "Update succeeded";
                        return RedirectToAction("Index");
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

            return View(providerAccount);
        }
        
        // GET: ProviderAccounts/Delete/5
        public async Task<ActionResult> Delete(string companyAccountId, string providerId, bool? saveChangesError = false)
        {
            if (companyAccountId == null || providerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            ProviderAccount providerAccount = await db.ProviderAccounts.Where(pa => pa.ProviderId == providerId && pa.CompanyAccountId == companyAccountId).SingleOrDefaultAsync();
            if (providerAccount == null)
            {
                return HttpNotFound();
            }
            return View(providerAccount);
        }

        // POST: ProviderAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string companyAccountId, string providerId)
        {
            try
            {
                ProviderAccount providerAccount = await db.ProviderAccounts.Where(pa => pa.ProviderId == providerId && pa.CompanyAccountId == companyAccountId).SingleOrDefaultAsync();
                db.ProviderAccounts.Remove(providerAccount);
                await db.SaveChangesAsync();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { companyAccountId = companyAccountId, providerId = providerId, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
