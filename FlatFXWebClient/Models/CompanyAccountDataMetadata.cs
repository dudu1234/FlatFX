using System;
using System.ComponentModel.DataAnnotations;

namespace FlatFXCore.Model.Data
{
    [MetadataType(typeof(CompanyAccountDataMetadata))]
    public partial class CompanyAccountData
    {
    }

    public partial class CompanyAccountDataMetadata
    {
        [Display(Name = "Company")]
        public CompanyData Company { get; set; }

        [Required(ErrorMessage = "Please enter : CompanyAccountId")]
        [Display(Name = "CompanyAccountId")]
        public string CompanyAccountId { get; set; }

        [Required(ErrorMessage = "Please enter : AccountName")]
        [Display(Name = "AccountName")]
        public string AccountName { get; set; }

        [Required(ErrorMessage = "Please enter : CompanyId")]
        [Display(Name = "CompanyId")]
        public string CompanyId { get; set; }

        [Display(Name = "AccountFullName")]
        public string AccountFullName { get; set; }

        [Display(Name = "IsActive")]
        public bool IsActive { get; set; }

        [Display(Name = "IsDefaultAccount")]
        public bool IsDefaultAccount { get; set; }

        [Display(Name = "Balance")]
        public double Balance { get; set; }

        [Display(Name = "Equity")]
        public double Equity { get; set; }

        [Display(Name = "DailyPNL")]
        public double DailyPNL { get; set; }

        [Display(Name = "GrossPNL")]
        public double GrossPNL { get; set; }

    }
}
