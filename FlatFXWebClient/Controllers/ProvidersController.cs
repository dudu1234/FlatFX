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
        private FfxContext db = new FfxContext();

        // GET: Providers
        public ActionResult Index()
        {
            var providers = db.Providers.Include(p => p.ContactDetails);
            //var providers = db.Providers.Include("ContactDetails");
            List<Provider> list = providers.ToList();
            // GUY 34
            // why does the Address is null ?
            if (list.Count > 0)
            {
                string Address = list[0].ContactDetails.Address;
            }
            return View(list);
        }

        // GET: Providers/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            Provider provider = db.Providers.Find(id);
            if (provider == null)
            {
                return HttpNotFound();
            }

            ContactDetails details = db.ContactsDetails.Where(d => d.ContactDetailsId == provider.ContactDetailsId).Single();
            provider.ContactDetails = details;

            return View(provider);
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
            if (ModelState.IsValid)
            {
                ContactDetails contactDetails = new ContactDetails();
                contactDetails.ContactDetailsId = Guid.NewGuid().ToString();
                contactDetails.Address = provider.ContactDetails.Address;
                contactDetails.Country = provider.ContactDetails.Country;
                contactDetails.Email = provider.ContactDetails.Email;
                contactDetails.Fax = provider.ContactDetails.Fax;
                contactDetails.OfficePhone = provider.ContactDetails.OfficePhone;
                contactDetails.MobilePhone = provider.ContactDetails.MobilePhone;

                provider.ContactDetailsId = contactDetails.ContactDetailsId;
                provider.ContactDetails = contactDetails;

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

                db.ContactsDetails.Add(contactDetails);
                db.Providers.Add(provider);
                db.SaveChanges();
                return RedirectToAction("Index");
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
            Provider provider = db.Providers.Include(p => p.ContactDetails).Where(p => p.ProviderId == id).Single();
            if (provider == null)
            {
                return HttpNotFound();
            }
            
            ContactDetails details = db.ContactsDetails.Where(d => d.ContactDetailsId == provider.ContactDetailsId).Single();
            provider.ContactDetails = details;

            return View(provider);
        }

        // POST: Providers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Provider provider)
        {
            if (ModelState.IsValid)
            {
                db.Entry(provider).State = EntityState.Modified;
                db.Entry(provider.ContactDetails).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(provider);
        }

        // GET: Providers/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
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
            Provider provider = db.Providers.Find(id);

            ContactDetails details = db.ContactsDetails.Where(d => d.ContactDetailsId == provider.ContactDetailsId).Single();
            provider.ContactDetails = details;
            
            db.Providers.Remove(provider);
            db.ContactsDetails.Remove(details);
            db.SaveChanges();
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
