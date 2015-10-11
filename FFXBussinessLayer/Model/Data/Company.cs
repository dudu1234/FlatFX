﻿using FlatFXCore.BussinessLayer;
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
    [Table("Companies")]
    public class CompanyData
    {
        [Key]
        public string CompanyId { get; set; }
        [Index("IX_CompanyShortName", IsUnique = true), MaxLength(20), Required]
        public string CompanyShortName { get; set; }
        [MaxLength(400), Required]
        public string CompanyFullName { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public Consts.eCompanyStatus Status { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? LastUpdate { get; set; }
        [MaxLength(300), Description("Valid users IP, Seperated by ;")]
        public string ValidIP { get; set; }
        public Consts.eCustomerType? CustomerType { get; set; }
        public bool? IsDepositValid { get; set; }
        public bool? IsSignOnRegistrationAgreement { get; set; }

        public string ContactDetailsId { get; set; }
        [ForeignKey("ContactDetailsId")]
        public ContactDetails ContactDetails { get; set; }

        public virtual ICollection<CompanyAccountData> Accounts { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
       
        public CompanyData() { }
    }
    [Table("CompanyAccounts")]
    public class CompanyAccountData
    {
        [Key]
        public string CompanyAccountId { get; set; }
      
        [Index("IX_AccountName", IsUnique = true), MaxLength(200), Required]
        public string AccountName { get; set; }
      
        public string CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual CompanyData Company { get; set; }

        [MaxLength(400), Required]
        public string AccountFullName { get; set; }
        [Required]
        public bool IsActive { get; set; }
        [Required]
        public bool IsDefaultAccount { get; set; }
        
        [Required]
        public double Balance { get; set; }
        [Required]
        public double Equity { get; set; }
        [Required]
        public double DailyPNL { get; set; }
        [Required]
        public double GrossPNL { get; set; }

        public CompanyAccountData() { }
    }
    [Table("Spreads")]
    public class SpreadData
    {
        [Key, Column(Order = 1, TypeName = "VARCHAR"), MaxLength(10)]
        public string Key { get; set; }
        [ForeignKey("Key")]
        public virtual FXRateData FXRate { get; set; }

        [Key, Column(Order = 2)]
        public string CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual CompanyData Company { get; set; }

        [Key, Column(Order = 3)]
        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual ProviderData Provider { get; set; }

        [Key, Column(Order = 4), Required]
        public bool IsStartingSpread { get; set; }
        
        [Required]
        public int Spread { get; set; }
        
        public SpreadData() { }
    }
}
