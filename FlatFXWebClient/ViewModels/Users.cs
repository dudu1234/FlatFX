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
}