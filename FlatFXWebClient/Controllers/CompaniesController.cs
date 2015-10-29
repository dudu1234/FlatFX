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

namespace FlatFXWebClient.Controllers
{
    public class CompaniesController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: Companies
        public async Task<ActionResult> Index()
        {
            return View(await db.Companies.ToListAsync());
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
                if (TryUpdateModel(company, "",
                   new string[] { "CompanyShortName", "CompanyFullName", "IsActive", "Status", "ValidIP", "CustomerType", "IsDepositValid", "IsSignOnRegistrationAgreement", "CompanyVolumePerYearUSD", 
                   "UserList_SendEmail", "UserList_SendInvoice" }) &&
                    TryUpdateModel(company.ContactDetails, "ContactDetails", new string[] { "CarPhone", "Address", "Country", "Email", "Email2", "Fax", "MobilePhone", "MobilePhone2", 
                        "OfficePhone", "OfficePhone2", "HomePhone", "WebSite" }))
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

        // GET: Companies/Edit/5
        public async Task<ActionResult> EditByUser(string id)
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
        [HttpPost, ActionName("EditByUser")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditByUserPost(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = await db.Companies.FindAsync(id);
            try
            {
                if (TryUpdateModel(company, "",
                   new string[] { "CompanyShortName", "CompanyFullName", "CustomerType", "CompanyVolumePerYearUSD", "UserList_SendEmail", "UserList_SendInvoice" }) &&
                    TryUpdateModel(company.ContactDetails, "ContactDetails", new string[] { "CarPhone", "Address", "Country", "Email", "Email2", "Fax", "MobilePhone", "MobilePhone2", 
                        "OfficePhone", "OfficePhone2", "HomePhone", "WebSite" }))
                {
                    try
                    {
                        company.LastUpdate = DateTime.Now;
                        db.SaveChanges();
                        TempData["Result"] = "Update succeeded";
                        return RedirectToAction("EditByUser");
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
