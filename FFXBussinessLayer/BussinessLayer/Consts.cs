﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlatFX.BussinessLayer
{
    public class Consts
    {
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
            Changer = 3,
            Broker = 4,
            Private = 5,
            Demo = 6,
            FlatFX = 7
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
            PromilPlusConstant = 1,
            Constant = 2,
            PromilPlusConstantOverideByConstant = 3
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
        public enum eDealType
        {
            None = 1,
            Spot = 2,
            Forward = 3,
            Same_Day = 4
        }
        public enum eBuySell
        {
            Buy = 1,
            Sell = 2,
            Both = 3
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
    }
}