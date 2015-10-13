using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace FlatFXCore.BussinessLayer
{
    public class CurrencyFeedManager
    {
        #region Members
        private static CurrencyFeedManager m_CurrencyFeedManagerInstance = null;
        private Timer m_UpdateFeedTimer = null;
        private int m_UpdateFeedTimerInterval = 60 * 1000;

        private string m_LastUpdateFeedResponse = "";

        public List<KeyValuePair<string, string>> CurrencyList = new List<KeyValuePair<string, string>>();
        private string m_CurrencyListString = "";
        #endregion

        #region Ctor + Dtor
        /// <summary>
        ///     The Singelton ctor.
        /// </summary>
        internal CurrencyFeedManager()
        {
            try
            {
                CurrencyList.Add(new KeyValuePair<string, string>("EURUSD", "יורו - דולר"));
                CurrencyList.Add(new KeyValuePair<string, string>("USDILS", "דולר ארה\"ב - שקל"));
                CurrencyList.Add(new KeyValuePair<string,string>("EURILS", "יורו - שקל"));
                CurrencyList.Add(new KeyValuePair<string, string>("GBPILS", "לירה שטרלינג - שקל"));
                CurrencyList.Add(new KeyValuePair<string, string>("JPYILS", "ין יפני - שקל"));
                CurrencyList.Add(new KeyValuePair<string, string>("CHFILS", "פרנק שווצרי - שקל"));
                CurrencyList.Add(new KeyValuePair<string, string>("AUDILS", "דולר אוסטרלי - שקל"));

                foreach (KeyValuePair<string, string> pair in CurrencyList)
                {
                    m_CurrencyListString += pair.Key + ",";
                }
                m_CurrencyListString = m_CurrencyListString.TrimEnd(',');

                m_UpdateFeedTimer = new Timer(m_UpdateFeedTimerInterval);
                // Have the timer fire repeated events (true is the default)
                m_UpdateFeedTimer.AutoReset = true;
                m_UpdateFeedTimer.Elapsed += UpdateFeedTimer_Elapsed;
                // Start the timer
                m_UpdateFeedTimer.Enabled = true;

                UpdateFeedTimer_Elapsed(null, null);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in CurrencyFeedManager::Ctor", ex);
            }
        }
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (m_UpdateFeedTimer != null)
                {
                    m_UpdateFeedTimer.Stop();
                    m_UpdateFeedTimer = null;
                }
            }
        }
        /// <summary>
        /// Logger
        /// </summary>
        public static CurrencyFeedManager Instance
        {
            get
            {
                if (m_CurrencyFeedManagerInstance == null)
                    m_CurrencyFeedManagerInstance = new CurrencyFeedManager();

                return m_CurrencyFeedManagerInstance;
            }
        }
        #endregion

        /// <summary>
        /// GetYahooRates
        /// </summary>
        /// <returns></returns>
        public string GetYahooRates()
        {
            try
            {
                return m_LastUpdateFeedResponse;
            }
            catch(Exception ex)
            {
                Logger.Instance.WriteError("Failed in CurrencyFeedManager::GetYahooRates", ex);
                return null;
            }
        }
        void UpdateFeedTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                WebClient web = new WebClient();

                string url = "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.xchange%20where%20pair%20in%20(%22" +
                    m_CurrencyListString +
                    "%22)&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=";
                string response = web.DownloadString(url);

                /*
                    {
                        "query": {
                            "count": 3, 
                            "created": "2015-08-05T15:13:41Z",
                            "lang": "en-US",
                            "results": { "rate": [
                                { "id": "USDILS", "Name": "USD/ILS", "Rate": "3.8135", "Date": "8/5/2015", "Time": "4:13pm", "Ask": "3.8148", "Bid": "3.8135" },
                                { "id": "EURUSD", "Name": "EUR/USD", "Rate": "1.0874", "Date": "8/5/2015", "Time": "4:13pm", "Ask": "1.0875", "Bid": "1.0874" }, 
                                { "id": "EURILS", "Name": "EUR/ILS", "Rate": "4.1469", "Date": "8/5/2015", "Time": "4:13pm", "Ask": "4.1486", "Bid": "4.1451" } ] }
                        }
                    }
                 */
 
                //Convert to Json
                var results = JsonConvert.DeserializeObject<dynamic>(response);
                string date = results.query.results.rate[0].Date;
                string time = results.query.results.rate[0].Time;

                DateTime dt = DateTime.ParseExact(date + " " + time.ToLower(), "M/d/yyyy h:mtt", CultureInfo.InvariantCulture);
                dt = dt.AddHours(2);

                response = response.Replace(date, dt.ToString("MM/dd/yyyy"));
                response = response.Replace(time, dt.ToString("HH:mm"));

                //Insert the response to the DB.


                //Update the response in cache
                m_LastUpdateFeedResponse = response;
            }
            catch(Exception ex)
            {
                Logger.Instance.WriteError("Failed in CurrencyFeedManager::UpdateFeedTimer_Elapsed", ex);
            }
        }
    }
}
