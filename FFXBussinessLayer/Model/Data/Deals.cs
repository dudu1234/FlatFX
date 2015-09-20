using FlatFX.BussinessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFX.Model.Data
{
    [Table("Deals")]
    public class DealData
    {
        [Key]
        public Int64 DealId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserData User { get; set; }

        public int CompanyAccountId { get; set; }
        [ForeignKey("CompanyAccountId")]
        public virtual CompanyAccountData CompanyAccount { get; set; }

        public int ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual ProviderData Provider { get; set; }

        public int ProviderUserId { get; set; }
        [ForeignKey("ProviderUserId")]
        public virtual UserData ProviderUser { get; set; }

        [MaxLength(100)]
        public string ProviderDealRef { get; set; }

        public Int64 QueryId { get; set; }
        [ForeignKey("QueryId")]
        public virtual QueryData Query { get; set; }

        public Consts.eDealType DealType { get; set; }
        public Consts.eBuySell BuySell { get; set; }

        [Required, Column(TypeName = "VARCHAR"), MaxLength(10)]
        public string Symbol { get; set; }

        public double? Amount1 { get; set; }
        public double? Amount2 { get; set; }
        [Required]
        public double AmountUSD { get; set; }
        [Required]
        public double Rate { get; set; }
        public double? SpotRate { get; set; }
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
	    public double? TotalProfitUSD { get; set; }

        public DealData() 
        {
            //Set the default values
            IsCanceled = false;
            IsExotic = false;
            IsDelivery = false;
        }
    }
    [Table("Queries")]
    public class QueryData
    {
        [Key]
        public Int64 QueryId { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual UserData User { get; set; }

        public int CompanyAccountId { get; set; }
        [ForeignKey("CompanyAccountId")]
        public virtual CompanyAccountData CompanyAccount { get; set; }

        public int ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual ProviderData Provider { get; set; }

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
    public class QueryPerProviderData
    {
        [Key, Column(Order = 1)]
        public Int64 QueryId { get; set; }
        [ForeignKey("QueryId")]
        public virtual QueryData Query { get; set; }

        [Key, Column(Order = 2)]
        public int ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual ProviderData Provider { get; set; }

        public double? ProviderApprovedBuyRate { get; set; }
        public double? ProviderApprovedSellRate { get; set; }
        public Consts.eProviderLastChatAction? ProviderLastAction { get; set; }
        public Consts.eCustomerLastChatAction? CustomerLastAction { get; set; }
        public DateTime? BankResponseTime { get; set; }
        public DateTime? UserResponseTime { get; set; }

        public QueryPerProviderData() { }
    }
}
