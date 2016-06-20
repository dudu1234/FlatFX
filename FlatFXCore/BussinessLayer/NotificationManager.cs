using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Security.Principal;
using System.Timers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;
using FlatFXCore.Model.User;

namespace FlatFXCore.BussinessLayer
{
    public class NotificationManager
    {
        private static NotificationManager m_Instance = null;
        private static Object m_Sync = new Object();
        private const int m_SendEmailInterval = 30 * 1000;
        private ApplicationDBContext db = new ApplicationDBContext();
        private Task m_SendEmailTask = null;
        private bool m_Terminate = false;
        private EmailService m_EmailService = null;

        #region Ctor + Dtor
        /// <summary>
        ///     The Singelton ctor.
        /// </summary>
        private NotificationManager()
        {
            m_EmailService = new EmailService();
        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_SendEmailTask != null)
                {
                    m_Terminate = true;
                    m_SendEmailTask.Dispose();
                    m_SendEmailTask = null;
                }
                if (db != null)
                {
                    db.Dispose();
                }
            }
        }
        /// <summary>
        /// NotificationManager
        /// </summary>
        public static NotificationManager Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (m_Sync)
                    {
                        if (m_Instance == null)
                            m_Instance = new NotificationManager();
                    }
                }

                return m_Instance;
            }
        }
        public void Start() 
        {
            if (m_SendEmailTask == null)
            {
                m_SendEmailTask = new Task(() => SendEmailLoop());
                m_SendEmailTask.Start();
            }
        }
        #endregion

        void SendEmailLoop()
        {
            while (!m_Terminate)
            {
                try
                {
                    m_SendEmailTask.Wait(m_SendEmailInterval);
                    List<EmailNotification> list = db.EmailNotifications.Where(e => e.EmailStatus == Consts.eEmailStatus.None).ToList();
                    if (list.Count == 0)
                        continue;

                    foreach(EmailNotification email in list)
                    {
                        bool res = m_EmailService.SendMail(email);
                        if (res)
                            email.EmailStatus = Consts.eEmailStatus.Sent;
                    }
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Logger.Instance.WriteError("Failed in NotificationManager::SendEmailTimer_Elapsed", ex);
                }
            }
        }

    }
}
