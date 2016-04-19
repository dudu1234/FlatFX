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
using FlatFXCore.BussinessLayer;

namespace FlatFXWebClient.Controllers
{
    [Authorize(Roles = Consts.Role_Administrator)]
    public class LogInfoController : BaseController
    {
        // GET: LogInfo
        public async Task<ActionResult> Index()
        {
            DateTime from = DateTime.Now.AddDays(-1);
            var logInfo = db.LogInfo.Include(l => l.User).Where(l => l.Date > from).OrderByDescending(l => l.Date);
            return View(await logInfo.ToListAsync());
        }
    }
}
