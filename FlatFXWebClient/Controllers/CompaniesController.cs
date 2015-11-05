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
using FlatFXCore.Model.User;
using FlatFXCore.BussinessLayer;

namespace FlatFXWebClient.Controllers
{
    [Authorize(Roles = Consts.Role_Administrator)]
    public class CompaniesController : BaseController
    {
        // GET: Companies
        public async Task<ActionResult> Index()
        {
            return View(await db.Companies.ToListAsync());
        }

        // GET: Companies
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser)]
        public async Task<ActionResult> IndexUserMode()
        {
            ViewBag.IsUserMode = true;
            string userId = User.Identity.GetUserId();
            ApplicationUser user = await db.Users.Include(u => u.Companies).Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return HttpNotFound();
            }
            List<Company> companies = user.Companies.ToList();
            return View("Index", companies);
        }

        // GET: Companies/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = await db.Companies.FindAsync(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Companies/Edit/5
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser)]
        public async Task<ActionResult> EditUserMode(string id)
        {
            ViewBag.IsUserMode = true;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            if (!SecurityManager.IsValidCompany(db, User, User.Identity.GetUserId(), id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Company company = await db.Companies.FindAsync(id);
            if (company == null)
            {
                return HttpNotFound();
            }

            //return View("~/Views/Companies/Edit.cshtml", company);
            return View("Edit", company);
        }

        // POST: Companies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser)]
        public async Task<ActionResult> EditPost(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!SecurityManager.IsValidCompany(db, User, User.Identity.GetUserId(), id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = await db.Companies.FindAsync(id);
            try
            {
                string[] whiteListCompany = null;
                string[] whiteListContactDetails = new string[] { "CarPhone", "Address", "Country", "Email", "Email2", "Fax", "MobilePhone", "MobilePhone2", 
                        "OfficePhone", "OfficePhone2", "HomePhone", "WebSite" };
                
                if (User.IsInRole(Consts.Role_Administrator))
                {
                    whiteListCompany = new string[] { "CompanyShortName", "CompanyFullName", "IsActive", "Status", "ValidIP", "CustomerType", "IsDepositValid", "IsSignOnRegistrationAgreement", "CompanyVolumePerYearUSD", 
                        "UserList_SendEmail", "UserList_SendInvoice" };
                }
                else
                {
                    whiteListCompany = new string[] { "CompanyShortName", "CompanyFullName", "CustomerType", "CompanyVolumePerYearUSD", "UserList_SendEmail", "UserList_SendInvoice" };
                }

                if (TryUpdateModel(company, "", whiteListCompany) && TryUpdateModel(company.ContactDetails, "ContactDetails", whiteListContactDetails))
                {
                    try
                    {
                        company.LastUpdate = DateTime.Now;
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
            return View(company);
        }

        // GET: Companies/Delete/5
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
            Company company = await db.Companies.FindAsync(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            try
            {
                Company company = await db.Companies.FindAsync(id);
                db.Companies.Remove(company);
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
