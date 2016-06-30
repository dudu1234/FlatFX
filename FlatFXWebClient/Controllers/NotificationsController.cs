using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;
using FlatFXCore.Model.User;
using FlatFXCore.BussinessLayer;
using FlatFXWebClient.ViewModels;

namespace FlatFXWebClient.Controllers
{
    [Authorize(Roles = Consts.Role_Administrator)]
    public class NotificationsController : BaseController
    {
        public async Task<ActionResult> IndexAdmin()
        {
            return View(await db.NewOrderNotifications.ToListAsync());
        }

        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser + "," + Consts.Role_CompanyDemoUser)]
        public async Task<ActionResult> IndexUser()
        {
            string userId = User.Identity.GetUserId();
            ApplicationUser user = await db.Users.Include(u => u.Companies).Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
            {
                return HttpNotFound();
            }
            List<NewOrderNotification> notifications = db.NewOrderNotifications.Where(n => n.UserId == userId).ToList();
            return View("IndexUser", notifications);
        }

        public async Task<ActionResult> UserNotifications(string userId)
        {
            ApplicationUser user = await db.Users.Where(u => u.Id == userId).SingleOrDefaultAsync();
            ViewBag.UserName = user.UserName;
            List<NewOrderNotification> notifications = await db.NewOrderNotifications.Include(n => n.User).Where(n => n.UserId == userId).ToListAsync();
            return View("IndexAdmin", notifications);
        }

        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser + "," + Consts.Role_CompanyDemoUser)]
        public ActionResult Create()
        {
            NewOrderNotification notification = new NewOrderNotification();
            return View(notification);
        }

        // POST: Notifications/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser + "," + Consts.Role_CompanyDemoUser)]
        public async Task<ActionResult> CreatePost()
        {
            NewOrderNotification notification = new NewOrderNotification();
            try
            {
                string[] whiteListNotification = new string[] { "Symbol", "BuySell", "MinVolume", "MaxVolume", "Expired", "ProviderId", "UserId" };
                if (TryUpdateModel(notification, "", whiteListNotification))
                {
                    try
                    {
                        notification.CreatedAt = DateTime.Now;
                        notification.NotificationType = Consts.eNotificationType.OnNewOrder;

                        if (notification.UserId == null)
                            notification.UserId = User.Identity.GetUserId();

                        bool isDemo = db.ProviderAccounts.Where(pa => pa.CompanyAccount.Company.Users.Any(u => u.Id == notification.UserId)).FirstOrDefault().IsDemoAccount;
                        notification.IsDemo = isDemo;
                        if (notification.ProviderId == null)
                            notification.ProviderId = "";
                        db.NewOrderNotifications.Add(notification);
                        await db.SaveChangesAsync();
                        TempData["Result"] = "Create succeeded";

                        return RedirectToAction("IndexUser");
                    }
                    catch (DataException /* dex */)
                    {
                        //Log the error (uncomment dex variable name and add a line here to write a log.
                        TempData["ErrorResult"] = "Unable to save changes. Try again, and if the problem persists, see your system administrator.";
                    }
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                TempData["ErrorResult"] = "Unable to save changes. Try again, and if the problem persists, see your system administrator.";
            }
            return View(notification);
        }


        public async Task<ActionResult> Edit(Int64 id)
        {
            NewOrderNotification notification = await db.NewOrderNotifications.FindAsync(id);
            if (notification == null)
                return HttpNotFound();
            
            return View(notification);
        }

        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditPost(Int64 id)
        {
            NewOrderNotification notification = await db.NewOrderNotifications.FindAsync(id);
            if (notification == null)
                return HttpNotFound();
            
            try
            {
                string[] whiteListNotification = new string[] { "ProviderId", "Symbol", "BuySell", "MinVolume", "MaxVolume", "Expired", "ProviderId" };

                if (TryUpdateModel(notification, "", whiteListNotification))
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
                        TempData["ErrorResult"] = "Unable to save changes. Try again, and if the problem persists, see your system administrator.";
                    }
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                TempData["ErrorResult"] = "Unable to save changes. Try again, and if the problem persists, see your system administrator.";
            }
            return View(notification);
        }
        // GET: Notifications/Edit/5
        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser + "," + Consts.Role_CompanyDemoUser)]
        public async Task<ActionResult> EditByUser(Int64 id)
        {
            NewOrderNotification notification = await db.NewOrderNotifications.FindAsync(id);
            if (notification == null)
                return HttpNotFound();
            
            return View("EditByUser", notification);
        }
        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost, ActionName("EditByUser")]
        [ValidateAntiForgeryToken]
        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser + "," + Consts.Role_CompanyDemoUser)]
        public async Task<ActionResult> EditByUserPost(Int64 id)
        {
            NewOrderNotification notification = await db.NewOrderNotifications.FindAsync(id);
            if (notification == null)
                return HttpNotFound();
            
            try
            {
                string[] whiteListNotification = new string[] { "ProviderId", "Symbol", "BuySell", "MinVolume", "MaxVolume", "Expired", "ProviderId" };

                if (TryUpdateModel(notification, "", whiteListNotification))
                {
                    try
                    {
                        db.SaveChanges();
                        TempData["Result"] = "Update succeeded";
                        return View(notification); // RedirectToAction("IndexUser");
                    }
                    catch (DataException /* dex */)
                    {
                        //Log the error (uncomment dex variable name and add a line here to write a log.
                        TempData["ErrorResult"] = "Unable to save changes. Try again, and if the problem persists, see your system administrator.";
                    }
                }
            }
            catch (DataException /* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                TempData["ErrorResult"] = "Unable to save changes. Try again, and if the problem persists, see your system administrator.";
            }
            return View(notification);
        }


        // GET: Notifications/Delete/5
        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser + "," + Consts.Role_CompanyDemoUser)]
        public async Task<ActionResult> Delete(Int64 id, bool? saveChangesError = false)
        {
            if (saveChangesError.GetValueOrDefault())
            {
                TempData["ErrorResult"] = "Delete failed. Try again, and if the problem persists see your system administrator.";
            }
            NewOrderNotification notification = await db.NewOrderNotifications.FindAsync(id);
            if (notification == null)
                return HttpNotFound();
            
            return View(notification);
        }

        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [OverrideAuthorization]
        [Authorize(Roles = Consts.Role_Administrator + "," + Consts.Role_CompanyUser + "," + Consts.Role_ProviderUser + "," + Consts.Role_CompanyDemoUser)]
        public async Task<ActionResult> DeleteConfirmed(Int64 id)
        {
            try
            {
                NewOrderNotification notification = await db.NewOrderNotifications.FindAsync(id);
                db.NewOrderNotifications.Remove(notification);
                await db.SaveChangesAsync();
            }
            catch (DataException/* dex */)
            {
                //Log the error (uncomment dex variable name and add a line here to write a log.
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            TempData["Result"] = "Delete succeeded";
            return RedirectToAction("IndexUser");
        }
    }
}
