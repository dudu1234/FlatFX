using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.User;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFXCore.Model.Data
{
    [Table("Providers")]
    public class Provider
    {
        [Key]
        public string ProviderId { get; set; }

        [Index("IX_ProviderName", IsUnique = true)]
        [DisplayName("Name"), MaxLength(20), Required]
        public string Name { get; set; }
        [DisplayName("Full Name"), Index("IX_FullName", IsUnique = true), MaxLength(200), Required]
        public string FullName { get; set; }

        [DisplayName("Bank Number"), Required]
        public int BankNumber { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public Consts.eProviderStatus Status { get; set; }
        [DisplayName("Provider Type"), Required]
        public Consts.eProviderType ProviderType { get; set; }

        [DisplayName("Is enabled"), Required]
        public bool QuoteResponse_Enabled { get; set; }
        [DisplayName("Spread method"), Required]
        public Consts.eQuoteResponseSpreadMethod QuoteResponse_SpreadMethod { get; set; }
        [DisplayName("Daily start time"), Required]
        //[DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:hh\\:mm}", ApplyFormatInEditMode = true)]
        //[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:HH\\:mm\\:ss}")]
        public DateTime QuoteResponse_StartTime { get; set; }
        [DisplayName("Daily end time"), Required]
        public DateTime QuoteResponse_EndTime { get; set; }
        [DisplayName("Friday start time"), Required]
        public DateTime QuoteResponse_FridayStartTime { get; set; }
        [DisplayName("Friday end time"), Required]
        public DateTime QuoteResponse_FridayEndTime { get; set; }

        [DisplayName("user confirmation time interval"), Required]
        public Int16 QuoteResponse_UserConfirmationTimeInterval { get; set; }
        [DisplayName("Automatic response enabled"), Required]
        public bool QuoteResponse_AutomaticResponseEnabled { get; set; }
        [DisplayName("Min request volume (USD)"), Required]
        public int QuoteResponse_MinRequestVolumeUSD { get; set; }
        [DisplayName("Max daily volume (USD)"), Required]
        public int QuoteResponse_MaxDailyVolumeUSD { get; set; }
        [DisplayName("Number of promils without discount"), Required]
        public double QuoteResponse_NumberOfPromilsWithoutDiscount { get; set; }

        public virtual ICollection<ProviderAccount> Accounts { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        public ContactDetails ContactDetails { get; set; }

        public Provider()
        {
            ProviderId = Guid.NewGuid().ToString();
            IsActive = true;
            Status = FlatFXCore.BussinessLayer.Consts.eProviderStatus.Active;

            ContactDetails = new ContactDetails();
            Accounts = new List<ProviderAccount>();
            Users = new List<ApplicationUser>();
        }

        private static Dictionary<string, string> _ProviderList = null;
        public static Dictionary<string, string> ProviderList
        {
            get
            {
                if (_ProviderList == null)
                {
                    _ProviderList = new Dictionary<string, string>();
                    using (var context = new ApplicationDBContext())
                    {
                        if (context.Providers.Where(p => p.IsActive).Any())
                            _ProviderList = context.Providers.Where(p => p.IsActive).ToDictionary(p1 => p1.ProviderId, p2 => p2.FullName);
                    }
                }
                return _ProviderList;
            }
        }

        private static Dictionary<string, string> _BankList = null;
        public static Dictionary<string, string> BankList
        {
            get
            {
                if (_BankList == null)
                {
                    _BankList = new Dictionary<string, string>();
                    using (var context = new ApplicationDBContext())
                    {
                        if (context.Providers.Where(p => p.IsActive).Any())
                            _BankList = context.Providers.Where(p => p.IsActive && p.ProviderType == Consts.eProviderType.Bank).ToDictionary(p1 => p1.ProviderId, p2 => p2.FullName);
                    }
                }
                return _BankList;
            }
        }
        public bool IsSimpleExchangeEnabled(out string errorMessage)
        {
            errorMessage = null;
            if (!this.IsActive)
                errorMessage = "Provider is not active." + Environment.NewLine;
            if (this.Name != "FlatFX")
                errorMessage = "Provider must be FlatFX." + Environment.NewLine;
            if (!this.QuoteResponse_Enabled)
                errorMessage = "Quote Response is not enable." + Environment.NewLine;
            if (!this.QuoteResponse_AutomaticResponseEnabled)
                errorMessage = "Automatic Quote Response is not enable." + Environment.NewLine;
            if (this.Status == Consts.eProviderStatus.Blocked)
                errorMessage = "Provider status is blocked." + Environment.NewLine;

            //Check working hours
            int startHour, startMinute, endHour, endMinute;
            if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
            {
                startHour = this.QuoteResponse_FridayStartTime.Hour;
                startMinute = this.QuoteResponse_FridayStartTime.Minute;
                endHour = this.QuoteResponse_FridayEndTime.Hour;
                endMinute = this.QuoteResponse_FridayEndTime.Minute;
            }
            else
            {
                startHour = this.QuoteResponse_StartTime.Hour;
                startMinute = this.QuoteResponse_StartTime.Minute;
                endHour = this.QuoteResponse_EndTime.Hour;
                endMinute = this.QuoteResponse_EndTime.Minute;
            }

            DateTime start = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startHour, startMinute, 0);
            DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, endHour, endMinute, 0);

            if (!ApplicationInformation.Instance.IsDemoUser)
            {
                if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday || DateTime.Now < start || DateTime.Now > end)
                {
                    errorMessage = "Currency Exchange is not available right now, " + Environment.NewLine +
                        "Office Hours: " + Environment.NewLine +
                        "Monday - Thursday: " + this.QuoteResponse_StartTime.Hour + ":" + this.QuoteResponse_StartTime.Minute.ToString().PadLeft(2, '0') + " - " +
                        this.QuoteResponse_EndTime.Hour + ":" + this.QuoteResponse_EndTime.Minute.ToString().PadLeft(2, '0') + Environment.NewLine +
                        "Friday: " + this.QuoteResponse_FridayStartTime.Hour + ":" + this.QuoteResponse_FridayStartTime.Minute.ToString().PadLeft(2, '0') + " - " +
                        this.QuoteResponse_FridayEndTime.Hour + ":" + this.QuoteResponse_FridayEndTime.Minute.ToString().PadLeft(2, '0') + Environment.NewLine;
                }
            }

            if (errorMessage == null)
                return true;
            else
                return false;
        }
        public bool GetSpread(ApplicationDBContext db, string companyId, string pairKey, out double spread, out string errorMessage)
        {
            errorMessage = null;
            spread = 0;

            try
            {
                //level 1 - provider promil
                SpreadInfo providerSpreadInfo = db.Spreads.Where(s => s.ProviderId == ProviderId).SingleOrDefault();
                if (providerSpreadInfo == null || !providerSpreadInfo.Promil.HasValue)
                {
                    if (this.QuoteResponse_SpreadMethod == Consts.eQuoteResponseSpreadMethod.PromilPerProvider)
                        errorMessage = "Failed to find provider spread information, please contact FlatFX team.";
                }
                else
                {
                    double promil = providerSpreadInfo.Promil.Value;
                    if (CurrencyManager.Instance.PairRates.ContainsKey(pairKey))
                        spread = 0.0001 * promil * CurrencyManager.Instance.PairRates[pairKey].Mid;
                }

                //To Do add the other SpreadMethods logic

                //level 2 - provider spread per pair

                //level 3 - customer promil

                //level 4 - customer spread per pair

                if (spread == 0 || errorMessage != null)
                    return false;
                else
                    return true;
            }
            catch(Exception ex)
            {
                Logger.Instance.WriteError("Failed in Provider::GetSpread", ex);
                errorMessage = "Failed to generate customer spread." + Environment.NewLine;
                return false;
            }
        }
    }
    [Table("ProviderAccounts")]
    public class ProviderAccount
    {
        [Key, Column(Order = 1)]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        public string CompanyAccountId { get; set; }
        [ForeignKey("CompanyAccountId")]
        public virtual CompanyAccount CompanyAccount { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "ProviderOrBank", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual Provider Provider { get; set; }

        [Display(Name = "AccountName", ResourceType = typeof(FlatFXResources.Resources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string AccountName { get; set; }
        [Display(Name = "BankAccountName", ResourceType = typeof(FlatFXResources.Resources))]
        [StringLength(100, MinimumLength = 2, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string BankAccountName { get; set; }
        [Display(Name = "BranchNumber", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(10, MinimumLength = 3, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string BankBranchNumber { get; set; }
        [Display(Name = "AccountNumber", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(20, MinimumLength = 6, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string BankAccountNumber { get; set; }
        [Display(Name = "Address", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(200, MinimumLength = 6, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string BankAddress { get; set; }
        [Display(Name = "IBAN", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(30, MinimumLength = 10, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string IBAN { get; set; }
        [Display(Name = "SWIFT", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(10, MinimumLength = 6, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string SWIFT { get; set; }
        [Display(Name = "AllowToTradeDirectlly", ResourceType = typeof(FlatFXResources.Resources))]
        public bool AllowToTradeDirectlly { get; set; }
        [DisplayName("Approved by FlatFX")]
        public bool ApprovedBYFlatFX { get; set; }
        [DisplayName("Approved by Provider")]
        public bool ApprovedBYProvider { get; set; }
        [DisplayName("user key in provider systems")]
        public string UserKeyInProviderSystems { get; set; }

        [Required]
        [DisplayName("Is active")]
        public bool IsActive { get; set; }
        [DisplayName("Is demo account")]
        public bool IsDemoAccount { get; set; }
        [DisplayName("Created at")]
        public DateTime? CreatedAt { get; set; }
        [DisplayName("Last update")]
        public DateTime? LastUpdate { get; set; }
        [DisplayName("Last update by")]
        public string LastUpdateBy { get; set; }

        [Required]
        [DisplayName("Is blocked")]
        public bool QuoteResponse_IsBlocked { get; set; }
        [DisplayName("Quote Response customer promil")]
        public double? QuoteResponse_CustomerPromil { get; set; }

        public virtual ICollection<Deal> Deals { get; set; }

        public ProviderAccount()
        {
            ApprovedBYFlatFX = false;
            ApprovedBYProvider = false;
            CreatedAt = DateTime.Now;
            IsActive = true;
            LastUpdate = DateTime.Now;
            QuoteResponse_CustomerPromil = null;
            QuoteResponse_IsBlocked = false;
        }
    }
}
