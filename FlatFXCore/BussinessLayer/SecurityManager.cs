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
}
