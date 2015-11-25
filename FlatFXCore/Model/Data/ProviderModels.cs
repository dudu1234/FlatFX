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
        
        [DisplayName("Short Name"), Index("IX_ShortName", IsUnique = true), MaxLength(10), Required]
        public string ShortName { get; set; }
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

        [Index("IX_AccountName", IsUnique = true)]
        [Display(Name = "AccountName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
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
