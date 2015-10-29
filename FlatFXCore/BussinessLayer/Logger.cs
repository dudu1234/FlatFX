using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;
using System.Collections;
using System.Threading;
using System.Timers;
using System.Runtime.InteropServices;
using System.Linq;
using System.Threading.Tasks;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;

namespace FlatFXCore.BussinessLayer
{
    /// <summary>
    ///     Responsible to write to log. There are several log States.
    /// </summary>
    /// <example>
    ///     Logger.Instance.WriteToLog(message, Consts.eLevel.ERROR, "");
    /// </example>
    public class Logger
    {
        #region Members
        private Consts.eLogLevel m_LogWriteLevel = Consts.eLogLevel.ERROR;
        private static Logger m_LoggerInstance = null;
        #endregion

        #region Properties
        /// <summary>
        /// LogWriteLevel
        /// </summary>
        public Consts.eLogLevel LogWriteLevel
        {
            get { return m_LogWriteLevel; }
        }
        #endregion

        #region Ctor + Dtor
        /// <summary>
        ///     The Singelton ctor.
        /// </summary>
        internal Logger()
        {
            try
            {
                m_LogWriteLevel = Consts.eLogLevel.ERROR;
            }
            catch (Exception ex)
            {
                //Throw regular exception. 
                //This will prevent a loop between the Error class and the Config class.
                throw ex;
            }
        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
        /// <summary>
        /// Logger
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (m_LoggerInstance == null)
                    m_LoggerInstance = new Logger();

                return m_LoggerInstance;
            }
        }
        #endregion

        #region WriteToLog
        /// <summary>
        /// Message
        /// </summary>
        /// <param name="sMessage"></param>
        public void WriteToLog(string sMessage)
        {
            WriteToLog(sMessage, null, Consts.eLogLevel.DEBUG, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, null);
        }
        /// <summary>
        /// Message
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="ExtendedMessage"></param>
        public void WriteToLog(string Message, string ExtendedMessage)
        {
            WriteToLog(Message, ExtendedMessage, Consts.eLogLevel.DEBUG, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, null);
        }
        /// <summary>
        ///     function that write the log row
        /// </summary>
        /// <param name="sMessage">Message</param>
        /// <param name="iLevel">Log Level</param>
        public void WriteToLog(string sMessage, Consts.eLogLevel iLevel)
        {
            WriteToLog(sMessage, null, iLevel, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, null);
        }
        /// <summary>
        /// function that write the log row
        /// </summary>
        /// <param name="sMessage"></param>
        /// <param name="ex"></param>
        /// <param name="iLevel"></param>
        public void WriteToLog(string sMessage, Exception ex, Consts.eLogLevel iLevel)
        {
            if (ex == null)
                WriteToLog(sMessage, null, iLevel, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, null);
            else
                WriteToLog((sMessage == null || sMessage == String.Empty) ? ex.Message : sMessage, ex.ToString(), iLevel, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, null);
        }
        /// <summary>
        ///     function that write the log row
        /// </summary>
        /// <param name="sMessage">Message</param>
        /// <param name="iLevel">Log Level</param>
        /// <param name="RelatedClass">The Class that sends the message</param>
        public void WriteToLog(string sMessage, Consts.eLogLevel iLevel, string RelatedClass)
        {
            WriteToLog(sMessage, null, iLevel, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, RelatedClass);
        }
        /// <summary>
        ///     function that write the log row
        /// </summary>
        /// <param name="sMessage">Message</param>
        /// <param name="ex">exception</param>
        /// <param name="iLevel">Log Level</param>
        /// <param name="RelatedClass">The Class that sends the message</param>
        public void WriteToLog(string sMessage, Exception ex, Consts.eLogLevel iLevel, string RelatedClass)
        {
            if (ex == null)
                WriteToLog(sMessage, null, iLevel, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, RelatedClass);
            else
                WriteToLog((sMessage == null || sMessage == String.Empty) ? ex.Message : sMessage, ex.ToString(), iLevel, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, RelatedClass);
        }
        /// <summary>
        /// function that write the log row
        /// </summary>
        /// <param name="Description"></param>
        /// <param name="ExtendedDescription"></param>
        /// <param name="LogLevel"></param>
        /// <param name="OperationLevel"></param>
        /// <param name="OperationName"></param>
        /// <param name="OperationStatus"></param>
        /// <param name="RelatedObject"></param>
        public bool WriteToLog(string _Description, string _ExtendedDescription, Consts.eLogLevel _LogLevel,
            Consts.eLogOperationLevel _OperationLevel, string _OperationName, Consts.eLogOperationStatus _OperationStatus, string _RelatedObject)
        {
            try
            {
                if (!checkIfWriteLevel(_LogLevel, _OperationLevel))
                    return false;

                string _UserId = ApplicationInformation.Instance.GetUserID();
                if (_UserId == "")
                    _UserId = null;
                string _SessionId = ApplicationInformation.Instance.GetSessionID();
                string _UserIP = ApplicationInformation.Instance.GetUserIP();

                using (var context = new ApplicationDBContext())
                {
                    LogInfo row = new LogInfo
                    {
                        ApplicationName = "FlatFXClient",
                        Date = DateTime.Now,
                        Description = _Description,
                        ExtendedDescription = _ExtendedDescription,
                        LogLevel = _LogLevel,
                        OperationLevel = _OperationLevel,
                        OperationName = _OperationName,
                        OperationStatus = _OperationStatus,
                        SessionId = _SessionId,
                        UserId = _UserId,
                        UserIP = _UserIP,
                    };

                    context.LogInfo.Add(row);
                    context.SaveChanges();
                }

                return true;
            }
            catch
            {
                //Failed to write to log
                return false;
            }
        }
        #endregion

