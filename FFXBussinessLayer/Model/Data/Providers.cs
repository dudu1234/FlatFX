using FlatFX.BussinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFX.Model.Data
{
    [Table("Providers")]
    public class ProviderData
    {
        [Key]
        public int ProviderId { get; set; }
        
        [Index("IX_ShortName", IsUnique = true), MaxLength(10), Required]
        public string ShortName { get; set; }
        [Index("IX_FullName", IsUnique = true), MaxLength(200), Required]
        public string FullName { get; set; }

        [Required]
        public int BankNumber { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public Consts.eProviderStatus Status { get; set; }
        [Required]
        public Consts.eProviderType ProviderType { get; set; }

        [Required]
        public bool QuoteResponse_Enabled { get; set; }
        [Required]
        public Consts.eQuoteResponseSpreadMethod QuoteResponse_SpreadMethod { get; set; }
        [Required]
        public DateTime QuoteResponse_StartTime { get; set; }
        [Required]
        public DateTime QuoteResponse_EndTime { get; set; }
        [Required]
        public DateTime QuoteResponse_FridayStartTime { get; set; }
        [Required]
        public DateTime QuoteResponse_FridayEndTime { get; set; }
    
        [Required]
        public Int16 QuoteResponse_UserConfirmationTimeInterval { get; set; }
        [Required]
        public bool QuoteResponse_AutomaticResponseEnabled { get; set; }
        [Required]
        public int QuoteResponse_MinRequestVolumeUSD { get; set; }
        [Required]
        public int QuoteResponse_MaxDailyVolumeUSD { get; set; }
        [Required]
        public double QuoteResponse_NumberOfPromilsWithoutDiscount { get; set; }

        public virtual ICollection<ProviderAccountData> Accounts { get; set; }
        public virtual ICollection<UserData> Users { get; set; }

        public int ContactDetailsId { get; set; }
        [ForeignKey("ContactDetailsId")]
        public ContactDetails ContactDetails { get; set; }

        public ProviderData() { }
    }
    [Table("ProviderAccounts")]
    public class ProviderAccountData
    {
        [Key, Column(Order = 1)]
        public int CompanyAccountId { get; set; }
        [ForeignKey("CompanyAccountId")]
        public virtual CompanyAccountData CompanyAccount { get; set; }
        [Key, Column(Order = 2)]
        public int ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual ProviderData Provider { get; set; }

        [Index("IX_BankAccountName", IsUnique = true), MaxLength(200), Required]
        public string BankAccountName { get; set; }
        [MaxLength(400)]
        public string BankAccountFullName { get; set; }
        [MaxLength(10)]
        public string BankBranchNumber { get; set; }
        [MaxLength(20)]
        public string BankAccountNumber { get; set; }

        
        [Required]
        public bool IsActive { get; set; }
        
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string LastUpdateBy { get; set; }

        [Required]
        public bool QuoteResponse_IsBlocked { get; set; }
        public double? QuoteResponse_CustomerPromil { get; set; }
        
        public ProviderAccountData() { }
    }
}
