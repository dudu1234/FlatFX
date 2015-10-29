using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;
using FlatFXCore.Model.User;

namespace FlatFXWebClient.Controllers
{
    public class ProvidersController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: Providers
        public ActionResult Index()
        {
            List<Provider> providers = db.Providers.ToList();
            return View(providers);
        }

        // GET: Providers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Providers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = @"ShortName,FullName,BankNumber,ProviderType,QuoteResponse_Enabled,
            QuoteResponse_AutomaticResponseEnabled,QuoteResponse_MinRequestVolumeUSD,QuoteResponse_MaxDailyVolumeUSD,
            ContactDetails")] Provider provider)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    provider.IsActive = true;
                    provider.ProviderId = Guid.NewGuid().ToString();
                    provider.QuoteResponse_StartTime = new DateTime(1999, 1, 1, 8, 0, 0);
                    provider.QuoteResponse_EndTime = new DateTime(1999, 1, 1, 23, 0, 0);
                    provider.QuoteResponse_FridayEndTime = new DateTime(1999, 1, 1, 13, 0, 0);
                    provider.QuoteResponse_FridayStartTime = new DateTime(1999, 1, 1, 8, 0, 0);
                    provider.QuoteResponse_NumberOfPromilsWithoutDiscount = 10;
                    provider.QuoteResponse_SpreadMethod = FlatFXCore.BussinessLayer.Consts.eQuoteResponseSpreadMethod.Constant;
                    provider.QuoteResponse_UserConfirmationTimeInterval = 40;
                    provider.Status = FlatFXCore.BussinessLayer.Consts.eProviderStatus.Active;
                    provider.ContactDetails.Address = provider.ContactDetails.Address;
                    provider.ContactDetails.Country = provider.ContactDetails.Country;
                    provider.ContactDetails.Email = provider.ContactDetails.Email;
                    provider.ContactDetails.Fax = provider.ContactDetails.Fax;
                    provider.ContactDetails.OfficePhone = provider.ContactDetails.OfficePhone;
                    provider.ContactDetails.MobilePhone = provider.ContactDetails.MobilePhone;


                    db.Providers.Add(provider);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            catch (DataException) // dex )
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }
            return View(provider);
        }

        // GET: Providers/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Provider provider = db.Providers.Where(p => p.ProviderId == id).Single();
            if (provider == null)
            {
                return HttpNotFound();
            }

            return View(provider);
        }

        // POST: Providers/Edit/5
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
            Provider provider = db.Providers.Find(id);
            try
            {
                if (TryUpdateModel(provider, "",
                   new string[] { "ShortName", "FullName", "BankNumber", "IsActive", "Status", "ProviderType", "QuoteResponse_Enabled", "QuoteResponse_SpreadMethod", "QuoteResponse_StartTime", 
                   "QuoteResponse_EndTime", "QuoteResponse_FridayStartTime", "QuoteResponse_FridayEndTime", "QuoteResponse_UserConfirmationTimeInterval", 
                   "QuoteResponse_AutomaticResponseEnabled", "QuoteResponse_MinRequestVolumeUSD", "QuoteResponse_MaxDailyVolumeUSD", "QuoteResponse_NumberOfPromilsWithoutDiscount" }) &&
                    TryUpdateModel(provider.ContactDetails, "ContactDetails", new string[] { "Address", "Country", "Email", "Fax", "MobilePhone", "OfficePhone" }))
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
            return View(provider);
        }

        // GET: Providers/Delete/5
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
            Provider provider = db.Providers.Find(id);
            if (provider == null)
            {
                return HttpNotFound();
            }
            return View(provider);
        }

        // POST: Providers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            try
            {
                Provider provider = db.Providers.Find(id);

                db.Providers.Remove(provider);
                db.SaveChanges();
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
