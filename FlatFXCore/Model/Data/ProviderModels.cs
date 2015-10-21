using FlatFXCore.BussinessLayer;
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

        [DisplayName("User confirmation time interval"), Required]
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

        public string ContactDetailsId { get; set; }
        [ForeignKey("ContactDetailsId")]
        public ContactDetails ContactDetails { get; set; }

        public Provider() 
        {
            ContactDetails = new ContactDetails();
        }
    }
    [Table("ProviderAccounts")]
    public class ProviderAccount
    {
        [Key, Column(Order = 1)]
        public string CompanyAccountId { get; set; }
        [ForeignKey("CompanyAccountId")]
        public virtual CompanyAccount CompanyAccount { get; set; }
        [Key, Column(Order = 2)]
        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual Provider Provider { get; set; }

        [Index("IX_BankAccountName", IsUnique = true), MaxLength(200), Required]
        public string BankAccountName { get; set; }
        [MaxLength(400)]
        public string BankAccountFullName { get; set; }
        [MaxLength(10)]
        public string BankBranchNumber { get; set; }
        [MaxLength(20)]
        public string BankAccountNumber { get; set; }

        [MaxLength(200)]
        public string BankAddress { get; set; }
        [MaxLength(30)]
        public string IBAN { get; set; }
        [MaxLength(10)]
        public string SWIFT { get; set; }

        public bool ApprovedBYFlatFX { get; set; }
        public bool ApprovedBYProvider { get; set; }

        [Required]
        public bool IsActive { get; set; }
        public bool IsDemoAccount { get; set; }
        
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string LastUpdateBy { get; set; }

        [Required]
        public bool QuoteResponse_IsBlocked { get; set; }
        public double? QuoteResponse_CustomerPromil { get; set; }
        
        public ProviderAccount() { }
    }
}
