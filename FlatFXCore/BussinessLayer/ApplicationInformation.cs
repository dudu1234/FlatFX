using System;
using System.Collections;
using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Data;
//using System.Globalization;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Security.Cryptography;
//using Microsoft.Win32;
//using System.IO;
//using System.Text.RegularExpressions;
//using System.Web.Security;

using Microsoft.AspNet.Identity;
using System.Web.SessionState;
using System.Text;
using System.Security.Cryptography;
using System.IO;
//using FlatFXCore.Model.Data;

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
        public void Start() 
        {
        }
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
        private static ApplicationInformation m_ApplicationInformationInstance = null;
        #endregion

        #region Public Functions
        public bool IsDevelopmetMachine
        {
            get
            {
                if (Environment.MachineName == "DUDU-HP")
                    return true;
                else
                    return false;
            }
        }
        public string UserID
        {
            get
            {
                System.Web.HttpContext context = System.Web.HttpContext.Current;
                if (context == null || context.User == null || context.User.Identity.Name == "")
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
        public bool IsAdministrator
        {
            get
            {
                return IsUserInRole(Consts.Role_Administrator);
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
        /// IsToday
        /// </summary>
        /// <param name="Date"></param>
        /// <returns></returns>
        public bool IsToday(DateTime Date)
        {
            DateTime today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            if (Date >= today && Date < today.AddDays(1))
                return true;
            else
                return false;
        }
        public string NextBussinessDay()
        {
            DateTime nextDay = DateTime.Now;
            // To do Support Israel only, Does not support hollydays
            if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
                nextDay = DateTime.Now.AddDays(3);
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                nextDay = DateTime.Now.AddDays(2);
            else
                nextDay = DateTime.Now.AddDays(1);

            return nextDay.ToLongDateString();
        }
        public DateTime NextBussinessDay(int hour, int minute)
        {
            DateTime nextDay = DateTime.Now;
            // To do Support Israel only, Does not support hollydays
            if (DateTime.Now.DayOfWeek == DayOfWeek.Thursday)
                nextDay = DateTime.Now.AddDays(3);
            else if (DateTime.Now.DayOfWeek == DayOfWeek.Friday)
                nextDay = DateTime.Now.AddDays(2);
            else
                nextDay = DateTime.Now.AddDays(1);

            return new DateTime(nextDay.Year, nextDay.Month, nextDay.Day, hour, minute, 0);
        }
        #endregion
    }
    public class Crypto
    {
        private static string stam = "e45tybWE231BnjkQRM09Plkd578CV65";
        private static byte[] _salt = Encoding.ASCII.GetBytes("o6848342kvTTc5");

        public static string MixStam()
        {
            string newStam = "";
            for(int i=0;i<stam.Length;i++)
            {
                char addChar = (char)(stam[i] + i);
                newStam += addChar;
            }
            return newStam;
        }
        /// <summary>
        /// Encrypt the given string using AES.  The string can be decrypted using 
        /// DecryptStringAES().  
        /// </summary>
        /// <param name="plainText">The text to encrypt.</param>
        public static string EncryptStringAES(string plainText)
        {

            if (string.IsNullOrEmpty(plainText))
                throw new ArgumentNullException("plainText");
            
            string outStr = null;                       // Encrypted string to return
            RijndaelManaged aesAlg = null;              // RijndaelManaged object used to encrypt the data.

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(MixStam(), _salt);

                // Create a RijndaelManaged object
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);

                // Create a decryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // prepend the IV
                    msEncrypt.Write(BitConverter.GetBytes(aesAlg.IV.Length), 0, sizeof(int));
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                    }
                    outStr = Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            // Return the encrypted bytes from the memory stream.
            return outStr;
        }

        /// <summary>
        /// Decrypt the given string.  Assumes the string was encrypted using 
        /// EncryptStringAES(), 
        /// </summary>
        /// <param name="cipherText">The text to decrypt.</param>
        public static string DecryptStringAES(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                throw new ArgumentNullException("cipherText");
            
            // Declare the RijndaelManaged object
            // used to decrypt the data.
            RijndaelManaged aesAlg = null;

            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            try
            {
                // generate the key from the shared secret and the salt
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(MixStam(), _salt);

                // Create the streams used for decryption.                
                byte[] bytes = Convert.FromBase64String(cipherText);
                using (MemoryStream msDecrypt = new MemoryStream(bytes))
                {
                    // Create a RijndaelManaged object
                    // with the specified key and IV.
                    aesAlg = new RijndaelManaged();
                    aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                    // Get the initialization vector from the encrypted stream
                    aesAlg.IV = ReadByteArray(msDecrypt);
                    // Create a decrytor to perform the stream transform.
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))

                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
            finally
            {
                // Clear the RijndaelManaged object.
                if (aesAlg != null)
                    aesAlg.Clear();
            }

            return plaintext;
        }

        private static byte[] ReadByteArray(Stream s)
        {
            byte[] rawLength = new byte[sizeof(int)];
            if (s.Read(rawLength, 0, rawLength.Length) != rawLength.Length)
            {
                throw new SystemException("Stream did not contain properly formatted byte array");
            }

            byte[] buffer = new byte[BitConverter.ToInt32(rawLength, 0)];
            if (s.Read(buffer, 0, buffer.Length) != buffer.Length)
            {
                throw new SystemException("Did not read byte array properly");
            }

            return buffer;
        }
    }
}
