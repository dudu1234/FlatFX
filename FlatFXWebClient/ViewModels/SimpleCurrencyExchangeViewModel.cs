using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;
using FlatFXCore.Model.User;
using System.ComponentModel.DataAnnotations;

namespace FlatFXWebClient.ViewModels
{
    public class SimpleCurrencyExchangeViewModel
    {
        public int WorkflowStage { get; set; }
        public string OrderKey { get; set; }
        [Display(Name = "Account")]
        [Required]
        public string SelectedAccount { get; set; }
        public List<string> InvalidAccountReason { get; set; }
        [Required]
        public Consts.eBuySell BuySell { get; set; }
        [Required]
        [Display(Name = "Amount")]
        [Range(1000, 1000000, ErrorMessage = "Please enter number between 1,000-1,000,000")]
        public double Amount { get; set; }
        [Required]
        public string CCY1 { get; set; }
        [Required]
        public string CCY2 { get; set; }
        [Display(Name = "Comment")]
        public string Comment { get; set; }


        public SimpleCurrencyExchangeViewModel()
        {
            InvalidAccountReason = new List<string>();
            OrderKey = Guid.NewGuid().ToString();
        }

        public Deal DealInSession
        {
            get
            {
                if (ApplicationInformation.Instance.Session[OrderKey] != null)
                    return ApplicationInformation.Instance.Session[OrderKey] as Deal;
                else
                    return null;
            }
        }
        public IEnumerable<SelectListItem> UserBankAccounts 
        { 
            get
            {
                if (ApplicationInformation.Instance.Session["UserBankAccounts"] != null)
                    return ApplicationInformation.Instance.Session["UserBankAccounts"] as IEnumerable<SelectListItem>;
                else
                    return null;
            }
        }
    }
}