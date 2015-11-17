using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
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
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProviderAccount providerAccount = await db.ProviderAccounts.FindAsync(id);
            if (providerAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyAccountId = new SelectList(db.CompanyAccounts, "CompanyAccountId", "CompanyId", providerAccount.CompanyAccountId);
            ViewBag.ProviderId = new SelectList(db.Providers, "ProviderId", "ShortName", providerAccount.ProviderId);
            return View(providerAccount);
        }

        // POST: ProviderAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CompanyAccountId,ProviderId,BankAccountName,BankAccountFullName,BankBranchNumber,BankAccountNumber,BankAddress,IBAN,SWIFT,AllowToTradeDirectlly,ApprovedBYFlatFX,ApprovedBYProvider,UserKeyInProviderSystems,IsActive,IsDemoAccount,CreatedAt,LastUpdate,LastUpdateBy,QuoteResponse_IsBlocked,QuoteResponse_CustomerPromil")] ProviderAccount providerAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(providerAccount).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyAccountId = new SelectList(db.CompanyAccounts, "CompanyAccountId", "CompanyId", providerAccount.CompanyAccountId);
            ViewBag.ProviderId = new SelectList(db.Providers, "ProviderId", "ShortName", providerAccount.ProviderId);
            return View(providerAccount);
        }

        // GET: ProviderAccounts/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProviderAccount providerAccount = await db.ProviderAccounts.FindAsync(id);
            if (providerAccount == null)
            {
                return HttpNotFound();
            }
            return View(providerAccount);
        }

        // POST: ProviderAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            ProviderAccount providerAccount = await db.ProviderAccounts.FindAsync(id);
            db.ProviderAccounts.Remove(providerAccount);
            await db.SaveChangesAsync();
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
