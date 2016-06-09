using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;

namespace FlatFXCore.BussinessLayer
{
    public class GeneralFunction
    {
        public static List<DateTime> GetDays(int daysBack, bool includeToday)
        {
            return Enumerable.Range(includeToday ? 0 : 1, daysBack).Select(d => new DateTime(DateTime.Now.AddDays(-1 * d).Year, 
                DateTime.Now.AddDays(-1 * d).Month, DateTime.Now.AddDays(-1 * d).Day)).ToList();
        }
        public static List<DateTime> GetMonth(int monthBack, bool includeThisMonth)
        {
            return Enumerable.Range(includeThisMonth ? 0 : 1, monthBack).Select(m => new DateTime(DateTime.Now.AddMonths(-1 * m).Year, 
                DateTime.Now.AddMonths(-1 * m).Month, 1)).ToList();
        }
    }
    public class DailyTasks
    {
        private static DailyTasks m_DailyTasks = null;
        private System.Threading.Timer m_DailyTimer = null;
        private System.Threading.Timer m_5MinutesTimer = null;
        
        #region Ctor
        /// <summary>
        ///     Ctor
        /// </summary>
        internal DailyTasks()
        {

        }
        public static DailyTasks Instance
        {
            get
            {
                if (m_DailyTasks == null)
                    m_DailyTasks = new DailyTasks();

                return m_DailyTasks;
            }
        }
        public void Start() 
        {
            System.Threading.TimerCallback callback = DailyTrigger; 
            DateTime two = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 2, 0, 0).AddDays(1);
            TimeSpan ts = two - DateTime.Now;
            m_DailyTimer = new System.Threading.Timer(callback, null, (int)(ts.TotalSeconds * 1000), 24 * 60 * 60 * 1000);
            m_5MinutesTimer = new System.Threading.Timer(Minutes5Trigger, null, 60 * 1000, 5 * 1000);
            callback(null);
        }
        #endregion

        #region Dispose
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_DailyTimer != null)
                {
                    m_DailyTimer.Dispose();
                    m_DailyTimer = null;
                }
                if (m_5MinutesTimer != null)
                {
                    m_5MinutesTimer.Dispose();
                    m_5MinutesTimer = null;
                }
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
        ~DailyTasks()
        {
            Dispose(false);
        }
        #endregion

        /// <summary>
        /// Runs every day at 02:00 AM
        /// </summary>
        /// <param name="stateInfo"></param>
        public void DailyTrigger(Object stateInfo)
        {
            try
            {
                using (var db = new ApplicationDBContext())
                {
                    DateTime dt = DateTime.Now;

                    //Set Orders Expiry Date, to do dudu Should run on Mid-Night Exactlly
                    dt = DateTime.Today;
                    List<Order> orders2 = db.Orders.Where(o => (o.Status == Consts.eOrderStatus.None || o.Status == Consts.eOrderStatus.Waiting || o.Status == Consts.eOrderStatus.Triggered_partially) && o.ExpiryDate.HasValue && o.ExpiryDate.Value <= dt).ToList();
                    foreach (Order order in orders2)
                    {
                        order.Status = Consts.eOrderStatus.Expired;
                    }
                    db.SaveChanges();

                    //Set status = Close for all DEMO FxSimpleExchange Deals
                    dt = DateTime.Now.AddDays(-5);
                    List<Deal> deals = db.Deals.Where(d => d.IsDemo && (d.Status == Consts.eDealStatus.None || d.Status == Consts.eDealStatus.New || d.Status == Consts.eDealStatus.CustomerTransfer) 
                        && d.MaturityDate < dt && d.DealProductType == Consts.eDealProductType.FxSimpleExchange).ToList();
                    foreach(Deal deal in deals)
                    {
                        deal.Status = Consts.eDealStatus.Closed;
                    }
                    db.SaveChanges();


                    //Set status = Close for all DEMO OrderMatch and match's orders + deals
                    dt = DateTime.Now.AddDays(-1);
                    List<OrderMatch> matches = db.OrderMatches.Include("Order1").Include("Deal1").Include("Order2").Include("Deal2").Where(m => m.Status != Consts.eMatchStatus.Closed && m.Order1.IsDemo == true && m.TriggerDate < dt).ToList();
                    foreach (OrderMatch match in matches)
                    {
                        match.Status = Consts.eMatchStatus.Closed;
                        match.CloseDate = DateTime.Now;

                        match.Order1.Status = Consts.eOrderStatus.Closed_Successfully;
                        match.Order2.Status = Consts.eOrderStatus.Closed_Successfully;

                        match.Deal1.Status = Consts.eDealStatus.Closed;
                        match.Deal2.Status = Consts.eDealStatus.Closed;
                    }
                    db.SaveChanges();


                    //Set status = Close for all old DEMO Orders
                    dt = DateTime.Now.AddDays(-60);
                    List<Order> orders = db.Orders.Where(o => o.IsDemo && (o.Status == Consts.eOrderStatus.None || o.Status == Consts.eOrderStatus.Waiting || o.Status == Consts.eOrderStatus.Triggered_partially) && 
                        o.OrderDate < dt).ToList();
                    foreach (Order order in orders)
                    {
                        order.Status = Consts.eOrderStatus.Expired;
                        order.ExpiryDate = DateTime.Today;
                    }
                    db.SaveChanges();

                    //Delete Daily Rates
                    DateTime date = DateTime.Now.AddDays(-7);
                    db.DailyFXRates.RemoveRange(db.DailyFXRates.Where(r => r.Time < date));
                }

            }
            catch(Exception ex)
            {
                Logger.Instance.WriteError("Failed in DailyTrigger", ex);
            }
        }
        /// <summary>
        /// Runs every 5 Minutes
        /// </summary>
        /// <param name="stateInfo"></param>
        public void Minutes5Trigger(Object stateInfo)
        {
            try
            {
                using (var db = new ApplicationDBContext())
                {
                    DateTime dt = DateTime.Now;

                    //Set Orders Expiry Date
                    List<Order> orders2 = db.Orders.Where(o => (o.Status == Consts.eOrderStatus.None || o.Status == Consts.eOrderStatus.Waiting || o.Status == Consts.eOrderStatus.Triggered_partially) && o.ExpiryDate.HasValue && o.ExpiryDate.Value <= dt).ToList();
                    foreach (Order order in orders2)
                    {
                        order.Status = Consts.eOrderStatus.Expired;
                    }
                    db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in Minutes5Trigger", ex);
            }
        }
    }
}
