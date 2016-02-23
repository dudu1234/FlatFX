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
using System.Linq.Expressions;

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

        public Int64? QueryId { get; set; }
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
        public DateTime? ContractDate { get; set; }
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
        public Consts.eDealStatus Status { get; set; }
        [MaxLength(500)]
        public string StatusDetails { get; set; }

        //Statistices
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

        public bool IsRealDeal
        {
            get
            {
                return (
                    IsCanceled == false &&
                    /* d.IsDemo == false &&*/ 
                    IsOffer == false &&
                    !(DealProductType == Consts.eDealProductType.FxMidRateOrder && !MaturityDate.HasValue)
                    );
            }
        }
    }
    [Table("Order")]
    public class Order
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int64 OrderId { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser user { get; set; }

        public string CompanyAccountId { get; set; }
        [ForeignKey("CompanyAccountId")]
        public virtual CompanyAccount CompanyAccount { get; set; }

        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual Provider Provider { get; set; }

        [Display(Name = "Account")]
        public virtual ProviderAccount CreditedProviderAccount { get; set; }
        [Display(Name = "Account")]
        public virtual ProviderAccount ChargedProviderAccount { get; set; }

        public Consts.eDealType DealType { get; set; }
        public Consts.eDealProductType DealProductType { get; set; }
        public Consts.eBuySell BuySell { get; set; }

        [Required, Column(TypeName = "VARCHAR"), MaxLength(10)]
        public string Symbol { get; set; }
        public double AmountCCY1 { get; set; }
        public double AmountCCY2_Estimation { get; set; }
        public double AmountUSD_Estimation { get; set; }

        public DateTime OrderDate { get; set; }
        [MaxLength(500)]
        public string Comment { get; set; }
        public bool IsCanceled { get; set; }
        public bool IsWaiting { get; set; }
        public bool IsClosed { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsDemo { get; set; }

        //Statistices
        public double? CustomerTotalProfitUSD_Estimation { get; set; }
        public double? FlatFXCommissionUSD_Estimation { get; set; }
        
        public double? MinimalPartnerExecutionAmountCCY1 { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public double? MinimalPartnerTotalVolumeUSD { get; set; }
        public int? PartnerMinScore { get; set; }

        public double? AmountCCY1_Executed { get; set; }
        public double? AmountCCY1_Remainder { get; set; }

        public Order()
        {
            //Set the default values
            IsCanceled = false;
            IsDemo = false;
            IsWaiting = false;
            IsClosed = false;
            IsConfirmed = false;
        }

        public string CCY1
        {
            get
            {
                return Symbol.Substring(0, 3);
            }
        }
        public string CCY2
        {
            get
            {
                return Symbol.Substring(3, 3);
            }
        }
        public double BuyAmount
        {
            get
            {
                if (BuySell == Consts.eBuySell.Buy)
                    return AmountCCY1;
                else
                    return AmountCCY2_Estimation;
            }
        }
        public double SellAmount
        {
            get
            {
                if (BuySell == Consts.eBuySell.Buy)
                    return AmountCCY2_Estimation;
                else
                    return AmountCCY1;
            }
        }
        public string BuyCurrency
        {
            get
            {
                if (BuySell == Consts.eBuySell.Buy)
                    return CCY1;
                else
                    return CCY2;
            }
        }
        public string SellCurrency
        {
            get
            {
                if (BuySell == Consts.eBuySell.Buy)
                    return CCY2;
                else
                    return CCY1;
            }
        }
    }
    [Table("PromilOrderMatch")]
    public class OrderMatch
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public Int64 MatchId { get; set; }

        public virtual Deal Deal1 { get; set; }
        public virtual Deal Deal2 { get; set; }

        public virtual Order Order1 { get; set; }
        public virtual Order Order2 { get; set; }
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
