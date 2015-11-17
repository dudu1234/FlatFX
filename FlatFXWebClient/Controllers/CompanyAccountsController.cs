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
    public class CompanyAccountsController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: CompanyAccounts
        public async Task<ActionResult> Index()
        {
            var companyAccounts = db.CompanyAccounts.Include(c => c.Company);
            return View(await companyAccounts.ToListAsync());
        }

        // GET: CompanyAccounts/Details/5
        public async Task<ActionResult> Details(string id)
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
            return View(companyAccount);
        }

        // GET: CompanyAccounts/Create
        public ActionResult Create()
        {
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "CompanyShortName");
            return View();
        }

        // POST: CompanyAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CompanyAccountId,CompanyId,AccountName,AccountFullName,IsActive,IsDefaultAccount,Balance,Equity,DailyPNL,GrossPNL")] CompanyAccount companyAccount)
        {
            if (ModelState.IsValid)
            {
                db.CompanyAccounts.Add(companyAccount);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "CompanyShortName", companyAccount.CompanyId);
            return View(companyAccount);
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
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "CompanyShortName", companyAccount.CompanyId);
            return View(companyAccount);
        }

        // POST: CompanyAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "CompanyAccountId,CompanyId,AccountName,AccountFullName,IsActive,IsDefaultAccount,Balance,Equity,DailyPNL,GrossPNL")] CompanyAccount companyAccount)
        {
            if (ModelState.IsValid)
            {
                db.Entry(companyAccount).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "CompanyShortName", companyAccount.CompanyId);
            return View(companyAccount);
        }

        // GET: CompanyAccounts/Delete/5
        public async Task<ActionResult> Delete(string id)
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
            return View(companyAccount);
        }

        // POST: CompanyAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(string id)
        {
            CompanyAccount companyAccount = await db.CompanyAccounts.FindAsync(id);
            db.CompanyAccounts.Remove(companyAccount);
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
