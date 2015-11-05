using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Security.Principal;

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
    }
}