        #region WriteAdministrationAction
        /// <summary>
        /// Write Administration Event to the log table
        /// </summary>
        /// <param name="OperationName"></param>
        /// <param name="OperationStatus"></param>
        /// <param name="Message"></param>
        public void WriteAdministrationAction(string OperationName, Consts.eLogOperationStatus OperationStatus, string Message)
        {
            WriteToLog(Message, null, Consts.eLogLevel.NOT_RELEVANT, Consts.eLogOperationLevel.Administration, OperationName, OperationStatus, null);
        }
        /// <summary>
        /// Write Administration Event to the log table
        /// </summary>
        /// <param name="OperationName"></param>
        /// <param name="OperationStatus"></param>
        /// <param name="RelatedObject"></param>
        /// <param name="Message"></param>
        public void WriteAdministrationAction(string OperationName, Consts.eLogOperationStatus OperationStatus, string RelatedObject, string Message)
        {
            WriteToLog(Message, null, Consts.eLogLevel.NOT_RELEVANT, Consts.eLogOperationLevel.Administration, OperationName, OperationStatus, RelatedObject);
        }
        #endregion

        #region WriteSystemTrace
        /// <summary>
        /// Write System Trace Event to the log table
        /// </summary>
        /// <param name="OperationName"></param>
        /// <param name="OperationStatus"></param>
        /// <param name="Message"></param>
        public void WriteSystemTrace(string OperationName, Consts.eLogOperationStatus OperationStatus, string Message)
        {
            WriteToLog(Message, null, Consts.eLogLevel.TRACE, Consts.eLogOperationLevel.System_Trace, OperationName, OperationStatus, null);
        }
        /// <summary>
        /// Write System Trace Event to the log table
        /// </summary>
        /// <param name="OperationName"></param>
        /// <param name="OperationStatus"></param>
        /// <param name="RelatedObject"></param>
        /// <param name="Message"></param>
        public void WriteSystemTrace(string OperationName, Consts.eLogOperationStatus OperationStatus, string RelatedObject, string Message)
        {
            WriteToLog(Message, null, Consts.eLogLevel.TRACE, Consts.eLogOperationLevel.System_Trace, OperationName, OperationStatus, RelatedObject);
        }
        /// <summary>
        /// Write System Trace Event to the log table
        /// </summary>
        /// <param name="OperationName"></param>
        /// <param name="OperationStatus"></param>
        /// <param name="RelatedObject"></param>
        /// <param name="Message"></param>
        /// <param name="ExtendedMessage"></param>
        public void WriteSystemTrace(string OperationName, Consts.eLogOperationStatus OperationStatus, string RelatedObject, string Message, string ExtendedMessage)
        {
            WriteToLog(Message, ExtendedMessage, Consts.eLogLevel.TRACE, Consts.eLogOperationLevel.System_Trace, OperationName, OperationStatus, RelatedObject);
        }
        #endregion

