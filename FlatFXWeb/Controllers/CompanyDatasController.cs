using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FlatFX.Model.Core;
using FlatFX.Model.Data;

namespace FlatFXWeb.Controllers
{
    [Authorize]
    public class CompanyDatasController : Controller
    {
        private FfxContext db = new FfxContext();

        // GET: CompanyDatas
        //[Authorize]
        //[Authorize(Users="dudu,guy")]
        //[Authorize(Rules="admin")]
        //[AllowAnonymous]
        public ActionResult Index()
        {
            //if (User.Identity.IsAuthenticated)
            //User.Identity.Name
            
            var companies = db.Companies.Include(c => c.ContactDetails);
            return View(companies.ToList());
        }

        // GET: CompanyDatas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyData companyData = db.Companies.Find(id);
            if (companyData == null)
            {
                return HttpNotFound();
            }
            return View(companyData);
        }

        // GET: CompanyDatas/Create
        public ActionResult Create()
        {
            ViewBag.ContactDetailsId = new SelectList(db.ContactsDetails, "ContactDetailsId", "Email");
            return View();
        }

        // POST: CompanyDatas/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CompanyId,CompanyShortName,CompanyFullName,IsActive,Status,CreatedAt,LastUpdate,ValidIP,CustomerType,IsDepositValid,IsSignOnRegistrationAgreement,ContactDetailsId")] CompanyData companyData)
        {
            if (ModelState.IsValid)
            {
                db.Companies.Add(companyData);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ContactDetailsId = new SelectList(db.ContactsDetails, "ContactDetailsId", "Email", companyData.ContactDetailsId);
            return View(companyData);
        }

        // GET: CompanyDatas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyData companyData = db.Companies.Find(id);
            if (companyData == null)
            {
                return HttpNotFound();
            }
            ViewBag.ContactDetailsId = new SelectList(db.ContactsDetails, "ContactDetailsId", "Email", companyData.ContactDetailsId);
            return View(companyData);
        }

        // POST: CompanyDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CompanyId,CompanyShortName,CompanyFullName,IsActive,Status,CreatedAt,LastUpdate,ValidIP,CustomerType,IsDepositValid,IsSignOnRegistrationAgreement,ContactDetailsId")] CompanyData companyData)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companyData).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ContactDetailsId = new SelectList(db.ContactsDetails, "ContactDetailsId", "Email", companyData.ContactDetailsId);
            return View(companyData);
        }

        // GET: CompanyDatas/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CompanyData companyData = db.Companies.Find(id);
            if (companyData == null)
            {
                return HttpNotFound();
            }
            return View(companyData);
        }

        // POST: CompanyDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CompanyData companyData = db.Companies.Find(id);
            db.Companies.Remove(companyData);
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
