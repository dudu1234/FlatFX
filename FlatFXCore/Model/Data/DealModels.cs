using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Security.Principal;
using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.User;

namespace FlatFXCore.Model.Data
{
    [Table("Deals")]
    public class Deal
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int64 DealId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser user { get; set; }

        public string CompanyAccountId { get; set; }
        [ForeignKey("CompanyAccountId")]
        public virtual CompanyAccount CompanyAccount { get; set; }

        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual Provider Provider { get; set; }

        [Display(Name="חשבון לזיכוי")]
        public virtual ProviderAccount CreditedProviderAccount { get; set; }
        [Display(Name = "חשבון לחיוב")]
        public virtual ProviderAccount ChangedProviderAccount { get; set; }

        public string ProviderUserId { get; set; }
        [ForeignKey("ProviderUserId")]
        public virtual ApplicationUser ProviderUser { get; set; }

        [MaxLength(100)]
        public string ProviderDealRef { get; set; }

        public Int64 QueryId { get; set; }
        [ForeignKey("QueryId")]
        public virtual QueryData Query { get; set; }

        public Consts.eDealType DealType { get; set; }
        [Display(Name="סוג המרה")]
        public Consts.eSimpleCurrencyExchangeType? SimpleCurrencyExchangeType { get; set; }

        public Consts.eBuySell BuySell { get; set; }

        [Required, Column(TypeName = "VARCHAR"), MaxLength(10)]
        public string Symbol { get; set; }
        public string CreditedCurrency { get; set; }
        public string ChargedCurrency { get; set; }

        public double? Amount1 { get; set; }
        public double? Amount2 { get; set; }
        [Required]
        public double AmountUSD { get; set; }
        [Required]
        public double CustomerRate { get; set; }
        public double? BankRate { get; set; }
        public double? MidRate { get; set; }
        public DateTime OfferingDate { get; set; }
        [Required]
        public DateTime ContractDate { get; set; }
        public DateTime? MaturityDate { get; set; }
        public Consts.eCallPut CallPut { get; set; }
        public double? Commission { get; set; }
        [MaxLength(500)]
        public string Comment { get; set; }
        public bool IsDelivery { get; set; }
        public bool IsExotic { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsOffer { get; set; }
        public bool IsDemo { get; set; }

	    public double? CustomerTotalProfitUSD { get; set; }
        public double? FlatFXTotalProfitUSD { get; set; }
        public double? BankTotalProfitUSD { get; set; }

        //to do: loads it dynamically from the database
        public List<string> CurrencyList = new List<string> { "USD", "EUR" };
        public Dictionary<string, ProviderAccount> userBankAccounts = null;

        //Simple Currency Exchange
        [Display(Name = "סכום להמרה במטבע הנרכש")]
        public double AmountToExchangeCreditedCurrency { get; set; }
        [Display(Name = "סכום להמרה במטבע הרוכש")]
        public double AmountToExchangeChargedCurrency { get; set; }

        public Deal() 
        {
            //Set the default values
            IsCanceled = false;
            IsExotic = false;
            IsDelivery = false;
            IsOffer = false;
            IsDemo = false;
        }

        private async Task<Dictionary<string, ProviderAccount>> UpdateUserBankAccountsAsync()
        {
            if (userBankAccounts == null)
            {
                using (var db = new ApplicationDBContext())
                {
                    string userId = ApplicationInformation.Instance.GetUserID();
                    ApplicationUser user = db.Users.Find(userId);

                    //to do fix the function with the right data
                    userBankAccounts = await db.ProviderAccounts.ToDictionaryAsync(pa => pa.CompanyAccountId + "___" + pa.ProviderId);

                    //await db.ProviderAccounts.Include(pa => pa.CompanyAccount).Join()
                    //userBankAccounts = await db.ProviderAccounts.Include(pa => pa.CompanyAccount).Where(pa => pa.CompanyAccount.Company.Users.Contains(user)).
                    //    ToDictionaryAsync(pa => pa.CompanyAccountId + "___" + pa.ProviderId);
                }
            }
            return userBankAccounts;
        }

        /// <summary>
        /// Initialize the empty deal as "SimpleCurrencyExchange" according to the current user
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InitializeSimpleCurrencyExchangeAsync()
        {
            using (var db = new ApplicationDBContext())
            {
                await InitializeBasicDealAsync(db);
                DealType = Consts.eDealType.SimpleCurrencyExchange;
                IsOffer = true;
            }
            return true;
        }
        /// <summary>
        /// Initialize the empty deal as "SimpleCurrencyExchange" according to the current user
        /// </summary>
        /// <returns></returns>
        public async Task<bool> InitializeBasicDealAsync(ApplicationDBContext db)
        {
            this.UserId = ApplicationInformation.Instance.GetUserID();
            this.user = db.Users.Find(UserId);
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            IsDemo = context.User.IsInRole(Consts.Role_CompanyDemoUser);
            await UpdateUserBankAccountsAsync();
            return true;
        }
    }
    [Table("Queries")]
    public class QueryData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int64 QueryId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public string CompanyAccountId { get; set; }
        [ForeignKey("CompanyAccountId")]
        public virtual CompanyAccount CompanyAccount { get; set; }

        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual Provider Provider { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }        
        [Required, Column(TypeName = "VARCHAR"), MaxLength(10)]
        public string Symbol { get; set; }
        public Consts.eDealType DealType { get; set; }
        public Consts.eBuySell BuySell { get; set; }
        [Required]
        public double AmountCCY1 { get; set; }
        public double? AmountUSD { get; set; }

        public QueryData() { }
    }
    [Table("QueriesPerProvider")]
    public class QueryPerProvider
    {
        [Key, Column(Order = 1)]
        public Int64 QueryId { get; set; }
        [ForeignKey("QueryId")]
        public virtual QueryData Query { get; set; }

        [Key, Column(Order = 2)]
        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual Provider Provider { get; set; }

        public double? ProviderApprovedBuyRate { get; set; }
        public double? ProviderApprovedSellRate { get; set; }
        public Consts.eProviderLastChatAction? ProviderLastAction { get; set; }
        public Consts.eCustomerLastChatAction? CustomerLastAction { get; set; }
        public DateTime? BankResponseTime { get; set; }
        public DateTime? UserResponseTime { get; set; }

        public QueryPerProvider() { }
    }
}
