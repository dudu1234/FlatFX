using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFXCore.BussinessLayer
{
    public class Consts
    {
        public const string Role_CompanyUser = "CompanyUser";
        public const string Role_CompanyDemoUser = "CompanyDemoUser";
        public const string Role_ProviderUser = "ProviderUser";
        public const string Role_Administrator = "Administrator";

        public const string GenericDictionaryCategory_AddCoWorkerToCompany = "GenericDictionaryCategory_AddCoWorkerToCompany";

        public enum eUserStatus
        {
            Active = 1,
            Blocked = 2
        }
        public enum eCompanyStatus
        {
            Active = 1,
            Blocked = 2
        }
        public enum eProviderStatus
        {
            Active = 1,
            Blocked = 2
        }
        public enum eCustomerType
        {
            Importer = 1,
            Exporter = 2,
            [Display(Name="Exporter & Importer")]
            Exporter_Importer = 3,
            Changer = 4,
            Broker = 5,
            Private = 6,
            Demo = 7,
            FlatFX = 8
        }
        public enum eProviderType
        {
            Bank = 1,
            Demo = 2,
            Broker = 3,
            FlatFX = 4
        }
        public enum eQuoteResponseSpreadMethod
        {
            [Display(Description="Set only the promil per provider+pair")]
            PromilPerProvider = 1,
            [Display(Description = "Set only the spread per provider+pair")]
            SpreadPerProvider = 2,
            [Display(Description = "Set the promil per provider+pair with an option to override it by spread per provider+pair")]
            PromilOverrideBySpreadPerProvider = 3,
            [Display(Description = "Mid price + commision")]
            Mid = 4,
            [Display(Description="Set the promil per provider+pair with an option to override it by promil per customer+pair")]
            PromilPerProviderAndCustomer = 5,
            [Display(Description = "Set only the spread per provider+pair with an option to override it by spread per customer+pair")]
            SpreadPerProviderAndCustomer = 6,
            [Display(Description = "Set the promil per provider+pair with an option to override it by spread per provider+pair with an option to override it by spread/promil per customer+pair")]
            PromilOverrideBySpreadPerProviderAndCustomer = 7
        }
        public enum eChatEntryType
        {
            Text = 1,
            System = 2,
            Trace = 3,
            Query = 4,
            Bank_Offer = 5,
            Bank_Approved = 6,
            Client_Approved = 7,
            Client_Improve_Price = 8,
            Client_Cancel = 9
        }
        public enum eNotificationType
        {
            OnNewOrder = 1
        }
        public enum eDealType
        {
            None = 1,
            Spot = 2,
            Forward = 3,
            Same_Day = 4
        }
        public enum eDealStatus
        {
            None = 1,
            CustomerTransfer = 2,
            FlatFXTransfer = 3,
            Closed = 4,
            Canceled = 5,
            Problem = 6,
            New = 7
        }
        public enum eMatchStatus
        {
            New = 1,
            Opened = 2,
            Closed = 3,
            Cancel = 4
        }
        public enum eMatchTriggerSource
        {
            Order1 = 1,
            Order2 = 2,
            Automatic = 3
        }
        public enum eClearingType
        {
            [Display(Name = "None")]
            None = -1,
            [Display(Name = "Same Day")]
            SameDay = 0,
            [Display(Name = "1 Business Day")]
            BusinessDay_1 = 1,
            [Display(Name = "2 Business Day")]
            BusinessDay_2 = 2,
            [Display(Name = "5 Business Day")]
            BusinessDay_5 = 5
        }
        public enum eOrderStatus
        {
            None = 1,
            Waiting = 2,
            Triggered = 3,
            Closed_Successfully = 4,
            Canceled = 5,
            Problem = 6,
            Expired = 7,
            Triggered_partially = 8
        }
        public enum eDealProductType
        {
            FxSimpleExchange = 1,
            FxRFQ = 2,
            FxStreaming = 3,
            FxMidRateOrder
        }
        public enum eBuySell
        {
            Buy = 1,
            Sell = 2,
            Both = 3
        }
        public enum eBidAsk
        {
            Bid = 1,
            Ask = 2
        }
        public enum eCallPut
        {
            Call = 1,
            Put = 2
        }
        public enum eProviderLastChatAction
        {
            None = 1,
            Sent_Price = 2,
            Confirm_Deal = 3
        }
        public enum eCustomerLastChatAction
        {
            Sent_Query = 1,
            Confirm_Deal = 2,
            Cancel_Deal = 3,
            Exit = 4,
            Ask_to_Improve = 5
        }
        public enum eConfigurationFieldType
        {
            Text_Box = 1,
            Text_Box_Integer = 2,
            Check_Box = 3,
            Date_Time_Picker_Date_Part = 4,
            Date_Time_Picker_Time_Part = 5,
            Date_Time_Picker_Both = 6,
            Directory_Path = 7,
            Combo_Box = 8
        }
        /// <summary>
        /// The Level of Logger messages
        /// </summary>
        public enum eLogLevel
        {
            /// <summary>
            /// In case of critical error (system failed to run)
            /// /// </summary>
            CRITICAL_ERROR = 0,
            /// <summary>
            /// In case of error but the system can still run
            /// </summary>
            ERROR = 1,
            /// <summary>
            /// In case of incorrect functionality
            /// </summary>
            WARNING = 2,
            /// <summary>
            /// In case of event the we want to trace
            /// </summary>
            TRACE = 3,
            /// <summary>
            /// In case of debugging
            /// </summary>
            DEBUG = 4,
            /// <summary>
            /// Developer testing
            /// </summary>
            TEST = 5,
            /// <summary>
            /// NOT_RELEVANT
            /// </summary>
            NOT_RELEVANT = 6
        }
        /// <summary>
        /// Describe the operation, the type of the log row operation 
        /// </summary>
        public enum eLogOperationLevel
        {
            /// <summary>
            /// Inner application log row
            /// </summary>
            Internal_Log = 1,
            /// <summary>
            /// An administration log row (e.g. create a new user)
            /// </summary>
            Administration = 2,
            /// <summary>
            /// A system Trace log row (e.g. user login succeeded)
            /// </summary>
            System_Trace = 3,
            /// <summary>
            /// System_Exception (e.g. blocking user after 45 days without using the system, user enter illegal parameter...)
            /// </summary>
            System_Exception = 4
        }
        /// <summary>
        /// eOperationStatus
        /// </summary>
        public enum eLogOperationStatus
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,
            /// <summary>
            /// Succeeded
            /// </summary>
            Succeeded = 1,
            /// <summary>
            /// Failed
            /// </summary>
            Failed = 2
        }
        /// <summary>
        /// eComputeHashMode
        /// </summary>
        public enum eComputeHashMode
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,
            /// <summary>
            /// SHA1
            /// </summary>
            SHA1 = 1,
            /// <summary>
            /// SHA2
            /// </summary>
            SHA2 = 2
        }
        public enum eUserActionPriority
        {
            High = 1,
            Medium = 2,
            Low = 3
        }
        /// <summary>
        /// Defines the user roles.
        /// </summary>
        public enum UserRoles : short
        {
            Administrator = 1,
            ProviderUser = 2,
            ProviderDemoUser = 3,
            CompanyUser = 4,
            CompanyDemoUser = 5,
            Unknown = 6
        }
        /// <summary>
        /// eLanguage
        /// </summary>
        public enum eLanguage
        {
            English = 1,
            Hebrew = 2
        }
        /// <summary>
        /// eInvoiceCurrency
        /// </summary>
        public enum eInvoiceCurrency
        {
            USD = 1,
            ILS = 2,
            EUR = 3
        }
        public enum eCompanyVolume
        {
            [Display(Name="No FX")]
            NoFX = 1,
            [Display(Name = "< 100,000 $")]
            Under100K = 2,
            [Display(Name = "100,000-500,000 $")]
            Under500K = 3,
            [Display(Name = "500,000-1,000,000 $")]
            Under1M = 4,
            [Display(Name = "1,000,000-5,000,000 $")]
            Under5M = 5,
            [Display(Name = "5,000,000-12,000,000 $")]
            Under12M = 6,
            [Display(Name = "> 12,000,000 $")]
            MoreThan12M = 7
        }
        public enum eCountries
        {
            Israel = 1
        }
        public enum eSimpleCurrencyExchangeType
        {
            None = 0,
            [Display(Name = "מטח לשקלים")]
            FXtoILS = 1,
            [Display(Name = "שקלים למטח")]
            ILStoFX = 2,
            [Display(Name = "מטח למטח")]
            FXtoFX = 3
        }
        public enum eEmailStatus
        {
            None = 1,
            Sent = 2,
            SentConfirmed = 3
        }
    }
}
