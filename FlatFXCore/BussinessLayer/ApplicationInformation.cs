using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;
//using System.Globalization;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Security.Cryptography;
//using Microsoft.Win32;
//using System.IO;
//using System.Text.RegularExpressions;
//using System.Web.Security;

using Microsoft.AspNet.Identity;
using System.Web.SessionState;
//using FlatFXCore.Model.Data;

namespace FlatFXCore.BussinessLayer
{
    /// <summary>
    ///     Contains General Information regarding current application state.
    ///     This will be saved per Session.
    /// </summary>
    public class ApplicationInformation : IDisposable
    {
        #region Ctor
        /// <summary>
        ///     Ctor
        /// </summary>
        internal ApplicationInformation()
        {
        }
        public static ApplicationInformation Instance
        {
            get
            {
                if (m_ApplicationInformationInstance == null)
                    m_ApplicationInformationInstance = new ApplicationInformation();

                return m_ApplicationInformationInstance;
            }
        }
        public void Start() { }
        #endregion

        #region Dispose
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Dtor
        /// </summary>
        ~ApplicationInformation()
        {
            Dispose(false);
        }
        #endregion

        #region Members
        private static ApplicationInformation m_ApplicationInformationInstance = null;
        #endregion

        #region Public Functions
        public bool IsDevelopmetMachine
        {
            get
            {
                if (Environment.MachineName == "DUDU-HP")
                    return true;
                else
                    return false;
            }
        }
        public string UserID
        {
            get
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                if (context == null || context.User == null || context.User.Identity.Name == "")
                    return "";
                else
                    return context.User.Identity.GetUserId();
            }
        }
        public bool IsUserInRole(string role)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            if (context == null || context.User.Identity.Name == "")
                return false;
            else
                return context.User.IsInRole(role);
        }
        public bool IsDemoUser
        {
            get
            {
                return IsUserInRole(Consts.Role_CompanyDemoUser);
            }
        }
        public bool IsAdministrator
        {
            get
            {
                return IsUserInRole(Consts.Role_Administrator);
            }
        }
        public string SessionID
        {
            get
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;

                if (context == null || context.Session == null)
                    return null;
                else
                    return context.Session.SessionID;
            }
        }
        public HttpSessionState Session
        {
            get
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;

                if (context == null || context.Session == null)
                    return null;
                else
                    return context.Session;
            }
        }
        public string UserIP
        {
            get
            {
                try
                {
                    System.Web.HttpContext context = System.Web.HttpContext.Current;
                    if (context == null || context.Request == null || context.Request.ServerVariables == null || context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null)
                        return null;

                    string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (!string.IsNullOrEmpty(ipAddress))
                    {
                        string[] addresses = ipAddress.Split(',');
                        if (addresses.Length != 0)
                        {
                            return addresses[0];
                        }
                    }

                    if (context.Request.ServerVariables["REMOTE_ADDR"] == null)
                        return null;
                    else
                        return context.Request.ServerVariables["REMOTE_ADDR"];
                }
                catch
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// GetStartOfDay
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public DateTime GetStartOfDay(DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
        }
        /// <summary>
        /// GetEndOfDay
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public DateTime GetEndOfDay(DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, Date.Day, 23, 59, 59);
        }
        /// <summary>
        /// IsToday
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public bool IsToday(DateTime Date)
        {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            if (Date >= today && Date < today.AddDays(1))
                return true;
            else
                return false;
        }
        public string NextBussinessDay()
        {
            DateTime nextDay = DateTime.Now;
            // To do Support Israel only, Does not support hollydays
            if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
                nextDay = DateTime.Now.AddDays(3);
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                nextDay = DateTime.Now.AddDays(2);
            else
                nextDay = DateTime.Now.AddDays(1);

            return nextDay.ToLongDateString();
        }
        public DateTime NextBussinessDay(int hour, int minute)
        {
            DateTime nextDay = DateTime.Now;
            // To do Support Israel only, Does not support hollydays
            if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
                nextDay = DateTime.Now.AddDays(3);
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                nextDay = DateTime.Now.AddDays(2);
            else
                nextDay = DateTime.Now.AddDays(1);

            return new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, hour, minute, 0);
        }
        #endregion
    }
}
