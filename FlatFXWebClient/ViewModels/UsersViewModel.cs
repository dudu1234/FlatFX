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
    public class RegisterUserViewModel
    {
        [Display(Name = "UserName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(40, MinimumLength = 4, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        [System.Web.Mvc.Remote("isFieldUnique", "Account", HttpMethod = "POST", ErrorMessage = "User name already exists. Please enter a different user name.")]
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

        public ContactDetailsViewModel contactDetails = new ContactDetailsViewModel();
    }

    public class RegisterCompanyViewModel
    {
        [Display(Name = "CompanyShortName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(30, MinimumLength = 2, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        [System.Web.Mvc.Remote("isFieldUnique", "Account", HttpMethod = "POST", ErrorMessage = "Company short name already exists. Please enter a different company short name.")]
        public string CompanyShortName { get; set; }
        [Display(Name = "CompanyFullName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(200, MinimumLength = 5, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        [System.Web.Mvc.Remote("isFieldUnique", "Account", HttpMethod = "POST", ErrorMessage = "Company full name already exists. Please enter a different company full name.")]
        public string CompanyFullName { get; set; }
        [Display(Name = "ValidIP", ResourceType = typeof(FlatFXResources.Resources))]
        [StringLength(300, MinimumLength = 14, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string ValidIP { get; set; }
        [Display(Name = "CustomerType", ResourceType = typeof(FlatFXResources.Resources))]
        public Consts.eCustomerType? CustomerType { get; set; }
        [Display(Name = "IsDepositValid", ResourceType = typeof(FlatFXResources.Resources))]
        public bool IsDepositValid { get; set; }
        [Display(Name = "IsSignOnRegistrationAgreement", ResourceType = typeof(FlatFXResources.Resources))]
        public bool IsSignOnRegistrationAgreement { get; set; }
        [Display(Name = "CompanyVolumePerYearUSD", ResourceType = typeof(FlatFXResources.Resources))]
        public Consts.eCompanyVolume? CompanyVolumePerYearUSD { get; set; }
        [Display(Name = "UserList_SendEmail", ResourceType = typeof(FlatFXResources.Resources))]
        [StringLength(800, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string UserList_SendEmail { get; set; }
        [Display(Name = "UserList_SendInvoice", ResourceType = typeof(FlatFXResources.Resources))]
        [StringLength(800, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string UserList_SendInvoice { get; set; }


        public ContactDetailsViewModel contactDetails = new ContactDetailsViewModel();
        public ContactDetailsExViewModel contactDetailsEx = new ContactDetailsExViewModel();
    }

    public class RegisterCompanyAccountViewModel
    {
        [Display(Name = "AccountName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(200, MinimumLength = 5, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        [System.Web.Mvc.Remote("isFieldUnique", "Account", HttpMethod = "POST", ErrorMessage = "Account name already exists. Please enter a different account name.")]
        public string AccountName { get; set; }

        [Display(Name = "AccountFullName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(400, MinimumLength = 5, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        [System.Web.Mvc.Remote("isFieldUnique", "Account", HttpMethod = "POST", ErrorMessage = "Account full name already exists. Please enter a different account full name.")]
        public string AccountFullName { get; set; }

        [Display(Name = "IsDefaultAccount", ResourceType = typeof(FlatFXResources.Resources))]
        public bool IsDefaultAccount { get; set; }
    }

    public class RegisterProviderAccountViewModel
    {
        [Display(Name = "ProviderOrBank", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        public string ProviderId { get; set; }
        [Display(Name = "AccountName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(200, MinimumLength = 5, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string AccountName { get; set; }
        [Display(Name = "AccountFullName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(400, MinimumLength = 5, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string AccountFullName { get; set; }
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
        [Display(Name = "ApprovedBYFlatFX", ResourceType = typeof(FlatFXResources.Resources))]
        public bool ApprovedBYFlatFX { get; set; }
        [Display(Name = "ApprovedBYProvider", ResourceType = typeof(FlatFXResources.Resources))]
        public bool ApprovedBYProvider { get; set; }
        [Display(Name = "IsDemoAccount", ResourceType = typeof(FlatFXResources.Resources))]
        public bool IsDemoAccount { get; set; }
        
        public Dictionary<string, string> Providers = new Dictionary<string, string>();

        public RegisterProviderAccountViewModel()
        {
            using (var context = new FfxContext())
            {
                Providers = context.Providers.Where(p => p.IsActive).ToDictionary(p1 => p1.ProviderId, p2 => p2.FullName);
            }
        }
    }

    public class ContactDetailsViewModel
    {
        [Display(Name = "Email", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [EmailAddress]
        [System.Web.Mvc.Remote("isFieldUnique", "Account", HttpMethod = "POST", ErrorMessage = "Email address already exists. Please enter a different Email address.")]
        public string Email { get; set; }

        [Display(Name = "MobilePhone", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
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

    public class RegisterCompanyEntitiesModelView
    {
        public RegisterUserViewModel UserVM = new RegisterUserViewModel();
        public RegisterCompanyViewModel companyVM = new RegisterCompanyViewModel();
        public RegisterCompanyAccountViewModel companyAccountVM = new RegisterCompanyAccountViewModel();
        public RegisterProviderAccountViewModel providerAccountVM = new RegisterProviderAccountViewModel();
    }
}