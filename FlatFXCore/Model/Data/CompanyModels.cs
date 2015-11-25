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
    [Table("Companies")]
    public class Company
    {
        [Key]
        public string CompanyId { get; set; }
        [Index("IX_CompanyShortName", IsUnique = true)]
        [Display(Name = "CompanyShortName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(30, MinimumLength = 2, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        //[System.Web.Mvc.Remote("isFieldUnique", "Account", HttpMethod = "POST", ErrorMessage = "Company short name already exists. Please enter a different company short name.")]
        public string CompanyShortName { get; set; }
        [Display(Name = "CompanyFullName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(200, MinimumLength = 5, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        //[System.Web.Mvc.Remote("isFieldUnique", "Account", HttpMethod = "POST", ErrorMessage = "Company full name already exists. Please enter a different company full name.")]
        public string CompanyFullName { get; set; }
        [Required, DisplayName("Is Active")]
        public bool IsActive { get; set; }
        [Required]
        public Consts.eCompanyStatus Status { get; set; }
        [DisplayName("Created At")]
        public DateTime? CreatedAt { get; set; }
        [DisplayName("Last Update")]
        public DateTime? LastUpdate { get; set; }
        [Display(Name = "ValidIP", ResourceType = typeof(FlatFXResources.Resources))]
        [StringLength(300, MinimumLength = 14, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string ValidIP { get; set; }
        [Display(Name = "CustomerType", ResourceType = typeof(FlatFXResources.Resources))]
        public Consts.eCustomerType? CustomerType { get; set; }
        [Display(Name = "IsDepositValid", ResourceType = typeof(FlatFXResources.Resources))]
        [DefaultValue(false)]
        public bool IsDepositValid { get; set; }
        [Display(Name = "IsSignOnRegistrationAgreement", ResourceType = typeof(FlatFXResources.Resources))]
        [DefaultValue(false)]
        public bool IsSignOnRegistrationAgreement { get; set; }
        [Display(Name = "CompanyVolumePerYearUSD", ResourceType = typeof(FlatFXResources.Resources))]
        public Consts.eCompanyVolume? CompanyVolumePerYearUSD { get; set; }
        [Display(Name = "UserList_SendEmail", ResourceType = typeof(FlatFXResources.Resources))]
        [StringLength(800, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string UserList_SendEmail { get; set; }
        [Display(Name = "UserList_SendInvoice", ResourceType = typeof(FlatFXResources.Resources))]
        [StringLength(800, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string UserList_SendInvoice { get; set; }

        public ContactDetails ContactDetails { get; set; }

        public virtual ICollection<CompanyAccount> Accounts { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

        public Company() 
        {
            CompanyId = Guid.NewGuid().ToString();
            CreatedAt = DateTime.Now;
            IsActive = true;
            Status = Consts.eCompanyStatus.Active;
            IsDepositValid = false;
            IsSignOnRegistrationAgreement = false;
            LastUpdate = DateTime.Now;                

            ContactDetails = new ContactDetails();
            Users = new List<ApplicationUser>();
            Accounts = new List<CompanyAccount>();
        }
    }
    [Table("CompanyAccounts")]
    public class CompanyAccount
    {
        [Key]
        public string CompanyAccountId { get; set; }

        public string CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }


        [Index("IX_AccountName", IsUnique = true)]
        [Display(Name = "AccountName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(200, MinimumLength = 5, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        //[System.Web.Mvc.Remote("isFieldUnique", "Account", HttpMethod = "POST", ErrorMessage = "Account name already exists. Please enter a different account name.")]
        public string AccountName { get; set; }
        [Required, DisplayName("Is Active")]
        public bool IsActive { get; set; }
        [Required]
        [Display(Name = "IsDefaultAccount", ResourceType = typeof(FlatFXResources.Resources))]
        public bool IsDefaultAccount { get; set; }

        public CompanyAccount() 
        {
            IsActive = true;
            IsDefaultAccount = true;        
            CompanyAccountId = Guid.NewGuid().ToString();
        }
    }
    [Table("Spreads")]
    public class SpreadInfo
    {
        [Key, Column(Order = 1, TypeName = "VARCHAR"), MaxLength(10)]
        public string Key { get; set; }
        [ForeignKey("Key")]
        public virtual FXRate FXRate { get; set; }

        [Key, Column(Order = 2)]
        public string CompanyId { get; set; }
        [ForeignKey("CompanyId")]
        public virtual Company Company { get; set; }

        [Key, Column(Order = 3)]
        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual Provider Provider { get; set; }

        [Key, Column(Order = 4), Required]
        public bool IsStartingSpread { get; set; }

        [Required]
        public int Spread { get; set; }

        public SpreadInfo() 
        {
        }
    }
}
