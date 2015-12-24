using FlatFXCore.Model.Core;
using FlatFXCore.Model.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Resources;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace FlatFXCore.BussinessLayer
{
    public class CurrencyManager
    {
        #region Members
        public readonly List<string> CurrencyBeforeUSD = new List<string>() { "EUR", "GBP", "AUD", "NZD", "XAU", "XAG" };
        private static CurrencyManager m_CurrencyManagerInstance = null;
        private Dictionary<string, string> _PairList = new Dictionary<string, string>();
        public Dictionary<string, FXRate> PairRates = new Dictionary<string, FXRate>();
        public List<string> CurrencyList = null;
        #endregion

        #region Ctor + Dtor
        /// <summary>
        ///     The Singelton ctor.
        /// </summary>
        internal CurrencyManager()
        {
            try
            {
                using (var db = new ApplicationDBContext())
                {
                    CurrencyList = db.Currencies.Where(c => c.IsActive == true).Select(c => c.Key).ToList();
                    _PairList = db.FXRates.Where(r => r.IsActive == true).Select(c => c.Key).ToDictionary(k => k, k => FlatFXResources.Resources.ResourceManager.GetString(k, FlatFXResources.Resources.Culture));
                    PairRates = db.FXRates.Where(r => r.IsActive == true).ToDictionary(r => r.Key);
                }
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in CurrencyManager::Ctor", ex);
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
        /// CurrencyManager
        /// </summary>
        public static CurrencyManager Instance
        {
            get
            {
                if (m_CurrencyManagerInstance == null)
                    m_CurrencyManagerInstance = new CurrencyManager();

                return m_CurrencyManagerInstance;
            }
        }
        public void Start() { }
        #endregion

        public Dictionary<string, string> PairList
        {
            get
            {
                if (ApplicationInformation.Instance.Session == null)
                    return _PairList;
                if (ApplicationInformation.Instance.Session["PairList"] == null)
                {
                    using (var db = new ApplicationDBContext())
                    {
                        ApplicationInformation.Instance.Session["PairList"] = db.FXRates.Where(r => r.IsActive == true).Select(c => c.Key).ToDictionary(k => k, k => FlatFXResources.Resources.ResourceManager.GetString(k, FlatFXResources.Resources.Culture));
                    }
                }

                return ApplicationInformation.Instance.Session["PairList"] as Dictionary<string, string>;
            }        
        }
        public FXRate GetFXRateVsUSD(string currency)
        {
            if (currency == null || currency == "")
                return null;

            string pair = "";
            currency = currency.ToUpper();
            if (CurrencyBeforeUSD.Contains(currency))
                pair = currency + "USD";
            else
                pair = "USD" + currency;

            return PairRates[pair];
        }
        public double GetAmountUSD(string currency, double amount)
        {
            if (currency.ToUpper() == "USD")
                return amount;
            else 
                return amount * GetFXRateVsUSD(currency).Mid;
        }
    }
    public class CurrencyFeedManager
    {
        #region Members
        private static CurrencyFeedManager m_CurrencyFeedManagerInstance = null;
        private Timer m_UpdateFeedTimer = null;
        private int m_UpdateFeedTimerInterval = 60 * 1000;
        private string m_LastUpdateFeedResponse = "";
        private string m_CurrencyListString = "";
        private DateTime? m_LastHistoricalUpdate = null;

        #endregion

        #region Ctor + Dtor
        /// <summary>
        ///     The Singelton ctor.
        /// </summary>
        internal CurrencyFeedManager()
        {
            try
            {
                foreach (KeyValuePair<string, string> pair in CurrencyManager.Instance.PairList)
                {
                    if (pair.Key != "USDUSD")
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
        public void Start() { }
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
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in CurrencyFeedManager::GetYahooRates", ex);
                return null;
            }
        }
        void UpdateFeedTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            string response = "";
            try
            {
                WebClient web = new WebClient();

                string url = "https://query.yahooapis.com/v1/public/yql?q=select%20*%20from%20yahoo.finance.xchange%20where%20pair%20in%20(%22" +
                    m_CurrencyListString +
                    "%22)&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=";
                response = web.DownloadString(url);

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

                //{"query":{"count":7,"created":"2015-11-18T14:25:25Z","lang":"en-US","results":{"rate":[{"id":"EURUSD","Name":"EUR/USD","Rate":"1.0663","Date":"11/18/2015","Time":"2:25pm","Ask":"1.0666","Bid":"1.0661"},{"id":"USDILS","Name":"USD/ILS","Rate":"3.9007","Date":"11/18/2015","Time":"2:25pm","Ask":"3.9022","Bid":"3.9007"},{"id":"EURILS","Name":"EUR/ILS","Rate":"4.1595","Date":"11/18/2015","Time":"2:25pm","Ask":"4.1620","Bid":"4.1569"},{"id":"GBPILS","Name":"GBP/ILS","Rate":"5.9330","Date":"11/18/2015","Time":"2:25pm","Ask":"5.9354","Bid":"5.9305"},{"id":"JPYILS","Name":"JPY/ILS","Rate":"0.0316","Date":"11/18/2015","Time":"2:25pm","Ask":"0.0316","Bid":"0.0316"},{"id":"CHFILS","Name":"CHF/ILS","Rate":"3.8334","Date":"11/18/2015","Time":"2:25pm","Ask":"3.8358","Bid":"3.8309"},{"id":"AUDILS","Name":"AUD/ILS","Rate":"2.7701","Date":"11/18/2015","Time":"2:25pm","Ask":"2.7715","Bid":"2.7687"}]}}}

                //Convert to Json
                var results = JsonConvert.DeserializeObject<dynamic>(response);
                string date = results.query.results.rate[0].Date;
                string time = results.query.results.rate[0].Time;

                DateTime updateTime = DateTime.ParseExact(date + " " + time.ToLower(), "M/d/yyyy h:mtt", CultureInfo.InvariantCulture);
                updateTime = updateTime.AddHours(2);

                //Update the response in cache
                //response = response.Replace(date, updateTime.ToString("MM/dd/yyyy"));
                //response = response.Replace(time, updateTime.ToString("HH:mm"));
                //m_LastUpdateFeedResponse = response;

                //Insert the response to the DB.
                UpdateFXRatesTables(results, updateTime);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in CurrencyFeedManager::UpdateFeedTimer_Elapsed. Response: " + response, ex);
            }
        }

        private void UpdateFXRatesTables(dynamic results, DateTime updateTime)
        {
            bool updateHistoricalData = false;
            if (!m_LastHistoricalUpdate.HasValue || (DateTime.Now - m_LastHistoricalUpdate.Value).TotalDays > 1)
            {
                updateHistoricalData = true;
                m_LastHistoricalUpdate = DateTime.Now;
            }

            //Create Json string
            StringBuilder sbResponse = new StringBuilder();
            sbResponse.Append("{\"query\": { \"LastUpdate\": \"" + updateTime.ToString("MM/dd/yyyy") + " " + updateTime.ToString("HH:mm") + "\", \"results\": { \"rate\": [");

            using (var db = new ApplicationDBContext())
            {
                bool isFirst = true;
                foreach (var pairInfo in results.query.results.rate)
                {
                    string key = pairInfo.id;
                    double ask = pairInfo.Ask;
                    double bid = pairInfo.Bid;
                    double mid = (ask + bid) / 2;
                    
                    //Calculate FlatFX prices
                    double spread = mid * 0.003; // 3 Promil
                    bid = System.Math.Round(mid - spread, 4);
                    ask = System.Math.Round(mid + spread, 4);
                    mid = System.Math.Round(mid, 4);

                    if (isFirst)
                    {
                        sbResponse.Append("{ \"id\": \"" + key + "\", \"Mid\": \"" + mid + "\", \"Ask\": \"" + ask + "\", \"Bid\": \"" + bid + "\" }");
                        isFirst = false;
                    }
                    else
                    {
                        sbResponse.Append(",{ \"id\": \"" + key + "\", \"Mid\": \"" + mid + "\", \"Ask\": \"" + ask + "\", \"Bid\": \"" + bid + "\" }");
                    }   
                    
                    #region FXRate table
                    FXRate pairData = null;
                    pairData = db.FXRates.Where(rate => rate.Key == key).FirstOrDefault();
                    if (pairData == null)
                    {
                        pairData = new FXRate()
                        {
                            Key = key,
                            IsActive = true,
                            LastUpdate = updateTime,
                            Bid = bid,
                            Ask = ask,
                            Mid = mid
                        };
                        db.FXRates.Add(pairData);
                    }
                    else
                    {
                        pairData.Bid = bid;
                        pairData.Ask = ask;
                        pairData.Mid = mid;
                        pairData.LastUpdate = updateTime;
                    }
                    #endregion

                    #region Update CurrencyManager PairRates memory cache
                    if (CurrencyManager.Instance.PairRates.ContainsKey(key))
                    {
                        CurrencyManager.Instance.PairRates[key].Bid = bid;
                        CurrencyManager.Instance.PairRates[key].Ask = ask;
                        CurrencyManager.Instance.PairRates[key].Mid = mid;
                        CurrencyManager.Instance.PairRates[key].LastUpdate = updateTime;
                    }
                    #endregion

                    #region DailyFXRate table
                    DailyFXRate dailyPairData = null;
                    dailyPairData = db.DailyFXRates.Where(rate => rate.Key == key && rate.Time == updateTime).FirstOrDefault();
                    if (dailyPairData == null)
                    {
                        DailyFXRate pairDailyData = new DailyFXRate()
                            {
                                Key = key,
                                Bid = bid,
                                Ask = ask,
                                Mid = mid,
                                Time = updateTime
                            };
                        db.DailyFXRates.Add(pairDailyData);
                    }
                    #endregion

                    #region HistoricalFXRate table
                    if (updateHistoricalData)
                    {
                        HistoricalFXRate pairHistoricalData = null;
                        pairHistoricalData = db.HistoricalFXRates.Where(rate => rate.Key == key && rate.Time == updateTime).FirstOrDefault();
                        if (pairHistoricalData == null)
                        {
                            pairHistoricalData = new HistoricalFXRate()
                            {
                                Key = key,
                                Bid = bid,
                                Ask = ask,
                                Mid = mid,
                                Time = updateTime
                            };
                            db.HistoricalFXRates.Add(pairHistoricalData);
                        }
                    }
                    #endregion
                }

                db.SaveChanges();
            }

            sbResponse.Append("]}}}");
            m_LastUpdateFeedResponse = sbResponse.ToString();
        }
    }
}
