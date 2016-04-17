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
        public long DealId { get; set; }
        [Display(Name = "Account", ResourceType = typeof(FlatFXResources.Resources))]
        [Required]
        public string SelectedAccount { get; set; }
        public bool EnsureOnLinePrice { get; set; }
        public bool PvPEnabled { get; set; }
        public bool FastTransferEnabled { get; set; }
        public List<string> InvalidAccountReason { get; set; }
        [Required]
        public Consts.eBuySell BuySell { get; set; }
        [Required]
        [Display(Name = "Amount")]
        [Range(1000, 1000000, ErrorMessage = "Please enter number between 1,000-1,000,000")]
        [DisplayFormat(DataFormatString = "{0:0,0}")]
        public double Amount { get; set; }
        [Required]
        public string CCY1 { get; set; }
        [Required]
        public string CCY2 { get; set; }
        [Display(Name = "Comment", ResourceType = typeof(FlatFXResources.Resources))]
        public string Comment { get; set; }
        public Deal deal { get; set; }
        
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

    public class OrderViewModel
    {
        public int WorkflowStage { get; set; }
        public string OrderKey { get; set; }
        public long OrderId { get; set; }
        [Display(Name = "Account", ResourceType = typeof(FlatFXResources.Resources))]
        [Required]
        public string SelectedAccount { get; set; }
        [Required]
        public Consts.eBuySell BuySell { get; set; }
        [Required]
        [Display(Name = "Amount CCY1")]
        [Range(1000, 1000000, ErrorMessage = "Please enter number between 1,000-1,000,000")]
        [DisplayFormat(DataFormatString = "{0:0,0}")]
        public double AmountCCY1 { get; set; }
        [Display(Name = "Minimal Exchange Amount (CCY1)")]
        [Range(0, 100000000, ErrorMessage = "Please enter number between 0 - 100,000,000")]
        public double? MinimalPartnerExecutionAmountCCY1 { get; set; }
        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }
        [Required]
        public string Symbol { get; set; }
        [Display(Name = "Comment", ResourceType = typeof(FlatFXResources.Resources))]
        public string Comment { get; set; }
        public bool PvPEnabled { get; set; }
        public bool EnsureOnLinePrice { get; set; }
        public Order order { get; set; }
        public List<string> InvalidAccountReason { get; set; }

        public long MatchOrderId { get; set; }
        public double MatchMinAmount { get; set; }
        public double MatchMaxAmount { get; set; }
        public double MatchMidRate { get; set; }

        public bool IsEdit { get; set; }
        public Deal deal { get; set; }

        public OrderViewModel()
        {
            InvalidAccountReason = new List<string>();
            OrderKey = Guid.NewGuid().ToString();
            IsEdit = false;
        }

        public Order OrderInSession
        {
            get
            {
                if (ApplicationInformation.Instance.Session[OrderKey] != null)
                    return ApplicationInformation.Instance.Session[OrderKey] as Order;
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