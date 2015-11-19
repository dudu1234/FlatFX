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
                    return db.CompanyAccounts.Include(ca => ca.Company).Where(ca => user1.Companies.Contains(ca.Company)).Any();
                }

                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        
    }
}
