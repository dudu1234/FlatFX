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
        private ApplicationDBContext sendNewOrderNotificationDB = new ApplicationDBContext();
        private Task m_SendEmailTask = null;
        private bool m_Terminate = false;
        private EmailService m_EmailService = null;
        private Object m_NewOrdersListSync = new Object();
        private Queue<Order> m_NewOrdersQueue = new Queue<Order>();

        private Task m_SendNewOrderNotificationTask = null;

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

                if (m_SendNewOrderNotificationTask != null)
                {
                    m_SendNewOrderNotificationTask.Dispose();
                    m_SendNewOrderNotificationTask = null;
                }

                if (db != null)
                {
                    db.Dispose();
                }

                if (sendNewOrderNotificationDB != null)
                {
                    sendNewOrderNotificationDB.Dispose();
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

            if (m_SendNewOrderNotificationTask == null)
            {
                m_SendNewOrderNotificationTask = new Task(() => SendNewOrderNotificationLoop());
                m_SendNewOrderNotificationTask.Start();
            }
        }
        #endregion

        #region Send Emails Loop
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

                    foreach (EmailNotification email in list)
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
        #endregion

        #region New Order Notification
        public void AddNewOrder(Order order)
        {
            lock (m_NewOrdersListSync)
            {
                m_NewOrdersQueue.Enqueue(order);
            }
        }
        void SendNewOrderNotificationLoop()
        {
            while (true)
            {
                Order order = null;
                try
                {
                    System.Threading.Thread.Sleep(5000);

                    lock (m_NewOrdersListSync)
                    {
                        if (m_NewOrdersQueue.Count > 0)
                            order = m_NewOrdersQueue.Dequeue();
                    }

                    if (order != null)
                    {
                        //Check if there are notifications to send
                        DateTime dt = DateTime.Now;

                        List<NewOrderNotification> notifications = sendNewOrderNotificationDB.NewOrderNotifications.Where(n => n.Expired > dt && 
                            (n.BuySell == Consts.eBuySell.Both || n.BuySell == order.BuySell) &&
                            n.Symbol == order.Symbol &&
                            (n.ProviderId == null || n.ProviderId == "" || n.ProviderId == order.ProviderId) &&
                            n.UserId != order.UserId &&
                            n.NotificationType == Consts.eNotificationType.OnNewOrder &&
                            n.MaxVolume >= order.AmountCCY1 &&
                            n.MinVolume <= order.AmountCCY1 && 
                            n.IsDemo == order.IsDemo).ToList();

                        bool performSave = false;
                        foreach(NewOrderNotification notification in notifications)
                        {
                            performSave = true;
                            EmailNotification emailNotification = new EmailNotification(notification.User.Email, "info@FlatFX.com");
                            emailNotification.Subject = ((order.IsDemo) ? "Demo " : "") + "New Order notification: " +
                                order.BuySell.ToString() + " " + order.AmountCCY1 + " " + order.Symbol;
                            if (order.ExpiryDate.HasValue && order.ExpiryDate.Value < DateTime.Now.AddDays(2))
                                emailNotification.Subject += ". Expired: " + order.ExpiryDate.Value.ToString("dd/MM/yyyy HH:mm");

                            emailNotification.Body = "<b>Hello " + notification.User.FullName + "</b><br /><br />" +
                                "This is to inform you that potential partner entered a new exchange order.<br /><br />" +
                                "<b>Order Summary: </b><br />" +
                                "<div style=\"font-size: 0.9em\">Symbol: " + order.Symbol + "<br />" +
                                "Direction: " + order.BuySell + "<br />" +
                                "Amount CCY1: " + order.AmountCCY1.ToString("N2") + "<br />" +
                                "Expiry Date: " + (order.ExpiryDate.HasValue? order.ExpiryDate.Value.ToString() : "GTC") + "<br />" +
                                "Clearing Type: " + order.ClearingType.ToString() + "<br />" +
                                "Max Amount: " + order.AmountCCY1.ToString("N2") + "<br />" +
                                "Min Amount: " + (order.MinimalPartnerExecutionAmountCCY1.HasValue? order.MinimalPartnerExecutionAmountCCY1.Value.ToString("N2") : "-") + "<br /><br /></div>";
                            emailNotification.Body += AddBackOfficeSignature();
                            db.EmailNotifications.Add(emailNotification);
                        }
                        if (performSave)
                            db.SaveChanges();
                    }
                }
                catch
                {

                }
            }
        }
        #endregion

        #region Email generic functions
        public string AddBackOfficeSignature()
        {
            string htmlCode = "<br /><br />Best Regards,<br /><br />" +
                "<div>" +
                    "<b style=\"font-size: 1.1em;color: blue\">FlatFX Back Office Team</b><br />" +
                    "<div style=\"font-size: 0.9em\">" +
                        "phone:&nbsp;<a href=\"callto:972-3-111-1111\">972-3-111-1111</a><br />" +
                        "email:&nbsp;<a href=\"mailto:support@FlatFX.com\">support@FlatFX.com</a><br />" +
                        "web:&nbsp;<a href=\"http://www.FlatFX.com\">www.FlatFX.com</a><br />" +
                    "</div></div>";
            return htmlCode;
        }
        #endregion
    }
}
