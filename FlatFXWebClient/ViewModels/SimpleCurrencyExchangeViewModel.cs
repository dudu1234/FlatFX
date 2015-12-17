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
using System.ComponentModel.DataAnnotations;

namespace FlatFXWebClient.ViewModels
{
    public class SimpleCurrencyExchangeViewModel
    {
        public string OrderKey { get; set; }
        [Display(Name = "Account")]
        [Required]
        public string SelectedAccount { get; set; }
        public List<string> InvalidAccountReason { get; set; }
        [Required]
        public Consts.eBuySell BuySell { get; set; }
        [Required]
        [Display(Name = "Amount")]
        [Range(1000, 1000000, ErrorMessage = "Please enter number between 1,000-1,000,000")]
        public double Amount { get; set; }
        [Required]
        public string CCY1 { get; set; }
        [Required]
        public string CCY2 { get; set; }
        [Display(Name = "Comment")]
        public string Comment { get; set; }


        public SimpleCurrencyExchangeViewModel()
        {
            InvalidAccountReason = new List<string>();
            OrderKey = Guid.NewGuid().ToString();

            // To do load it dynamically
            //PairList
            List<string> CurrencyList = new List<string> { "USD", "EUR", "ILS" };
            ApplicationInformation.Instance.Session["PairList"] = CurrencyList;
        }


        public async Task<bool> Initialize(ApplicationDBContext db)
        {
            CCY1 = "USD";
            CCY2 = "ILS";

            //get User
            string userId = ApplicationInformation.Instance.UserID;
            ApplicationUser user = await db.Users.Include(u => u.Companies).Where(u => u.Id == userId && u.IsActive == true).FirstOrDefaultAsync();
            if (user == null)
                return false;
            if (!ApplicationInformation.Instance.IsDemoUser && !user.IsApprovedByFlatFX)
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
            ApplicationInformation.Instance.Session["UserBankAccounts"] = providerAccounts.Select(pa => new SelectListItem
            {
                Value = pa.ProviderId + "_" + pa.CompanyAccountId,
                Text = pa.CompanyAccount.Company.CompanyName + " - " + pa.AccountName
            });
            SelectedAccount = UserBankAccounts.First().Value;

            //Initialize deal
            Deal deal = new Deal();
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

            ApplicationInformation.Instance.Session[OrderKey] = deal;

            if (InvalidAccountReason.Count == 0)
                InvalidAccountReason = null;

            return true;
        }

        public Deal DealInSession
        {
            get
            {
                if (ApplicationInformation.Instance.Session[OrderKey] != null)
                    return ApplicationInformation.Instance.Session[OrderKey] as Deal;
                else
                    return null;
            }
        }
        public List<string> CurrencyList 
        {
            get 
            {
                if (ApplicationInformation.Instance.Session["PairList"] != null)
                    return ApplicationInformation.Instance.Session["PairList"] as List<string>;
                else
                    return null;
            }
        }
        public IEnumerable<SelectListItem> UserBankAccounts 
        { 
            get
            {
                if (ApplicationInformation.Instance.Session["UserBankAccounts"] != null)
                    return ApplicationInformation.Instance.Session["UserBankAccounts"] as IEnumerable<SelectListItem>;
                else
                    return null;
            }
        }
    }
}