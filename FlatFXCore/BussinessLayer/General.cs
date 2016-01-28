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
}
