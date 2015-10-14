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
using FlatFXCore.Model.User;

namespace FlatFXWebClient.Controllers
{
    public class CompanyController : BaseController
    {
        // GET: Company
        public async Task<ActionResult> Index()
        {
            var companies = _db.Companies.Include(c => c.ContactDetails);
            return View(await companies.ToListAsync());
        }

        // GET: Company/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = await _db.Companies.FindAsync(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Company/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Company/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CompanyShortName,CompanyFullName,ValidIP,CustomerType,IsDepositValid,IsSignOnRegistrationAgreement")] Company company)
        {
            if (ModelState.IsValid)
            {
                company.CompanyId = Guid.NewGuid().ToString();
                company.CreatedAt = DateTime.Now;
                company.IsActive = true;
                company.LastUpdate = DateTime.Now;
                company.Status = FlatFXCore.BussinessLayer.Consts.eCompanyStatus.Active;

                ContactDetails contactDetails = new ContactDetails();
                contactDetails.ContactDetailsId = Guid.NewGuid().ToString();

                company.ContactDetailsId = contactDetails.ContactDetailsId;

                _db.Companies.Add(company);
                _db.ContactsDetails.Add(contactDetails);

                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(company);
        }

        // GET: Company/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = await _db.Companies.FindAsync(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Company/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CompanyId,CompanyShortName,CompanyFullName,IsActive,Status,CreatedAt,LastUpdate,ValidIP,CustomerType,IsDepositValid,IsSignOnRegistrationAgreement,ContactDetailsId")] Company company)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(company).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(company);
        }

        // GET: Company/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = await _db.Companies.FindAsync(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Company/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            Company company = await _db.Companies.FindAsync(id);
            _db.Companies.Remove(company);
            await _db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
