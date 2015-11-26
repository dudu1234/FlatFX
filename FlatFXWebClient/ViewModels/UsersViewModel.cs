using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FlatFXWebClient.ViewModels
{
    public class CompanyUserAllEntitiesModelView 
    {
        public UserDetailsViewModel UserVM { get; set; }
        public CompanyDetailsViewModel companyVM { get; set; }
        public ProviderAccountDetailsViewModel providerAccountVM { get; set; }

        public bool ShowContactDetailsForUser = true; // false = show for company

        public string companyAccountParent = null;

        public CompanyUserAllEntitiesModelView()
        {
            UserVM = new UserDetailsViewModel();
            companyVM = new CompanyDetailsViewModel();
            providerAccountVM = new ProviderAccountDetailsViewModel();
        }
    }

    public class UserDetailsViewModel
    {
        [Display(Name = "UserName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(40, MinimumLength = 4, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        //[System.Web.Mvc.Remote("isFieldUnique", "Account", HttpMethod = "POST", ErrorMessage = "user name already exists. Please enter a different user name.")]
        public string UserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(100, MinimumLength = 1, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(FlatFXResources.Resources))]
        public string Password { get; set; }

        [Display(Name = "ConfirmPassword", ResourceType = typeof(FlatFXResources.Resources))]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "UserComfirmPasswordValidation")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "FirstName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(50, MinimumLength = 1, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string FirstName { get; set; }

        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [Display(Name = "LastName", ResourceType = typeof(FlatFXResources.Resources))]
        [StringLength(50, MinimumLength = 1, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string LastName { get; set; }

        [Display(Name = "Role", ResourceType = typeof(FlatFXResources.Resources))]
        [StringLength(50, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string Role { get; set; }

        public ContactDetailsViewModel userContactDetails { get; set; }

        public UserDetailsViewModel()
        {
            userContactDetails = new ContactDetailsViewModel();
        }
    }

    public class CompanyDetailsViewModel
    {
        [Display(Name = "CompanyName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(30, MinimumLength = 2, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string CompanyName { get; set; }
        [Display(Name = "CompanyFullName", ResourceType = typeof(FlatFXResources.Resources))]
        [StringLength(200, MinimumLength = 5, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        //[System.Web.Mvc.Remote("isFieldUnique", "Account", HttpMethod = "POST", ErrorMessage = "Company full name already exists. Please enter a different company full name.")]
        public string CompanyFullName { get; set; }
        [Display(Name = "ValidIP", ResourceType = typeof(FlatFXResources.Resources))]
        [StringLength(300, MinimumLength = 14, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string ValidIP { get; set; }
        [Display(Name = "CustomerType", ResourceType = typeof(FlatFXResources.Resources))]
        public Consts.eCustomerType? CustomerType { get; set; }
        [Display(Name = "CompanyVolumePerYearUSD", ResourceType = typeof(FlatFXResources.Resources))]
        public Consts.eCompanyVolume? CompanyVolumePerYearUSD { get; set; }
        public ContactDetailsViewModel companyContactDetails { get; set; }

        public CompanyDetailsViewModel()
        {
            companyContactDetails = new ContactDetailsViewModel();
        }
    }

    public class ProviderAccountDetailsViewModel
    {
        [Display(Name = "Bank", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        public string ProviderId { get; set; }
        [Display(Name = "BranchNumber", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(10, MinimumLength = 3, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string BranchNumber { get; set; }
        [Display(Name = "AccountNumber", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(20, MinimumLength = 6, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string AccountNumber { get; set; }
        [Display(Name = "Address", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(200, MinimumLength = 6, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string Address { get; set; }
        [Display(Name = "IBAN", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(30, MinimumLength = 10, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string IBAN { get; set; }
        [Display(Name = "SWIFT", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(10, MinimumLength = 6, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string SWIFT { get; set; }
        
        public ProviderAccountDetailsViewModel()
        {
        }
    }

    public class ContactDetailsViewModel
    {
        [Display(Name = "Email", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "MobilePhone", ResourceType = typeof(FlatFXResources.Resources))]
        //[Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [Phone]
        public string MobilePhone { get; set; }

        [Display(Name = "OfficePhone", ResourceType = typeof(FlatFXResources.Resources))]
        [Phone]
        public string OfficePhone { get; set; }

        [Display(Name = "Fax", ResourceType = typeof(FlatFXResources.Resources))]
        [Phone]
        public string Fax { get; set; }

        [Display(Name = "Country", ResourceType = typeof(FlatFXResources.Resources))]
        public Consts.eCountries? Country { get; set; }

        [Display(Name = "WebSite", ResourceType = typeof(FlatFXResources.Resources))]
        public string WebSite { get; set; }

        public ContactDetailsExViewModel contactDetailsEx { get; set; }
        
        public ContactDetailsViewModel()
        {
            contactDetailsEx = new ContactDetailsExViewModel();
        }
    }

    public class ContactDetailsExViewModel
    {
        [Display(Name = "Address", ResourceType = typeof(FlatFXResources.Resources))]
        public string Address { get; set; }

        [Display(Name = "Email2", ResourceType = typeof(FlatFXResources.Resources))]
        [EmailAddress]
        public string Email2 { get; set; }

        [Display(Name = "MobilePhone2", ResourceType = typeof(FlatFXResources.Resources))]
        [Phone]
        public string MobilePhone2 { get; set; }

        [Display(Name = "OfficePhone2", ResourceType = typeof(FlatFXResources.Resources))]
        [Phone]
        public string OfficePhone2 { get; set; }

        [Display(Name = "CarPhone", ResourceType = typeof(FlatFXResources.Resources))]
        [Phone]
        public string CarPhone { get; set; }

        [Display(Name = "HomePhone", ResourceType = typeof(FlatFXResources.Resources))]
        [Phone]
        public string HomePhone { get; set; }
    }
    public class DemoUserDetailsModelView
    {
        [Display(Name = "Email", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [EmailAddress]
        //[System.Web.Mvc.Remote("isFieldUnique", "Account", HttpMethod = "POST", ErrorMessage = "Email address already exists. Please enter a different Email address.")]
        public string Email { get; set; }
        
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(100, MinimumLength = 1, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(FlatFXResources.Resources))]
        public string Password { get; set; }

        [Display(Name = "MobilePhone", ResourceType = typeof(FlatFXResources.Resources))]
        [Phone]
        public string MobilePhone { get; set; }
    }
}