        #region WriteSystemException
        /// <summary>
        /// Write System Exception Event to the log table
        /// </summary>
        /// <param name="OperationName"></param>
        /// <param name="OperationStatus"></param>
        /// <param name="Message"></param>
        public void WriteSystemException(string OperationName, Consts.eLogOperationStatus OperationStatus, string Message)
        {
            WriteToLog(Message, null, Consts.eLogLevel.CRITICAL_ERROR, Consts.eLogOperationLevel.System_Exception, OperationName, OperationStatus, null);
        }
        /// <summary>
        /// Write System Exception Event to the log table
        /// </summary>
        /// <param name="OperationName"></param>
        /// <param name="OperationStatus"></param>
        /// <param name="RelatedObject"></param>
        /// <param name="Message"></param>
        public void WriteSystemException(string OperationName, Consts.eLogOperationStatus OperationStatus, string RelatedObject, string Message)
        {
            WriteToLog(Message, null, Consts.eLogLevel.CRITICAL_ERROR, Consts.eLogOperationLevel.System_Exception, OperationName, OperationStatus, RelatedObject);
        }
        /// <summary>
        /// Write System Exception Event to the log table
        /// </summary>
        /// <param name="OperationName"></param>
        /// <param name="OperationStatus"></param>
        /// <param name="RelatedObject"></param>
        /// <param name="Message"></param>
        /// <param name="ExMessage"></param>
        public void WriteSystemException(string OperationName, Consts.eLogOperationStatus OperationStatus, string RelatedObject, string Message, string ExMessage)
        {
            WriteToLog(Message, ExMessage, Consts.eLogLevel.CRITICAL_ERROR, Consts.eLogOperationLevel.System_Exception, OperationName, OperationStatus, RelatedObject);
        }
        #endregion

        #region WriteError
        /// <summary>
        /// function that write to the log file 
        /// </summary>
        /// <param name="sMessage"></param>
        public void WriteError(string sMessage)
        {
            WriteToLog(sMessage, null, Consts.eLogLevel.ERROR, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, null);
        }
        /// <summary>
        /// function that write the log row
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="RelatedObject"></param>
        public void WriteError(Exception ex, string RelatedObject)
        {
            if (ex == null)
                WriteToLog("Failed in " + RelatedObject, null, Consts.eLogLevel.ERROR, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, RelatedObject);
            else
                WriteToLog("Failed in " + RelatedObject + ": " + ex.Message, ex.ToString(), Consts.eLogLevel.ERROR, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, RelatedObject);
        }
        /// <summary>
        /// function that write the log row
        /// </summary>
        /// <param name="sMessage"></param>
        /// <param name="ex"></param>
        public void WriteError(string sMessage, Exception ex)
        {
            if (ex == null)
                WriteToLog(sMessage, null, Consts.eLogLevel.ERROR, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, null);
            else
                WriteToLog((sMessage == null || sMessage == String.Empty) ? ex.Message : sMessage, ex.ToString(), Consts.eLogLevel.ERROR, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, null);
        }
        /// <summary>
        /// function that write the log row
        /// </summary>
        /// <param name="sMessage"></param>
        /// <param name="ex"></param>
        /// <param name="RelatedObject"></param>
        public void WriteError(string sMessage, Exception ex, string RelatedObject)
        {
            if (ex == null)
                WriteToLog(sMessage, null, Consts.eLogLevel.ERROR, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, RelatedObject);
            else
                WriteToLog((sMessage == null || sMessage == String.Empty) ? ex.Message : sMessage, ex.ToString(), Consts.eLogLevel.ERROR, Consts.eLogOperationLevel.Internal_Log, null, Consts.eLogOperationStatus.None, RelatedObject);
        }
        /// <summary>
        /// function that write the log row
        /// </summary>
        /// <param name="sMessage"></param>
        /// <param name="ExtendedMessage"></param>
        /// <param name="RelatedObject"></param>
        /// <param name="OperationLevel"></param>
        public void WriteError(string sMessage, string ExtendedMessage, string RelatedObject, Consts.eLogOperationLevel OperationLevel)
        {
            WriteToLog(sMessage, ExtendedMessage, Consts.eLogLevel.ERROR, OperationLevel, null, Consts.eLogOperationStatus.None, RelatedObject);
        }
        #endregion

        #region Private Functions
        private bool checkIfWriteLevel(Consts.eLogLevel iLevel, Consts.eLogOperationLevel OperationLevel)
        {
            if (iLevel <= m_LogWriteLevel || OperationLevel != Consts.eLogOperationLevel.Internal_Log)
                return true;
            else
                return false;
        }
        #endregion
    }
}
