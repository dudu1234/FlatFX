using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.Win32;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Security;
using FlatFXCore.Model.Data;
using System.Web.SessionState;

namespace FlatFXCore.BussinessLayer
{
    /// <summary>
    ///     Contains General Information regarding current application state.
    ///     This will be saved per Session.
    /// </summary>
    public class ApplicationInformation : IDisposable
    {
        #region Ctor
        /// <summary>
        ///     Ctor
        /// </summary>
        internal ApplicationInformation()
        {
        }
        public static ApplicationInformation Instance
        {
            get
            {
                if (m_ApplicationInformationInstance == null)
                    m_ApplicationInformationInstance = new ApplicationInformation();

                return m_ApplicationInformationInstance;
            }
        }
        public void Start() { }
        #endregion

        #region Dispose
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
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        /// <summary>
        /// Dtor
        /// </summary>
        ~ApplicationInformation()
        {
            Dispose(false);
        }
        #endregion

        #region Members
        private List<string> m_CriticalErrorReportEmailList = null;
        private Dictionary<string, string> m_EncryptedKeys = new Dictionary<string, string>();
        private Dictionary<string, string> m_DecryptedKeys = new Dictionary<string, string>();
        private bool? m_IsDesignMode = null;
        private SHA1 m_SHA1 = null;
        private SHA256 m_SHA256 = null;
        private object m_SHALockObject = new object();
        /// <summary>
        /// Indicates whether the SQL queries between Client=>Server should contains a hash check.
        /// In case the client can connect to the SQL Server directlly, this feature is not enabled.
        /// 
        /// The Client's applications take this field from config file: ComputeHashMode
        /// The Server application take this field from the configuration database table.
        /// </summary>
        public Consts.eComputeHashMode ComputeHashMode_ClientSQLQueries = Consts.eComputeHashMode.None;
        private static ApplicationInformation m_ApplicationInformationInstance = null;
        #endregion

        #region Properties
        /// <summary>
        /// CriticalErrorReportEmailList
        /// </summary>
        public List<string> CriticalErrorReportEmailList
        {
            get
            {
                if (m_CriticalErrorReportEmailList == null)
                {
                    m_CriticalErrorReportEmailList = new List<string>();
                    string ToStr = Config.Instance["CriticalErrorReportEmailList"];
                    string[] ToArr = ToStr.Split(';');
                    foreach (string to in ToArr)
                    {
                        if (to != null && to.Length > 3)
                            m_CriticalErrorReportEmailList.Add(to);
                    }
                }
                return m_CriticalErrorReportEmailList;
            }
        }
        #endregion

