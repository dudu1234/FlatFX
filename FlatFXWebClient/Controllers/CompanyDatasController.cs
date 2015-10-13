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
    public class CompanyDatasController : Controller
    {
        private FfxContext db = new FfxContext();

        // GET: CompanyDatas
        public async Task<ActionResult> Index()
        {
            var companies = db.Companies.Include(c => c.ContactDetails);
            return View(await companies.ToListAsync());
        }

        // GET: CompanyDatas/Details/5
        public async Task<ActionResult> Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyData companyData = await db.Companies.FindAsync(id);
            if (companyData == null)
            {
                return HttpNotFound();
            }
            return View(companyData);
        }

        // GET: CompanyDatas/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CompanyDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CompanyShortName,CompanyFullName,ValidIP,CustomerType,IsDepositValid,IsSignOnRegistrationAgreement")] CompanyData companyData)
        {
            if (ModelState.IsValid)
            {
                companyData.CompanyId = Guid.NewGuid().ToString();
                companyData.CreatedAt = DateTime.Now;
                companyData.IsActive = true;
                companyData.LastUpdate = DateTime.Now;
                companyData.Status = FlatFXCore.BussinessLayer.Consts.eCompanyStatus.Active;

                ContactDetails contactDetails = new ContactDetails();
                contactDetails.ContactDetailsId = Guid.NewGuid().ToString();

                companyData.ContactDetailsId = contactDetails.ContactDetailsId;

                db.Companies.Add(companyData);
                db.ContactsDetails.Add(contactDetails);

                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(companyData);
        }

        // GET: CompanyDatas/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyData companyData = await db.Companies.FindAsync(id);
            if (companyData == null)
            {
                return HttpNotFound();
            }
            return View(companyData);
        }

        // POST: CompanyDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CompanyId,CompanyShortName,CompanyFullName,IsActive,Status,CreatedAt,LastUpdate,ValidIP,CustomerType,IsDepositValid,IsSignOnRegistrationAgreement,ContactDetailsId")] CompanyData companyData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companyData).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(companyData);
        }

        // GET: CompanyDatas/Delete/5
        public async Task<ActionResult> Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyData companyData = await db.Companies.FindAsync(id);
            if (companyData == null)
            {
                return HttpNotFound();
            }
            return View(companyData);
        }

        // POST: CompanyDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            CompanyData companyData = await db.Companies.FindAsync(id);
            db.Companies.Remove(companyData);
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
