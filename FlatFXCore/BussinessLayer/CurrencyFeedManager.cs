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
    /*
    @*<!-- ngRepeat:  -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Euro" currency-symbol="€" data-value="EUR"><a href="#"><img ng-src="/images/flag_icons/europeanunion.png" src="/images/flag_icons/europeanunion.png"><div class="currencyIso ng-binding">EUR</div><span class="currencyName ng-binding">Euro</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="United Kingdom Pound Sterling" currency-symbol="£" data-value="GBP"><a href="#"><img ng-src="/images/flag_icons/gb.png" src="/images/flag_icons/gb.png"><div class="currencyIso ng-binding">GBP</div><span class="currencyName ng-binding">United Kingdom Pound Sterling</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Romanian New Leu" currency-symbol="lei" data-value="RON"><a href="#"><img ng-src="/images/flag_icons/ro.png" src="/images/flag_icons/ro.png"><div class="currencyIso ng-binding">RON</div><span class="currencyName ng-binding">Romanian New Leu</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="United States Dollar" currency-symbol="$" data-value="USD"><a href="#"><img ng-src="/images/flag_icons/us.png" src="/images/flag_icons/us.png"><div class="currencyIso ng-binding">USD</div><span class="currencyName ng-binding">United States Dollar</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="United Arab Emirates Dirham" currency-symbol="د.إ" data-value="AED"><a href="#"><img ng-src="/images/flag_icons/sa.png" src="/images/flag_icons/sa.png"><div class="currencyIso ng-binding">AED</div><span class="currencyName ng-binding">United Arab Emirates Dirham</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Australian Dollar" currency-symbol="$" data-value="AUD"><a href="#"><img ng-src="/images/flag_icons/au.png" src="/images/flag_icons/au.png"><div class="currencyIso ng-binding">AUD</div><span class="currencyName ng-binding">Australian Dollar</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Israeli New Shekel" currency-symbol="₪" data-value="ILS"><a href="#"><img ng-src="/images/flag_icons/il.png" src="/images/flag_icons/il.png"><div class="currencyIso ng-binding">ILS</div><span class="currencyName ng-binding">Israeli New Shekel</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Canadian Dollar" currency-symbol="$" data-value="CAD"><a href="#"><img ng-src="/images/flag_icons/ca.png" src="/images/flag_icons/ca.png"><div class="currencyIso ng-binding">CAD</div><span class="currencyName ng-binding">Canadian Dollar</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Swiss Franc" currency-symbol="CHF" data-value="CHF"><a href="#"><img ng-src="/images/flag_icons/ch.png" src="/images/flag_icons/ch.png"><div class="currencyIso ng-binding">CHF</div><span class="currencyName ng-binding">Swiss Franc</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Czech Koruna" currency-symbol="Kč" data-value="CZK"><a href="#"><img ng-src="/images/flag_icons/cz.png" src="/images/flag_icons/cz.png"><div class="currencyIso ng-binding">CZK</div><span class="currencyName ng-binding">Czech Koruna</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Danish Krone" currency-symbol="kr" data-value="DKK"><a href="#"><img ng-src="/images/flag_icons/dk.png" src="/images/flag_icons/dk.png"><div class="currencyIso ng-binding">DKK</div><span class="currencyName ng-binding">Danish Krone</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Hong Kong Dollar" currency-symbol="$" data-value="HKD"><a href="#"><img ng-src="/images/flag_icons/hk.png" src="/images/flag_icons/hk.png"><div class="currencyIso ng-binding">HKD</div><span class="currencyName ng-binding">Hong Kong Dollar</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Hungarian Forint" currency-symbol="Ft" data-value="HUF"><a href="#"><img ng-src="/images/flag_icons/hu.png" src="/images/flag_icons/hu.png"><div class="currencyIso ng-binding">HUF</div><span class="currencyName ng-binding">Hungarian Forint</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Japanese Yen" currency-symbol="¥" data-value="JPY"><a href="#"><img ng-src="/images/flag_icons/jp.png" src="/images/flag_icons/jp.png"><div class="currencyIso ng-binding">JPY</div><span class="currencyName ng-binding">Japanese Yen</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Norwegian Krone" currency-symbol="kr" data-value="NOK"><a href="#"><img ng-src="/images/flag_icons/no.png" src="/images/flag_icons/no.png"><div class="currencyIso ng-binding">NOK</div><span class="currencyName ng-binding">Norwegian Krone</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="New Zealand Dollar" currency-symbol="$" data-value="NZD"><a href="#"><img ng-src="/images/flag_icons/nz.png" src="/images/flag_icons/nz.png"><div class="currencyIso ng-binding">NZD</div><span class="currencyName ng-binding">New Zealand Dollar</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Polish Zloty" currency-symbol="zł" data-value="PLN"><a href="#"><img ng-src="/images/flag_icons/pl.png" src="/images/flag_icons/pl.png"><div class="currencyIso ng-binding">PLN</div><span class="currencyName ng-binding">Polish Zloty</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Swedish Krona" currency-symbol="kr" data-value="SEK"><a href="#"><img ng-src="/images/flag_icons/se.png" src="/images/flag_icons/se.png"><div class="currencyIso ng-binding">SEK</div><span class="currencyName ng-binding">Swedish Krona</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Singaporean Dollar" currency-symbol="$" data-value="SGD"><a href="#"><img ng-src="/images/flag_icons/sg.png" src="/images/flag_icons/sg.png"><div class="currencyIso ng-binding">SGD</div><span class="currencyName ng-binding">Singaporean Dollar</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="Turkish Lira" currency-symbol="TL" data-value="TRY"><a href="#"><img ng-src="/images/flag_icons/tr.png" src="/images/flag_icons/tr.png"><div class="currencyIso ng-binding">TRY</div><span class="currencyName ng-binding">Turkish Lira</span></a></li><!-- end ngRepeat: currency in currencies -->
            <li ng-click="changeSendCurrency(currency.ISO)" ng-repeat="currency in currencies" class="currency_options ng-scope" currency-name="South African Rand" currency-symbol="R" data-value="ZAR"><a href="#"><img ng-src="/images/flag_icons/za.png" src="/images/flag_icons/za.png"><div class="currencyIso ng-binding">ZAR</div><span class="currencyName ng-binding">South African Rand</span></a></li><!-- end ngRepeat: currency in currencies -->*@
    */
    public class CurrencyManager
    {
        #region Members
        public readonly List<string> CurrencyBeforeUSD = new List<string>() { "EUR", "GBP", "AUD", "NZD", "XAU", "XAG" };
        private static CurrencyManager m_CurrencyManagerInstance = null;
        private Dictionary<string, string> _PairList = new Dictionary<string, string>();
        public Dictionary<string, FXRate> PairRates = new Dictionary<string, FXRate>();
        public DateTime LastFeedUpdate = DateTime.Now.AddDays(-3);
        public List<string> CurrencyList = null;

        public const double BankCommission = 0.0005;
        public const double FlatFXCommission = 0.0025;
        public const double FlatFXOrderCommission = 0.0015;
        public const double CustomerProfit = 0.008;
        public const double CustomerOrderProfit = 0.009;
        public const double TransactionFeeUSD = 17; //10$: USD transfer internal bank, 7$: ILS Zahav
        public const int MinDealAmountUSD = 5000;

        public const double ExtraCharge_EnsureOnLinePrice = 0.001;
        public const double ExtraCharge_PvPEnabled = 0.001;
        public const double ExtraCharge_FastTransferEnabled = 0.001;
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
                    _PairList = db.FXRates.Where(r => r.IsActive == true).OrderBy(r => r.Priority).Select(c => c.Key).ToDictionary(k => k, k => FlatFXResources.Resources.ResourceManager.GetString(k, FlatFXResources.Resources.Culture));
                    PairRates = db.FXRates.Where(r => r.IsActive == true).OrderBy(r => r.Priority).ToDictionary(r => r.Key);
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
            else if (CurrencyBeforeUSD.Contains(currency))
                return amount * GetFXRateVsUSD(currency).Mid;
            else
                return amount * (1 / GetFXRateVsUSD(currency).Mid);
        }
        public double TranslateAmount(double amount, string fromCurrency, string toCurrency)
        {
            if (PairRates.ContainsKey(fromCurrency + toCurrency))
                return amount * PairRates[fromCurrency + toCurrency].Mid;
            else if (PairRates.ContainsKey(toCurrency + fromCurrency))
                return amount * (1 / PairRates[toCurrency + fromCurrency].Mid);
            else
                throw new Exception("Failed to find pair : " + fromCurrency + toCurrency);
        }
        public Dictionary<string, string> CurrencyListByCulture
        {
            get
            {
                Dictionary<string, string> currencyListByCulture = new Dictionary<string, string>();
                foreach (string curr in CurrencyList)
                {
                    currencyListByCulture.Add(curr, curr + " - " + FlatFXResources.Resources.ResourceManager.GetString(curr));
                }
                return currencyListByCulture;
            }
        }
        public Dictionary<string, string> PairsListByCulture(bool isDemo)
        {
            Dictionary<string, string> pairsDic = CurrencyManager.Instance.PairRates.Values.Where(r => (r.IsTradable || isDemo) && r.IsActiveForSimpleTrading && r.Key != "USDUSD").ToDictionary(r => r.Key, r => FlatFXResources.Resources.ResourceManager.GetString(r.Key));
            return pairsDic;
        }
    }
    public class CurrencyFeedManager
    {
        #region Members
        private static CurrencyFeedManager m_CurrencyFeedManagerInstance = null;
        private Timer m_UpdateFeedTimer = null;
        private int m_UpdateFeedTimerInterval = 60 * 1000;
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

                //Insert the response to the DB.
                UpdateFXRatesTables(results);
            }
            catch (Exception ex)
            {
                Logger.Instance.WriteError("Failed in CurrencyFeedManager::UpdateFeedTimer_Elapsed. Response: " + response, ex);
            }
        }

        private void UpdateFXRatesTables(dynamic results)
        {
            bool updateHistoricalData = false;
            if (!m_LastHistoricalUpdate.HasValue || (DateTime.Now - m_LastHistoricalUpdate.Value).TotalDays > 1)
            {
                updateHistoricalData = true;
                m_LastHistoricalUpdate = DateTime.Now;
            }

            DateTime updateTime = DateTime.Now;

            //int timeZoneDiff = 3;
            //DateTime updateTime = results.query.created;
            //if (updateTime > DateTime.Now.AddHours((-1 * timeZoneDiff) - 3))
            //    updateTime = updateTime.AddHours(timeZoneDiff);

            //if (updateTime > DateTime.Now && updateTime < DateTime.Now.AddMinutes(20))
            //    updateTime = DateTime.Now;
            //else
            //    updateTime = updateTime.AddHours(-1 * timeZoneDiff);

            //string updateTimeStr = results.query.created;
            //DateTime updateTime = DateTime.ParseExact(updateTimeStr, "yyyy-MM-dd HH:mm:ss ", CultureInfo.InvariantCulture); //"2015-08-05T15:13:41Z"

            using (var db = new ApplicationDBContext())
            {
                Dictionary<string, FXRate> pairsData = db.FXRates.ToDictionary(r => r.Key);

                foreach (var pairInfo in results.query.results.rate)
                {
                    string key = pairInfo.id;
                    double ask = pairInfo.Ask;
                    double bid = pairInfo.Bid;
                    double mid = (ask + bid) / 2;

                    //Calculate FlatFX prices
                    double spread = mid * (CurrencyManager.BankCommission + CurrencyManager.FlatFXCommission); // 3 Promil
                    bid = System.Math.Round(mid - spread, 4);
                    ask = System.Math.Round(mid + spread, 4);
                    mid = System.Math.Round(mid, 4);

                    string date = pairInfo.Date;
                    string time = pairInfo.Time;
                    //DateTime updateTime = DateTime.ParseExact(date + " " + time.ToLower(), "M/d/yyyy h:mtt", CultureInfo.InvariantCulture);
                    //updateTime = updateTime.AddHours(2);


                    if ((DateTime.Now.DayOfWeek == DayOfWeek.Saturday || DateTime.Now.DayOfWeek == DayOfWeek.Sunday) && Environment.MachineName == "DUDU-HP")
                        updateTime = DateTime.Now;

                    if (CurrencyManager.Instance.LastFeedUpdate < updateTime)
                        CurrencyManager.Instance.LastFeedUpdate = updateTime;

                    #region FXRate table
                    FXRate pairData = null;
                    if (pairsData.ContainsKey(key))
                        pairData = pairsData[key];
                    //pairData = db.FXRates.Where(rate => rate.Key == key).FirstOrDefault();
                    if (pairData == null)
                    {
                        pairData = new FXRate()
                        {
                            Key = key,
                            IsActive = false,
                            LastUpdate = updateTime,
                            IsTradable = false,
                            IsActiveForSimpleTrading = false,
                            Bid = bid,
                            Ask = ask,
                            Mid = mid,
                            KeyDisplay = CurrencyManager.Instance.PairList[key]
                        };
                        db.FXRates.Add(pairData);
                    }
                    else
                    {
                        pairData.Bid = bid;
                        pairData.Ask = ask;
                        pairData.Mid = mid;
                        pairData.LastUpdate = updateTime;
                        pairData.IsTradable = (pairData.IsActive && pairData.IsActiveForSimpleTrading && (DateTime.Now - updateTime).TotalSeconds < 600) ? true : false; //if the rate was updated in the last 10 minutes
                        pairData.KeyDisplay = CurrencyManager.Instance.PairList[key];
                    }
                    #endregion

                    #region Update CurrencyManager PairRates memory cache
                    if (CurrencyManager.Instance.PairRates.ContainsKey(key))
                    {
                        CurrencyManager.Instance.PairRates[key].Bid = bid;
                        CurrencyManager.Instance.PairRates[key].Ask = ask;
                        CurrencyManager.Instance.PairRates[key].Mid = mid;
                        CurrencyManager.Instance.PairRates[key].LastUpdate = updateTime;
                        CurrencyManager.Instance.PairRates[key].IsTradable = (CurrencyManager.Instance.PairRates[key].IsActive &&
                            CurrencyManager.Instance.PairRates[key].IsActiveForSimpleTrading &&
                            (DateTime.Now - updateTime).TotalSeconds < 600) ? true : false; //if the rate was updated in the last 10 minutes
                        CurrencyManager.Instance.PairRates[key].KeyDisplay = CurrencyManager.Instance.PairList[key];
                    }
                    #endregion

                    #region DailyFXRate table
                    if ((DateTime.Now.Minute % 5) == 0)
                    {
                        DailyFXRate pairDailyData = new DailyFXRate()
                            {
                                Key = key,
                                Bid = bid,
                                Ask = ask,
                                Mid = mid,
                                Time = new DateTime(updateTime.Year, updateTime.Month, updateTime.Day, updateTime.Hour, updateTime.Minute, 0)
                            };
                        db.DailyFXRates.Add(pairDailyData);
                    }
                    #endregion

                    #region HistoricalFXRate table
                    if (updateHistoricalData)
                    {
                        DateTime today = DateTime.Today;
                        HistoricalFXRate pairHistoricalData = db.HistoricalFXRates.Where(rate => rate.Key == key && rate.Time > today).FirstOrDefault();
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
        }
    }
}
