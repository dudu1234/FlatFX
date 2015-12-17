using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Xml;
using System.Linq;
using System.IO;
using System.Data;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Microsoft.AspNet.Identity;
using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;

namespace FlatFXCore.BussinessLayer
{
    /// <summary>
    /// Returns all app configuration.
    /// First load the data into hashtable object.
    /// </summary>
    /// <example>
    ///     Config.Instance["[Key Name]"]
    /// </example>
    public class Config
    {
        #region Members
        private static Config m_Instance = null;
        private static object sync = new object();
        private Dictionary<string, Dictionary<string, ConfigurationRow>> _dictionary = new Dictionary<string, Dictionary<string, ConfigurationRow>>();
        #endregion

        #region Ctor
        /// <summary>
        /// Get the Config Instance
        /// </summary>
        public static Config Instance
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (sync)
                    {
                        if (m_Instance == null)
                        {
                            m_Instance = new Config();
                        }
                    }
                }
                return m_Instance;
            }
        }
        private Config()
        {
            try
            {
                LoadData();
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
        public void Start() { }
        #endregion

        #region Public Functions
        /// <summary>
        /// getRealTimeValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userId"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string getRealTimeValue(string key, string userId, string defaultValue)
        {
            return Convert.ToString(getRealTimeConfigValue(key, userId, defaultValue));
        }
        /// <summary>
        /// getRealTimeValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userId"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public int getRealTimeValue(string key, string userId, int defaultValue)
        {
            return Convert.ToInt32(getRealTimeConfigValue(key, userId, defaultValue.ToString()));
        }
        /// <summary>
        /// getRealTimeValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userId"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public bool getRealTimeValue(string key, string userId, bool defaultValue)
        {
            object value = getRealTimeConfigValue(key, userId, defaultValue.ToString());
            if (value.ToString() == "1" || value.ToString().ToLower() == "true")
                return true;
            else
                return false;

        }
        /// <summary>
        /// getRealTimeConfigValue
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="userId"></param>
        /// <param name="DefaultValue"></param>
        /// <returns></returns>
        public string getRealTimeConfigValue(string Key, string userId, string DefaultValue)
        {
            using (var context = new ApplicationDBContext())
            {
                //Get user row
                ConfigurationRow data = context.Configurations.FirstOrDefault(row => row.Key == Key && row.UserId == userId);

                //get default row if user not exists
                if (data == null)
                    data = context.Configurations.FirstOrDefault(row => row.Key == Key && row.UserId == "");

                if (data == null)
                    return DefaultValue;
                else
                    return data.Value;
            }
        }
   
        /// <summary>
        ///     Return Key value, if not exists return null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string getValue(string key)
        {
             return getValue(key,null);
        }
        /// <summary>
        ///     Return Key value, if not exists return defaultValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">The value in case that the key is not exists in the database</param>
        /// <returns></returns>
        public string getValue(string key,string defaultValue)
        {
            string userId = GetUserID(); 
            if (userId != "" && _dictionary.ContainsKey(userId) && _dictionary[userId].ContainsKey(key))
                return _dictionary[userId][key].Value;
            else if (_dictionary[""].ContainsKey(key))
                return _dictionary[""][key].Value;
            else
                return defaultValue;
        }
        
        /// <summary>
        ///     Return Key value, if not exists return defaultValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">The value in case that the key is not exists in the database</param>
        /// <returns>In case that the value in the database is not int, return defaultValue</returns>
        public int getValue(string key, int defaultValue)
        {
            try
            {
                string userId = GetUserID();

                if (userId != "" && _dictionary.ContainsKey(userId) && _dictionary[userId].ContainsKey(key))
                    return _dictionary[userId][key].Value.ToInt();
                else if (_dictionary[""].ContainsKey(key))
                    return _dictionary[""][key].Value.ToInt();
                else
                    return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        
        /// <summary>
        ///     Return Key value, if not exists return defaultValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">The value in case that the key is not exists in the database</param>
        /// <returns>In case that the value in the database is not int, return defaultValue</returns>
        public bool getValue(string key, bool defaultValue)
        {
            try
            {
                string userId = GetUserID();

                if (userId != "" && _dictionary.ContainsKey(userId) && _dictionary[userId].ContainsKey(key))
                    return _dictionary[userId][key].Value.ToBoolean();
                else if (_dictionary[""].ContainsKey(key))
                    return _dictionary[""][key].Value.ToBoolean();
                else
                    return defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }
        
        /// <summary>
        ///     Return Key value, if not exists return null
        /// </summary>
        /// <param name="key">Key Name</param>
        /// <returns>string - the value of the specified key</returns>
        public string this[string key]
        {
            get
            {
                return getValue(key);
            }
        }
        
        /// <summary>
        ///     Return Key value, if not exists return defaultValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">The value in case that the key is not exists in the database</param>
        /// <returns></returns>
        public string this[string key,string defaultValue]
        {
            get
            {
                return getValue(key, defaultValue);
            }
        }
        
        /// <summary>
        ///     Return Key value, if not exists return defaultValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">The value in case that the key is not exists in the database</param>
        /// <returns></returns>
        public int this[string key, int defaultValue]
        {
            get
            {
                return getValue(key, defaultValue);
            }
        }
        
        /// <summary>
        ///     Return Key value, if not exists return defaultValue
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue">The value in case that the key is not exists in the database</param>
        /// <returns></returns>
        public bool this[string key, bool defaultValue]
        {
            get
            {
                return getValue(key, defaultValue);
            }
        }
        
        /// <summary>
        ///     Returns the description of specified key, if key not exists return null
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string getDescription(string key)
        {
            ConfigurationRow val;
            bool hasKey = _dictionary[""].TryGetValue(key, out val);
            if (!hasKey)
                return null;

            return val.Description;
        }
        
        /// <summary>
        ///     Loads the data from the database into the hash.
        /// </summary>
        public void LoadData()
        {
            LoadCommonData();
        }
        
        /// <summary>
        ///     Loads the data from the database into the hash. override the common data by user specific parameters
        /// </summary>
        public void LoadData(string userId)
        {
            if (!_dictionary.ContainsKey(""))
                LoadCommonData();

            try
            {
                if (userId == "")
                    return;

                //Load user configurations
                if (_dictionary.ContainsKey(userId))
                    _dictionary.Remove(userId);

                using (var context = new ApplicationDBContext())
                {
                    Dictionary<string, ConfigurationRow> userDictionary = context.Configurations.Where(row => row.UserId == userId).ToDictionary(key => key.Key);
                    _dictionary.Add(userId, userDictionary);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load user configurations.", ex);
            }
        }
        /// <summary>
        /// Does the user have value for a given key in the configuration table
        /// </summary>
        /// <param name="key"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool HasValueForUser(string key, string userId)
        {
            return _dictionary.ContainsKey(userId) && _dictionary[userId].ContainsKey(key);
        }
        /// <summary>
        ///     Loads the common data from the database into the hash
        /// </summary>
        private void LoadCommonData()
        {
            try
            {
                using (var context = new ApplicationDBContext())
                {
                    if (_dictionary.ContainsKey(""))
                        _dictionary.Remove("");
                    Dictionary<string, ConfigurationRow> commonDictionary = context.Configurations.Where(row => row.UserId == "").ToDictionary(key => key.Key);
                    _dictionary.Add("", commonDictionary);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to load common configurations.", ex);
            }
        }
        private string GetUserID()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            if (context == null || context.User == null || context.User.Identity.Name == "")
                return "";
            else
                return context.User.Identity.GetUserId();
        }
        #endregion
    }
}
