using FlatFXCore.BussinessLayer;
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
        public string CompanyShortName { get; set; }
        [Display(Name = "CompanyFullName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(200, MinimumLength = 5, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string CompanyFullName { get; set; }
        [Display(Name = "ValidIP", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(300, MinimumLength = 14, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string ValidIP { get; set; }
        [Display(Name = "CustomerType", ResourceType = typeof(FlatFXResources.Resources))]
        public Consts.eCustomerType? CustomerType { get; set; }
        //[DisplayName("Is Deposit valid")]
        [Display(Name = "IsDepositValid", ResourceType = typeof(FlatFXResources.Resources))]
        public bool? IsDepositValid { get; set; }
        //[DisplayName("Is Sign On Registration Agreement")]
        [Display(Name = "IsSignOnRegistrationAgreement", ResourceType = typeof(FlatFXResources.Resources))]
        public bool? IsSignOnRegistrationAgreement { get; set; }

        public ContactDetailsViewModel contactDetails = new ContactDetailsViewModel();
        public ContactDetailsExViewModel contactDetailsEx = new ContactDetailsExViewModel();
    }

    public class RegisterCompanyAccountViewModel
    {
        [Display(Name = "AccountName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(200, MinimumLength = 5, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string AccountName { get; set; }

        [Display(Name = "AccountFullName", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [StringLength(400, MinimumLength = 5, ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationLength")]
        public string AccountFullName { get; set; }

        [Display(Name = "IsDefaultAccount", ResourceType = typeof(FlatFXResources.Resources))]
        public bool IsDefaultAccount { get; set; }
    }

    public class ContactDetailsViewModel
    {
        [Display(Name = "Email", ResourceType = typeof(FlatFXResources.Resources))]
        [Required(ErrorMessageResourceType = typeof(FlatFXResources.Resources), ErrorMessageResourceName = "ValidationRequired")]
        [EmailAddress]
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
        public string Country { get; set; }

        [Display(Name = "WebSite", ResourceType = typeof(FlatFXResources.Resources))]
        [Url]
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
    }
}