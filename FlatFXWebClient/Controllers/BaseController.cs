using FlatFXCore.Model.Core;
using System;
using System.Web.Mvc;

namespace FlatFXWebClient.Controllers
{
    /// <summary>
    /// Defines the base controller.
    /// </summary>
    /// <remarks>
    /// This is the base class for all site's controllers.
    /// </remarks>
    public class BaseController : Controller
    {
        /// <summary>
        /// Gets the current site session.
        /// </summary>
        public FlatFXSession CurrentSiteSession
        {
            get
            {
                if (Session["SiteSession"] != null)
                    return (FlatFXSession)this.Session["SiteSession"];
                else
                    return null;
            }
        }
        /// <summary>
        /// The data context.
        /// </summary>
        protected ApplicationDBContext _db = new ApplicationDBContext();

        /// <summary>
        /// Dispose the used resource.
        /// </summary>
        /// <param name="disposing">The disposing flag.</param>
        protected override void Dispose(bool disposing)
        {
            if (_db != null)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
        /// <summary>
        /// Called when an unhandled exception occurs in the action.
        /// </summary>
        /// <param name="filterContext">Information about the current request and action.</param>
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is UnauthorizedAccessException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = RedirectToAction("Home", "Index");
            }
            //
            base.OnException(filterContext);
        }

    }
}