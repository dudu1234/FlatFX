using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Security.Principal;
using FlatFXCore.Model.User;

namespace FlatFXCore.BussinessLayer
{
    public class SecurityManager
    {
        public static bool IsValidCompany(ApplicationDBContext db, IPrincipal user, string userId, string companyId)
        {
            try
            {
                if (user.IsInRole(Consts.Role_Administrator))
                    return true;
                else if (user.IsInRole(Consts.Role_CompanyUser))
                    return db.Companies.Include(c => c.Users).Where(c => c.CompanyId == companyId && c.Users.Any(u => u.Id == userId)).Any();

                return false;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsValidCompanyAccount(ApplicationDBContext db, IPrincipal user, string userId, string companyAccountId)
        {
            try
            {
                if (user.IsInRole(Consts.Role_Administrator))
                    return true;
                else if (user.IsInRole(Consts.Role_CompanyUser))
                {
                    ApplicationUser user1 = db.Users.Include(u => u.Companies).Where(u => u.Id == userId).FirstOrDefault();
                    List<string> userCompaniesIdList = user1.Companies.Select(c => c.CompanyId).ToList();
                    List<string> companyAccountsIdList = db.CompanyAccounts.Include(c => c.Company).Where(ca => userCompaniesIdList.Contains(ca.CompanyId))
                        .Select(ca => ca.CompanyAccountId).ToList();
                    return companyAccountsIdList.Contains(companyAccountId);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsValidProviderAccount(ApplicationDBContext db, IPrincipal user, string userId, string companyAccountId, string providerId)
        {
            try
            {
                if (user.IsInRole(Consts.Role_Administrator))
                    return true;
                else if (user.IsInRole(Consts.Role_CompanyUser))
                {
                    ApplicationUser user1 = db.Users.Include(u => u.Companies).Where(u => u.Id == userId).FirstOrDefault();
                    List<string> userCompaniesIdList = user1.Companies.Select(c => c.CompanyId).ToList();
                    List<string> companyAccountsIdList = db.CompanyAccounts.Include(c => c.Company).Where(ca => userCompaniesIdList.Contains(ca.CompanyId))
                        .Select(ca => ca.CompanyAccountId).ToList();
                    List<string> providerAccountsIdList = db.ProviderAccounts.Where(pa => companyAccountsIdList.Contains(pa.CompanyAccountId))
                        .Select(pa => pa.CompanyAccountId).ToList();
                    return companyAccountsIdList.Contains(companyAccountId);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
        
    }
    
    public class TradingSecurity
    {
        private static TradingSecurity m_Instance = null;

        public const int StartingHour = 8;
        public const int StartingMinute = 0;
        public const int EndingHour = 16;
        public const int EndingMinute = 0;
        public const int FridayStartingHour = 8;
        public const int FridayEndingHour = 13;
        private bool m_IsTradingBlocked = true;

        #region Ctor
        /// <summary>
        ///     Ctor
        /// </summary>
        internal TradingSecurity()
        {
        }
        public static TradingSecurity Instance
        {
            get
            {
                if (m_Instance == null)
                    m_Instance = new TradingSecurity();

                return m_Instance;
            }
        }
        #endregion

        public bool IsTradingEnabled
        {
            get
            {
                if (m_IsTradingBlocked)
                    return false;

                
                if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                    return false;

                DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, StartingHour, StartingMinute, 0);
                DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, EndingHour, EndingMinute, 0);
                if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                {
                    start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, FridayStartingHour, 0, 0);
                    end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, FridayEndingHour, 0, 0);
                }

                if (DateTime.Now < start || DateTime.Now > end)
                    return false;
                else
                    return true;
            }
        }
        public bool IsTradingBlocked
        {
            get
            {
                return m_IsTradingBlocked;
            }

        }
        public void BlockTrading()
        {
            m_IsTradingBlocked = true;
        }
        public void EnableTrading()
        {
            m_IsTradingBlocked = false;
        }
    }
}
