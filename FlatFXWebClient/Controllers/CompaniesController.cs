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
        public async Task<ActionResult> IndexAdmin()
        {
            return View(await db.Companies.ToListAsync());
        }

        // GET: Companies
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
            List<Company> companies = user.Companies.ToList();
            return View("IndexUser", companies);
        }

        public async Task<ActionResult> UserCompanies(string userId)
        {
            ApplicationUser user = await db.Users.Where(u => u.Id == userId).SingleOrDefaultAsync();
            ViewBag.UserName = user.UserName;
            List<Company> companies = await db.Companies.Include(u => u.Users).Where(c => c.Users.Any<ApplicationUser>(u => u.Id == userId) == true).ToListAsync();
            return View("IndexAdmin", companies);
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

        // POST: Companies/Edit/5
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
            Company company = await db.Companies.FindAsync(id);
            try
            {
                string[] whiteListCompany = new string[] { "CompanyName", "CompanyFullName", "IsActive", "Status", "ValidIP", "CustomerType", "IsDepositValid", "IsSignOnRegistrationAgreement", "CompanyVolumePerYearUSD", 
                        "UserList_SendEmail", "UserList_SendInvoice" };
                string[] whiteListContactDetails = new string[] { "CarPhone", "Address", "Country", "Email", "Email2", "Fax", "MobilePhone", "MobilePhone2", 
                        "OfficePhone", "OfficePhone2", "HomePhone", "WebSite" };

                if (TryUpdateModel(company, "", whiteListCompany) && 
                    TryUpdateModel(company.ContactDetails, "ContactDetails", whiteListContactDetails))
                {
                    try
                    {
                        company.LastUpdate = DateTime.Now;
                        db.SaveChanges();
                        ViewBag.Result = "Update succeeded";
                        return RedirectToAction("IndexAdmin");
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
        // GET: Companies/Edit/5
        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser)]
        public async Task<ActionResult> EditByUser(string id)
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
            if (company == null)
            {
                return HttpNotFound();
            }

            return View("EditByUser", company);
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
            if (!SecurityManager.IsValidCompany(db, User, User.Identity.GetUserId(), id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = await db.Companies.FindAsync(id);
            try
            {
                string[] whiteListCompany = new string[] { "CompanyName", "CompanyFullName", "CustomerType", "CompanyVolumePerYearUSD", "UserList_SendEmail", "UserList_SendInvoice" };
                string[] whiteListContactDetails = new string[] { "CarPhone", "Address", "Country", "Email", "Email2", "Fax", "MobilePhone", "MobilePhone2", 
                        "OfficePhone", "OfficePhone2", "HomePhone", "WebSite" };

                if (TryUpdateModel(company, "", whiteListCompany) && 
                    TryUpdateModel(company.ContactDetails, "ContactDetails", whiteListContactDetails))
                {
                    try
                    {
                        company.LastUpdate = DateTime.Now;
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
            return RedirectToAction("IndexAdmin");
        }
    }
}
