using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;
using FlatFXCore.Model.User;

namespace FlatFXWebClient.ViewModels
{
    public class SimpleCurrencyExchangeViewModel
    {
        public Deal deal { get; set; }
        public List<string> CurrencyList { get; set; }
        public Dictionary<string, ProviderAccount> userBankAccounts { get; set; }
        public List<string> InvalidAccountReason { get; set; }

        //[Display(Name = "סוג המרה")]
        //public Consts.eSimpleCurrencyExchangeType? SimpleCurrencyExchangeType { get; set; }

        public SimpleCurrencyExchangeViewModel()
        {
            InvalidAccountReason = new List<string>();
            userBankAccounts = null;
        }
        public async Task<bool> Initialize(ApplicationDBContext db)
        {
            //CurrencyList
            // To do load it dynamically
            CurrencyList = new List<string> { "USD", "EUR", "ILS" };

            //get User
            string userId = ApplicationInformation.Instance.GetUserID();
            ApplicationUser user = await db.Users.Include(u => u.Companies).Where(u => u.Id == userId && u.IsActive == true).FirstOrDefaultAsync();
            if (user == null)
                return false;
            if (!ApplicationInformation.Instance.IsDemoUser() && !user.IsApprovedByFlatFX)
                InvalidAccountReason.Add("User is not approved by FlatFX team.");
            
            //get Company
            List<string> userCompaniesIdList = user.Companies.Where(comp => comp.IsActive == true).Select(c => c.CompanyId).ToList();
            List<string> userCompaniesIdListPlusWhere = user.Companies.Where(comp => comp.IsActive == true && comp.IsSignOnRegistrationAgreement == true)
                .Select(c => c.CompanyId).ToList();
            if (userCompaniesIdList.Count > 0 && userCompaniesIdListPlusWhere.Count == 0)
                InvalidAccountReason.Add("Company is not signed on registration agreement.");

            //get Company Account
            List<string> companyAccountsIdList = await db.CompanyAccounts.Where(ca => userCompaniesIdList.Contains(ca.CompanyId) && ca.IsActive == true)
                .Select(ca => ca.CompanyAccountId).ToListAsync();


            List<ProviderAccount> providerAccounts = await db.ProviderAccounts.Include(pa => pa.CompanyAccount).Include(p => p.Provider)
                .Where(pa => companyAccountsIdList.Contains(pa.CompanyAccountId) && pa.IsActive == true).ToListAsync();
            List<ProviderAccount> toRemove = new List<ProviderAccount>();
            foreach (ProviderAccount pa in providerAccounts)
            {
                if (!pa.ApprovedBYFlatFX)
                {
                    InvalidAccountReason.Add("Account is not approved by FlatFX team.");
                    toRemove.Add(pa);
                    continue;
                }
            }
            foreach (ProviderAccount pa in toRemove)
                providerAccounts.Remove(pa);
            userBankAccounts = providerAccounts.ToDictionary(pa => pa.AccountName);

            //Initialize deal
            deal = new Deal();
            if (providerAccounts.Count > 0 && providerAccounts[0].IsDemoAccount == true)
                deal.IsDemo = true;
            if (ApplicationInformation.Instance.IsUserInRole(Consts.Role_CompanyDemoUser))
                deal.IsDemo = true;
            if (providerAccounts.Count > 0)
            {
                deal.ChargedProviderAccount = providerAccounts[0];
                deal.CreditedProviderAccount = providerAccounts[0];
            }
            deal.DealProductType = Consts.eDealProductType.FxSimpleExchange;
            deal.DealType = Consts.eDealType.Spot;
            deal.IsOffer = true;
            if (providerAccounts.Count > 0)
            {
                deal.Provider = providerAccounts[0].Provider;
                deal.CompanyAccount = providerAccounts[0].CompanyAccount;
            }
            deal.user = user;


            return true;
        }
    }
}