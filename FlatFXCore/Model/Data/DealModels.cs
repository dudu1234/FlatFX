using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Security.Principal;
using FlatFXCore.BussinessLayer;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.User;

namespace FlatFXCore.Model.Data
{
    [Table("Deals")]
    public class Deal
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int64 DealId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser user { get; set; }

        public string CompanyAccountId { get; set; }
        [ForeignKey("CompanyAccountId")]
        public virtual CompanyAccount CompanyAccount { get; set; }

        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual Provider Provider { get; set; }

        [Display(Name="Account")]
        public virtual ProviderAccount CreditedProviderAccount { get; set; }
        [Display(Name = "Account")]
        public virtual ProviderAccount ChargedProviderAccount { get; set; }

        public string ProviderUserId { get; set; }
        [ForeignKey("ProviderUserId")]
        public virtual ApplicationUser ProviderUser { get; set; }

        [MaxLength(100)]
        public string ProviderDealRef { get; set; }

        public Int64 QueryId { get; set; }
        [ForeignKey("QueryId")]
        public virtual QueryData Query { get; set; }

        public Consts.eDealType DealType { get; set; }
        public Consts.eDealProductType DealProductType { get; set; }
        public Consts.eBuySell BuySell { get; set; }

        [Required, Column(TypeName = "VARCHAR"), MaxLength(10)]
        public string Symbol { get; set; }
        public string CreditedCurrency { get; set; }
        public double AmountToExchangeCreditedCurrency { get; set; }
        public string ChargedCurrency { get; set; }
        public double AmountToExchangeChargedCurrency { get; set; }
        public double AmountUSD { get; set; }
        
        [Required]
        public double CustomerRate { get; set; }
        public double? BankRate { get; set; }
        public double? MidRate { get; set; }
        public DateTime OfferingDate { get; set; }
        [Required]
        public DateTime ContractDate { get; set; }
        public DateTime? MaturityDate { get; set; }
        public Consts.eCallPut CallPut { get; set; }
        public double? Commission { get; set; }
        [MaxLength(500)]
        public string Comment { get; set; }
        public bool IsDelivery { get; set; }
        public bool IsExotic { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsOffer { get; set; }
        public bool IsDemo { get; set; }

	    public double? CustomerTotalProfitUSD { get; set; }
        public double? FlatFXTotalProfitUSD { get; set; }
        public double? BankTotalProfitUSD { get; set; }

        public Deal() 
        {
            //Set the default values
            IsCanceled = false;
            IsExotic = false;
            IsDelivery = false;
            IsOffer = false;
            IsDemo = false;
        }
    }
    [Table("Queries")]
    public class QueryData
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int64 QueryId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public string CompanyAccountId { get; set; }
        [ForeignKey("CompanyAccountId")]
        public virtual CompanyAccount CompanyAccount { get; set; }

        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual Provider Provider { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }        
        [Required, Column(TypeName = "VARCHAR"), MaxLength(10)]
        public string Symbol { get; set; }
        public Consts.eDealType DealType { get; set; }
        public Consts.eBuySell BuySell { get; set; }
        [Required]
        public double AmountCCY1 { get; set; }
        public double? AmountUSD { get; set; }

        public QueryData() { }
    }
    [Table("QueriesPerProvider")]
    public class QueryPerProvider
    {
        [Key, Column(Order = 1)]
        public Int64 QueryId { get; set; }
        [ForeignKey("QueryId")]
        public virtual QueryData Query { get; set; }

        [Key, Column(Order = 2)]
        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual Provider Provider { get; set; }

        public double? ProviderApprovedBuyRate { get; set; }
        public double? ProviderApprovedSellRate { get; set; }
        public Consts.eProviderLastChatAction? ProviderLastAction { get; set; }
        public Consts.eCustomerLastChatAction? CustomerLastAction { get; set; }
        public DateTime? BankResponseTime { get; set; }
        public DateTime? UserResponseTime { get; set; }

        public QueryPerProvider() { }
    }
}