        #region Public Functions
        public string UserID
        {
            get
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                if (context == null || context.User.Identity.Name == "")
                    return "";
                else
                    return context.User.Identity.GetUserId();
            }
        }
        public bool IsUserInRole(string role)
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            if (context == null || context.User.Identity.Name == "")
                return false;
            else
                return context.User.IsInRole(role);
        }
        public bool IsDemoUser
        {
            get
            {
                return IsUserInRole(Consts.Role_CompanyDemoUser);
            }
        }
        public string SessionID
        {
            get
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;

                if (context == null || context.Session == null)
                    return null;
                else
                    return context.Session.SessionID;
            }
        }
        public HttpSessionState Session
        {
            get
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;

                if (context == null || context.Session == null)
                    return null;
                else
                    return context.Session;
            }
        }
        public string UserIP
        {
            get
            {
                try
                {
                    System.Web.HttpContext context = System.Web.HttpContext.Current;
                    if (context == null || context.Request == null || context.Request.ServerVariables == null || context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] == null)
                        return null;

                    string ipAddress = context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (!string.IsNullOrEmpty(ipAddress))
                    {
                        string[] addresses = ipAddress.Split(',');
                        if (addresses.Length != 0)
                        {
                            return addresses[0];
                        }
                    }

                    if (context.Request.ServerVariables["REMOTE_ADDR"] == null)
                        return null;
                    else
                        return context.Request.ServerVariables["REMOTE_ADDR"];
                }
                catch
                {
                    return null;
                }
            }
        }
        /// <summary>
        ///     Return whether the running process is the real time application or the design mode of the .Net Visual Studio
        /// </summary>
        /// <returns></returns>
        public bool IsDesignMode
        {
            get
            {
                if (!m_IsDesignMode.HasValue)
                {
                    if (System.Diagnostics.Process.GetCurrentProcess().ProcessName.IndexOf("devenv") != -1) // Design Mode
                        m_IsDesignMode = true;
                    else //Real Time Mode
                        m_IsDesignMode = false;
                }

                return m_IsDesignMode.Value;
            }
        }
        /// <summary>
        /// GetStartOfDay
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public DateTime GetStartOfDay(DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, Date.Day, 0, 0, 0);
        }
        /// <summary>
        /// GetEndOfDay
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public DateTime GetEndOfDay(DateTime Date)
        {
            return new DateTime(Date.Year, Date.Month, Date.Day, 23, 59, 59);
        }
        /// <summary>
        /// MixString
        /// </summary>
        /// <param name="SourceString"></param>
        /// <returns></returns>
        public string MixString(string SourceString)
        {
            if (SourceString == null)
                return null;

            string MixedString = "";
            int index = 0;
            foreach (Char c in SourceString)
            {
                if ((index % 2) == 0)
                {
                    MixedString += (char)((int)c + (index % 5));
                }
                else if ((index % 3) == 0)
                {
                    MixedString += (char)((int)c + (index % 4));
                }
                else if ((index % 7) == 0)
                {
                    MixedString += (char)((int)c + (index % 3));
                }
                else
                {
                    MixedString += c;
                }

                index++;
            }
            return MixedString;
        }
        /// <summary>
        /// UnmixString
        /// </summary>
        /// <param name="MixedString"></param>
        /// <returns></returns>
        public string UnmixString(string MixedString)
        {
            string SourceString = "";
            int index = 0;
            foreach (Char c in MixedString)
            {
                if ((index % 2) == 0)
                {
                    SourceString += (char)((int)c - (index % 5));
                }
                else if ((index % 3) == 0)
                {
                    SourceString += (char)((int)c - (index % 4));
                }
                else if ((index % 7) == 0)
                {
                    SourceString += (char)((int)c - (index % 3));
                }
                else
                {
                    SourceString += c;
                }

                index++;
            }
            return SourceString;
        }
        #endregion

        #region Private Functions

        #endregion

        #region Encryption
        private string EncPass
        {
            get
            {
                if (m_EncPass == null)
                {
                    try
                    {
                        string subKeyPath = "FTL";
                        RegistryKey rk = Registry.LocalMachine;
                        rk = rk.OpenSubKey("SOFTWARE\\" + subKeyPath);
                        string val = rk.GetValue("name67").ToString();
                        if (val.StartsWith("fe38ILdw12I98"))
                            m_EncPass = val;
                        else
                            m_EncPass = "fe38ILdw12I98" + val;
                    }
                    catch
                    {
                        throw new Exception("Encryption process failed. Contact your system administrator in order to fix this error.");
                    }
                }
                return m_EncPass;
            }
        }
        /// <summary>
        /// Indicate whether to use Logger in the encryption functions
        /// </summary>
        public bool UseLoggerInEncryption = true;
        private enum AlgType
        {
            Rijndael = 0,
            TripleDES = 1
        };
        private AlgType m_AlgType = AlgType.Rijndael;
        private string m_EncPass = null;
        private byte[] m_rgbSalt = new byte[] {0x19, 0x40, 0x25, 0x1d, 0x4c, 0x55, 
            0x81, 0x22, 0x90, 0x12, 0x7e, 0x48, 0x91};
        /// <summary>
        ///     Encrypt a byte array into a byte array using a key and an IV  
        /// </summary>
        /// <param name="clearData"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        private byte[] Encrypt(byte[] clearData, byte[] Key, byte[] IV)
        {
            // Create a MemoryStream to accept the encrypted bytes 
            MemoryStream ms = new MemoryStream();

            TripleDES TripleDESAlg = null;
            Rijndael RijndaelAlg = null;

            // Create a symmetric algorithm. 
            // We are going to use Rijndael because it is strong and
            // available on all platforms. 
            if (m_AlgType == AlgType.TripleDES)
                TripleDESAlg = TripleDES.Create();
            else
                RijndaelAlg = Rijndael.Create();

            // Now set the key and the IV. 
            // We need the IV (Initialization Vector) because
            // the algorithm is operating in its default 
            // mode called CBC (Cipher Block Chaining).
            // The IV is XORed with the first block (8 byte) 
            // of the data before it is encrypted, and then each
            // encrypted block is XORed with the 
            // following block of plaintext.
            // This is done to make encryption more secure. 

            // There is also a mode called ECB which does not need an IV,
            // but it is much less secure. 
            if (m_AlgType == AlgType.TripleDES)
            {
                TripleDESAlg.Key = Key;
                TripleDESAlg.IV = IV;
            }
            else
            {
                RijndaelAlg.Key = Key;
                RijndaelAlg.IV = IV;
            }


            // Create a CryptoStream through which we are going to be
            // pumping our data. 
            // CryptoStreamMode.Write means that we are going to be
            // writing data to the stream and the output will be written
            // in the MemoryStream we have provided. 
            CryptoStream cs = new CryptoStream(
                ms,
                (m_AlgType == AlgType.TripleDES) ? TripleDESAlg.CreateEncryptor() : RijndaelAlg.CreateEncryptor(),
                CryptoStreamMode.Write);

            // Write the data and make it do the encryption 
            cs.Write(clearData, 0, clearData.Length);

            // Close the crypto stream (or do FlushFinalBlock). 
            // This will tell it that we have done our encryption and
            // there is no more data coming in, 
            // and it is now a good time to apply the padding and
            // finalize the encryption process. 
            cs.Close();

            // Now get the encrypted data from the MemoryStream.
            // Some people make a mistake of using GetBuffer() here,
            // which is not the right way. 
            byte[] encryptedData = ms.ToArray();

            return encryptedData;
        }
        /// <summary>
        ///     Encrypt a string into a string using a password  
        ///     Uses Encrypt(byte[], byte[], byte[]) 
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns>the encrypted data. throw Exception in case of error</returns>
        /// <remarks>
        ///     If in the future we will need the encryption to work faster, we should use the "TripleDES" algorithm.
        ///     Rijndael is supported in all platform. That is why we are using it. 
        /// </remarks>
        public string Encrypt(string clearText)
        {
            try
            {
                if (clearText == null)
                    return null;

                string encValue = "";
                if (m_EncryptedKeys.TryGetValue(clearText, out encValue))
                {
                    return encValue;
                }

                // First we need to turn the input string into a byte array. 
                byte[] clearBytes =
                  System.Text.Encoding.Unicode.GetBytes(clearText);

                // Then, we need to turn the password into Key and IV 
                // We are using salt to make it harder to guess our key
                // using a dictionary attack - 
                // trying to guess a password by enumerating all possible words. 
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(EncPass, m_rgbSalt);

                // Now get the key/IV and do the encryption using the
                // function that accepts byte arrays. 
                // Using PasswordDeriveBytes object we are first getting
                // 32 bytes for the Key 
                // (the default Rijndael key length is 256bit = 32bytes)
                // and then 16 bytes for the IV. 
                // IV should always be the block size, which is by default
                // 16 bytes (128 bit) for Rijndael. 
                // If you are using DES/TripleDES/RC2 the block size is
                // 8 bytes and so should be the IV size. 
                // You can also read KeySize/BlockSize properties off
                // the algorithm to find out the sizes. 
                byte[] encryptedData = null;
                if (m_AlgType == AlgType.TripleDES)
                {
                    encryptedData = Encrypt(clearBytes,
                         pdb.GetBytes(24), pdb.GetBytes(8));
                }
                else
                {
                    encryptedData = Encrypt(clearBytes,
                         pdb.GetBytes(32), pdb.GetBytes(16));
                }

                // Now we need to turn the resulting byte array into a string. 
                // A common mistake would be to use an Encoding class for that.
                // It does not work because not all byte values can be
                // represented by characters. 
                // We are going to be using Base64 encoding that is designed
                // exactly for what we are trying to  do. 
                string result = Convert.ToBase64String(encryptedData);

                try
                {
                    m_EncryptedKeys.Add(clearText, result);
                }
                catch { }

                return result;
            }
            catch (Exception ex)
            {
                if (UseLoggerInEncryption && ex.Message != "Failed to find password")
                    Logger.Instance.WriteError("Failed to Encrypt", ex);
                throw ex;
            }

        }
        /// <summary>
        ///     Decrypt a byte array into a byte array using a key and an IV  
        /// </summary>
        /// <param name="cipherData"></param>
        /// <param name="Key"></param>
        /// <param name="IV"></param>
        /// <returns></returns>
        private byte[] Decrypt(byte[] cipherData,
                                    byte[] Key, byte[] IV)
        {
            // Create a MemoryStream that is going to accept the
            // decrypted bytes 
            MemoryStream ms = new MemoryStream();

            TripleDES TripleDESAlg = null;
            Rijndael RijndaelAlg = null;

            // Create a symmetric algorithm. 
            // We are going to use Rijndael because it is strong and
            // available on all platforms. 
            if (m_AlgType == AlgType.TripleDES)
                TripleDESAlg = TripleDES.Create();
            else
                RijndaelAlg = Rijndael.Create();

            // Now set the key and the IV. 
            // We need the IV (Initialization Vector) because the algorithm
            // is operating in its default 
            // mode called CBC (Cipher Block Chaining). The IV is XORed with
            // the first block (8 byte) 
            // of the data after it is decrypted, and then each decrypted
            // block is XORed with the previous 
            // cipher block. This is done to make encryption more secure. 
            // There is also a mode called ECB which does not need an IV,
            // but it is much less secure. 
            if (m_AlgType == AlgType.TripleDES)
            {
                TripleDESAlg.Key = Key;
                TripleDESAlg.IV = IV;
            }
            else
            {
                RijndaelAlg.Key = Key;
                RijndaelAlg.IV = IV;
            }

            // Create a CryptoStream through which we are going to be
            // pumping our data. 
            // CryptoStreamMode.Write means that we are going to be
            // writing data to the stream 
            // and the output will be written in the MemoryStream
            // we have provided. 
            CryptoStream cs = new CryptoStream(
                ms,
                (m_AlgType == AlgType.TripleDES) ? TripleDESAlg.CreateDecryptor() : RijndaelAlg.CreateDecryptor(),
                CryptoStreamMode.Write);

            // Write the data and make it do the decryption 
            cs.Write(cipherData, 0, cipherData.Length);

            // Close the crypto stream (or do FlushFinalBlock). 
            // This will tell it that we have done our decryption
            // and there is no more data coming in, 
            // and it is now a good time to remove the padding
            // and finalize the decryption process. 
            cs.Close();

            // Now get the decrypted data from the MemoryStream. 
            // Some people make a mistake of using GetBuffer() here,
            // which is not the right way. 
            byte[] decryptedData = ms.ToArray();

            return decryptedData;
        }
        /// <summary>
        ///     Decrypt a string into a string using a password 
        ///     Uses Decrypt(byte[], byte[], byte[]) 
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public string Decrypt(string cipherText)
        {
            try
            {
                if (cipherText == null)
                    return null;

                string decryptedValue = "";
                if (m_DecryptedKeys.TryGetValue(cipherText, out decryptedValue))
                {
                    return decryptedValue;
                }

                // First we need to turn the input string into a byte array. 
                // We presume that Base64 encoding was used 
                byte[] cipherBytes = Convert.FromBase64String(cipherText);

                // Then, we need to turn the password into Key and IV 
                // We are using salt to make it harder to guess our key
                // using a dictionary attack - 
                // trying to guess a password by enumerating all possible words. 
                PasswordDeriveBytes pdb = new PasswordDeriveBytes(EncPass, m_rgbSalt);

                // Now get the key/IV and do the decryption using
                // the function that accepts byte arrays. 
                // Using PasswordDeriveBytes object we are first
                // getting 32 bytes for the Key 
                // (the default Rijndael key length is 256bit = 32bytes)
                // and then 16 bytes for the IV. 
                // IV should always be the block size, which is by
                // default 16 bytes (128 bit) for Rijndael. 
                // If you are using DES/TripleDES/RC2 the block size is
                // 8 bytes and so should be the IV size. 
                // You can also read KeySize/BlockSize properties off
                // the algorithm to find out the sizes. 
                byte[] decryptedData = null;
                if (m_AlgType == AlgType.TripleDES)
                {
                    decryptedData = Decrypt(cipherBytes,
                         pdb.GetBytes(24), pdb.GetBytes(8));
                }
                else
                {
                    decryptedData = Decrypt(cipherBytes,
                         pdb.GetBytes(32), pdb.GetBytes(16));
                }
                // Now we need to turn the resulting byte array into a string. 
                // A common mistake would be to use an Encoding class for that.
                // It does not work 
                // because not all byte values can be represented by characters. 
                // We are going to be using Base64 encoding that is 
                // designed exactly for what we are trying to  do. 
                string result = System.Text.Encoding.Unicode.GetString(decryptedData);

                try
                {
                    m_DecryptedKeys.Add(cipherText, result);
                }
                catch { }

                return result;
            }
            catch (Exception ex)
            {
                if (UseLoggerInEncryption && ex.Message != "Failed to find password")
                    Logger.Instance.WriteError("Failed to Decrypt: '" + cipherText + "'", ex);
                throw ex;
            }
        }
        /// <summary>
        ///     Encryption of one way (good for passwords).
        ///     There is no Password/Key involved.
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        public string OneWayEncryption(string clearText)
        {
            lock (m_SHALockObject)
            {
                if (clearText == null)
                    return null;

                try
                {
                    //Edit the clearText
                    clearText += EncPass;

                    // First we need to turn the input string into a byte array. 
                    byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);

                    byte[] resultBuffer = null;

                    if (m_SHA1 == null)
                        m_SHA1 = new SHA1CryptoServiceProvider();
                    if (m_SHA256 == null && ComputerHashMode_UserPassword == Consts.eComputeHashMode.SHA2)
                        m_SHA256 = new SHA256CryptoServiceProvider();

                    if (ComputerHashMode_UserPassword == Consts.eComputeHashMode.SHA1)
                        resultBuffer = m_SHA1.ComputeHash(clearBytes);
                    else // SHA2
                        resultBuffer = m_SHA256.ComputeHash(clearBytes);

                    return Convert.ToBase64String(resultBuffer);
                }
                catch (Exception ex)
                {
                    if (UseLoggerInEncryption && ex.Message != "Failed to find password")
                        Logger.Instance.WriteError("Failed in OneWayEncryption", ex);
                    throw ex;
                }
            }
        }
        /// <summary>
        ///     GetMixedEncPass
        /// </summary>
        /// <returns></returns>
        public string GetMixedEncPass()
        {
            return MixString(EncPass);
        }
        /// <summary>
        /// SetMixedEncPass
        /// </summary>
        /// <param name="mixedPass"></param>
        /// <param name="isFull"></param>
        public void SetMixedEncPass(string mixedPass, bool isFull)
        {
            if (m_EncPass == null)
            {
                string Unmixed = UnmixString(mixedPass);
                if (isFull)
                    m_EncPass = Unmixed;
                else
                    m_EncPass = "fe38ILdw12I98" + Unmixed;
            }
        }
        #endregion

        #region Hash Functions
        /// <summary>
        /// AppComputeHash
        /// Compute the Hash after changing the string in order to prevent hakers to change
        /// the string and change the hash too (on the connection wire).
        /// </summary>
        /// <param name="clearText"></param>
        /// <returns></returns>
        public string AppComputeHash(string clearText)
        {
            lock (m_SHALockObject)
            {
                if (clearText == null)
                    return null;

                if (ComputeHashMode_ClientSQLQueries == Consts.eComputeHashMode.None)
                    return "";

                try
                {
                    //Edit the clearText
                    clearText += EncPass;

                    // First we need to turn the input string into a byte array. 
                    byte[] clearBytes = System.Text.Encoding.Unicode.GetBytes(clearText);
                    byte[] resultBuffer = null;
                    if (m_SHA1 == null)
                        m_SHA1 = new SHA1CryptoServiceProvider();
                    if (m_SHA256 == null && ComputeHashMode_ClientSQLQueries == Consts.eComputeHashMode.SHA2)
                        m_SHA256 = new SHA256CryptoServiceProvider();

                    if (ComputeHashMode_ClientSQLQueries == Consts.eComputeHashMode.SHA1)
                        resultBuffer = m_SHA1.ComputeHash(clearBytes);
                    else if (ComputeHashMode_ClientSQLQueries == Consts.eComputeHashMode.SHA2)
                        resultBuffer = m_SHA256.ComputeHash(clearBytes);

                    return Convert.ToBase64String(resultBuffer);
                }
                catch (Exception ex)
                {
                    if (UseLoggerInEncryption && ex.Message != "Failed to find password")
                        Logger.Instance.WriteError("Failed in OneWayEncryption", ex);
                    throw ex;
                }
            }
        }
        #endregion

        #region Format
        
        /// <summary>
        /// The Application FormatProvider
        /// </summary>
        public IFormatProvider FormatProvider = CultureInfo.CurrentCulture;
        
        /// <summary>
        /// Date Format
        /// </summary>
        private string DATE_FORMAT = Config.Instance["DATE_FORMAT", "dd/MM/yyyy"];
        #endregion

        #region Security
        /// <summary>
        /// Validate password format
        /// </summary>
        /// <param name="password"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool IsValidInput(string password, out string errMsg)
        {
            errMsg = "";
            // Valid value are digits, characters and underscore
            Regex regStr1 = new Regex(@"^(([0-9a-zA-Z_])*)$");
            if (regStr1.IsMatch(password))
            {
                bool isCapitelLetterExists = false;
                bool isDigitExists = false;
                bool isCharExists = false;

                //Check for Capital Letter
                if (Config.Instance["FORCE_DIGITS_AND_LETTERS_IN_PASSWORD", true])
                {
                    for (int i = 0; i < password.Length; i++)
                    {
                        if (Char.IsUpper(password[i]))
                            isCapitelLetterExists = true;
                    }
                }
                else
                {
                    isCapitelLetterExists = true;
                }

                //Check for both digits and letters
                if (Config.Instance["FORCE_DIGITS_AND_LETTERS_IN_PASSWORD", true])
                {
                    for (int i = 0; i < password.Length; i++)
                    {
                        if (Char.IsDigit(password[i]))
                            isDigitExists = true;
                        if (Char.IsLetter(password[i]))
                            isCharExists = true;
                    }
                }
                else
                {
                    isDigitExists = true;
                    isCharExists = true;
                }

                if (isCapitelLetterExists && isDigitExists && isCharExists)
                    return true;
                else
                {
                    string msg = "Invalid Password Format. Password's characters can be digits, letters and underscore.\n";
                    if (Config.Instance["FORCE_DIGITS_AND_LETTERS_IN_PASSWORD", true])
                        msg += "Password must contains both letters and digits.\n";
                    if (Config.Instance["FORCE_CAPITAL_LETTER_IN_PASSWORD", true])
                        msg += "Password must contains capital letter.\n";

                    errMsg = IsValidInput_ErrorMessage;
                    return false;
                }
            }
            else
            {
                errMsg = IsValidInput_ErrorMessage;
                return false;
            }
        }
        private string IsValidInput_ErrorMessage
        {
            get
            {
                string msg = "Invalid Password Format. Password's characters can be digits, letters and underscore.\n";
                if (Config.Instance["FORCE_DIGITS_AND_LETTERS_IN_PASSWORD", true])
                    msg += "Password must contains both letters and digits.\n";
                if (Config.Instance["FORCE_CAPITAL_LETTER_IN_PASSWORD", true])
                    msg += "Password must contains capital letter.\n";

                return msg;
            }
        }
        private Consts.eComputeHashMode m_ComputerHashMode_UserPassword = Consts.eComputeHashMode.None;
        /// <summary>
        /// ComputerHashMode_UserPassword
        /// </summary>
        public Consts.eComputeHashMode ComputerHashMode_UserPassword
        {
            get
            {
                if (m_ComputerHashMode_UserPassword == Consts.eComputeHashMode.None)
                {
                    int iComputerHashMode_UserPassword = Config.Instance["ComputerHashMode_UserPassword", 1];
                    if (iComputerHashMode_UserPassword == 1)
                        m_ComputerHashMode_UserPassword = Consts.eComputeHashMode.SHA1;
                    else
                        m_ComputerHashMode_UserPassword = Consts.eComputeHashMode.SHA2;
                }

                return m_ComputerHashMode_UserPassword;
            }
        }
        #endregion

        #region Checksum
        /// <summary>
        /// Return the Checksum of the String
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public string GetChecksum(string s)
        {
            if (s == null)
                return "";

            char lastC;
            try
            {
                ulong checksum = 0;
                int count = 0;
                foreach (char c in s)
                {
                    count++;
                    lastC = c;
                    checksum += Convert.ToUInt32(c) + Convert.ToUInt32((count % 503));
                }
                return checksum.ToString("X4");
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in General::GetChecksum", ex);
                return s;
            }
        }
        #endregion
    }
}
