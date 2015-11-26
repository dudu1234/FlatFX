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
    public class CompanyAccountsController : BaseController
    {
        // GET: CompanyAccounts
        public async Task<ActionResult> Index()
        {
            var companyAccounts = db.CompanyAccounts.Include(c => c.Company);
            return View(await companyAccounts.ToListAsync());
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
            var companyAccounts = await db.CompanyAccounts.Include(c => c.Company).Where(ca => userCompaniesIdList.Contains(ca.CompanyId)).ToListAsync();
            return View("IndexUser", companyAccounts);
        }

        // GET: CompanyAccounts/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyAccount companyAccount = await db.CompanyAccounts.FindAsync(id);
            if (companyAccount == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "CompanyName", companyAccount.CompanyId);
            return View(companyAccount);
        }

        // POST: CompanyAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyAccount companyAccount = await db.CompanyAccounts.FindAsync(id);
            try
            {
                string[] whiteList = new string[] { "AccountName", "IsActive", "IsDefaultAccount", "Balance", "Equity", "DailyPNL", "GrossPNL" };
                if (TryUpdateModel(companyAccount, "", whiteList))
                {
                    try
                    {
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

            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "CompanyName", companyAccount.CompanyId);
            return View(companyAccount);
        }

        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser)]
        public async Task<ActionResult> EditByUser(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!SecurityManager.IsValidCompanyAccount(db, User, User.Identity.GetUserId(), id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CompanyAccount companyAccount = await db.CompanyAccounts.FindAsync(id);
            if (companyAccount == null)
            {
                return HttpNotFound();
            }

            return View("EditByUser", companyAccount);
        }
        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("EditByUser")]
        [ValidateAntiForgeryToken]
        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser)]
        public async Task<ActionResult> EditByUserPost(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!SecurityManager.IsValidCompanyAccount(db, User, User.Identity.GetUserId(), id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyAccount companyAccount = await db.CompanyAccounts.FindAsync(id);
            try
            {
                string[] whiteList = new string[] { "AccountName", "IsDefaultAccount" };
                if (TryUpdateModel(companyAccount, "", whiteList))
                {
                    try
                    {
                        db.SaveChanges();
                        ViewBag.Result = "Update succeeded";
                        return RedirectToAction("IndexUser");
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
            return View(companyAccount);
        }

        // GET: CompanyAccounts/Delete/5
        public async Task<ActionResult> Delete(string id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            CompanyAccount companyAccount = await db.CompanyAccounts.FindAsync(id);
            if (companyAccount == null)
            {
                return HttpNotFound();
            }
            return View(companyAccount);
        }

        // POST: CompanyAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                CompanyAccount companyAccount = await db.CompanyAccounts.FindAsync(id);
                db.CompanyAccounts.Remove(companyAccount);
                await db.SaveChangesAsync();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }
            return RedirectToAction("Index");
        }
    }
}
