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
using FlatFXCore.Model.User;

namespace FlatFXWebClient.Controllers
{
    [Authorize(Roles = Consts.Role_Administrator)]
    public class ProviderAccountsController : BaseController
    {
        // GET: ProviderAccounts
        public async Task<ActionResult> Index()
        {
            var providerAccounts = db.ProviderAccounts.Include(p => p.CompanyAccount).Include(p => p.Provider);
            return View(await providerAccounts.ToListAsync());
        }

        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser)]
        public async Task<ActionResult> IndexUser()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser user = await db.Users.Include(u => u.Companies).Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return HttpNotFound();
            }
            List<string> userCompaniesIdList = user.Companies.Select(c => c.CompanyId).ToList();
            List<string> companyAccountsIdList = await db.CompanyAccounts.Where(ca => userCompaniesIdList.Contains(ca.CompanyId))
                .Select(ca => ca.CompanyAccountId).ToListAsync();
            var providerAccounts = await db.ProviderAccounts.Include(pa => pa.CompanyAccount).Include(p => p.Provider)
                .Where(pa => companyAccountsIdList.Contains(pa.CompanyAccountId)).ToListAsync();
            if (providerAccounts.Count == 1)
                return RedirectToAction("EditByUser", new { companyAccountId = providerAccounts[0].CompanyAccountId, providerId = providerAccounts[0].ProviderId });
            else
                return View("IndexUser", providerAccounts);
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
                string[] whiteList = new string[] { "AccountName","BankAccountName","BankBranchNumber","BankAccountNumber","BankAddress","IBAN","SWIFT",
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
                        return View(providerAccount);
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

        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser)]
        public async Task<ActionResult> EditByUser(string companyAccountId, string providerId)
        {
            if (companyAccountId == null || providerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!SecurityManager.IsValidProviderAccount(db, User, User.Identity.GetUserId(), companyAccountId, providerId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ProviderAccount providerAccount = await db.ProviderAccounts.Where(pa => pa.ProviderId == providerId && pa.CompanyAccountId == companyAccountId).SingleOrDefaultAsync();
            if (providerAccount == null)
            {
                return HttpNotFound();
            }

            return View("EditByUser", providerAccount);
        }
        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("EditByUser")]
        [ValidateAntiForgeryToken]
        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser)]
        public async Task<ActionResult> EditByUserPost(string companyAccountId, string providerId)
        {
            if (companyAccountId == null || providerId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!SecurityManager.IsValidProviderAccount(db, User, User.Identity.GetUserId(), companyAccountId, providerId))
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
                // to do , remove this line and do not enable the user to change the account details, the change should be only by email to FlatFX ???
                //if there is a change than the ApprovedBYFlatFX & ApprovedBYProvider should be reset to 'false' again.
                string[] whiteList = new string[] { "AccountName", "BankBranchNumber", "BankAccountNumber", "BankAddress", "IBAN", "SWIFT" };
                if (TryUpdateModel(providerAccount, "", whiteList))
                {
                    try
                    {
                        providerAccount.LastUpdate = DateTime.Now;
                        providerAccount.LastUpdateBy = User.Identity.GetUserName();
                        db.SaveChanges();
                        ViewBag.Result = "Update succeeded";
                        return View(providerAccount);
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
    }
}
