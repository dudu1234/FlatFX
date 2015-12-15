using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.User;
using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Data;
using System.Threading.Tasks;

namespace FlatFXWebClient.Controllers
{
    [Authorize(Roles = Consts.Role_Administrator)]
    public class ApplicationUsersController : BaseController
    {
        // GET: ApplicationUsers
        public ActionResult IndexAdmin()
        {
            return View(db.Users.Include(u => u.Companies).ToList());
        }
        public async Task<ActionResult> CompanyUsers(string companyId)
        {
            Company company = await db.Companies.Where(comp => comp.CompanyId == companyId).SingleOrDefaultAsync();
            ViewBag.CompanyName = company.CompanyName;
            List<ApplicationUser> users = await db.Users.Include(u => u.Companies).Where(u => u.Companies.Any<Company>(comp => comp.CompanyId == companyId) == true).ToListAsync();
            return View("IndexAdmin", users);
        }

        // GET: ApplicationUsers/Create
        public ActionResult Create()
        {
            return View();
        }
        
        // GET: ApplicationUsers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ApplicationUser user = db.Users.Find(id); 
            try
            {
                if (TryUpdateModel(user, "",
                   new string[] { "FirstName", "MiddleName", "LastName", "IsActive", "Status", "RoleInCompany", "Language", "SigningKey", 
                   "InvoiceCurrency", "Email", "EmailConfirmed", "PhoneNumber", "PhoneNumberConfirmed", "AccessFailedCount", "LockoutEnabled", "LockoutEndDateUtc",
                   "IsApprovedByFlatFX" }) &&
                    TryUpdateModel(user.ContactDetails, "ContactDetails", new string[] { "Address", "Country", "OfficePhone", "Fax", "WebSite" }))
                {
                    try
                    {
                        db.SaveChanges();
                        TempData["Result"] = "Update succeeded";
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
            return View(user);
        }
        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser)]
        public ActionResult EditByUser()
        {
            string id = User.Identity.GetUserId();
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("EditByUser")]
        [ValidateAntiForgeryToken]
        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser)]
        public ActionResult EditByUserPost()
        {
            string id = User.Identity.GetUserId();
            ApplicationUser user = db.Users.Find(id);
            try
            {
                if (TryUpdateModel(user, "", new string[] { "FirstName", "MiddleName", "LastName", "RoleInCompany", "InvoiceCurrency", "Email", "PhoneNumber" }) &&
                    TryUpdateModel(user.ContactDetails, "ContactDetails", new string[] { "Address", "Country", "OfficePhone", "Fax", "WebSite" }))
                {
                    db.SaveChanges();
                    TempData["Result"] = "Update succeeded";
                    return RedirectToAction("EditByUser");
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }
            return View(user);
        }

        // GET: ApplicationUsers/Delete/5
        public ActionResult Delete(string id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (saveChangesError.GetValueOrDefault())
            {
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            ApplicationUser applicationUser = db.Users.Find(id);
            if (applicationUser == null)
            {
                return HttpNotFound();
            }
            return View(applicationUser);
        }

        // POST: ApplicationUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                ApplicationUser applicationUser = db.Users.Find(id);
                db.Users.Remove(applicationUser);
                db.SaveChanges();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            TempData["Result"] = "Delete succeeded";
            return RedirectToAction("IndexAdmin");
        }
    }
}
