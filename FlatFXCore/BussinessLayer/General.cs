using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public void DailyTrigger(Object stateInfo)
        {
            try
            {
                //Set status for all demo deals
                using (var db = new ApplicationDBContext())
                {
                    List<Deal> deals = db.Deals.Where(d => d.IsDemo && d.Status != Consts.eDealStatus.Closed).ToList();
                    foreach(Deal deal in deals)
                    {
                        deal.Status = Consts.eDealStatus.Closed;
                    }
                    db.SaveChanges();
                }

            }
            catch(Exception ex)
            {
                Logger.Instance.WriteError("Failed in DailyTrigger", ex);
            }
        }
    }
}
