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
    public class LogInfoController : Controller
    {
        private ApplicationDBContext db = new ApplicationDBContext();

        // GET: LogInfo
        public async Task<ActionResult> Index()
        {
            DateTime from = DateTime.Now.AddMinutes(-60);
            var logInfo = db.LogInfo.Include(l => l.User).Where(l => l.Date > from);
            return View(await logInfo.ToListAsync());
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
