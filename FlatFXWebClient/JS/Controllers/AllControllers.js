var urlPrefix = "";
if (location.href.indexOf('localhost/FlatFXWebClient') > -1) {
    urlPrefix = "/FlatFXWebClient";
}


myApp.controller('RegisterAll', function ($scope) {
    "use strict";
});

// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('HomeIndex', function ($scope, $timeout, SharedDataService, UpdateFeedService) {
    "use strict";
    $scope.init = function (isRTL) {
        $scope.amountUSD = 10;
        $scope.exchangeDiscount = 0;
        $scope.spreadDiscount = 0;
        $scope.bankTransferFeeNIS = 70;
        $scope.bankTransferFeeUSD = 18;
        $scope.bankTransferFeeEUR = 16;
        $scope.flatFXCommission = 1.5;
        $scope.bankCommission = 0.5;
        $scope.isRTL = isRTL;
        $scope.LangDir = "";
        if ($scope.isRTL === "True") {
            $scope.LangDir = "Hebrew/";
        }

        $scope.ShowWhy = false;

        $scope.slides = [];

        if ($scope.isRTL === "True") {
            $scope.slides.push({ header: 'רשימת הזמנות המרה', text: 'מצא הזמנת המרה אשר מתאימה לדרישות ההמרה שלך', image: urlPrefix + '/Images/' + $scope.LangDir + 'Crousel_OrderBook.png' });
            $scope.slides.push({ header: 'צור הזמנת המרה', text: 'הכנס הזמנת המרה חדשה והמתן למשתמש אשר יבצע התאמה מולך', image: urlPrefix + '/Images/' + $scope.LangDir + 'Crousel_CreateOrder.png' });
            $scope.slides.push({ header: 'ניהול עיסקאות', text: 'נהל את העיסקאות והזמנות ההמרה', image: urlPrefix + '/Images/' + $scope.LangDir + 'Crousel_ManageDeals.png' });
            $scope.slides.push({ header: 'צפייה בסטטיסטיקה', text: 'צפה בנפח המסחר היומי, הנפח הכללי, הרווח שלך, גרפים, ...', image: urlPrefix + '/Images/' + $scope.LangDir + 'Crousel_Statistics.png' });
            $scope.slides.push({ header: 'בצע המרה', text: 'בצע המרה שתתבצע בזמן אמת מול FlatFX במרווחי המרה אטרקטיביים', image: urlPrefix + '/Images/' + $scope.LangDir + 'Crousel_SimpleExchangeCreate.png' });
            $scope.slides.push({ header: 'אשר ביצוע המרה', text: 'אשר את ביצוע ההמרה, השער והעמלות', image: urlPrefix + '/Images/' + $scope.LangDir + 'Crousel_SimpleExchangeConfirm.png' });
            $scope.slides.push({ header: 'שערים בזמן אמת', text: 'צפה במחירי ההמרה בזמן אמת והשווה את מחירי FlatFX עם מחירי הבנקים', image: urlPrefix + '/Images/' + $scope.LangDir + 'Crousel_Rates.png' });
            $scope.slides.push({ header: 'חדר מסחר', text: 'אתה מוזמן לבקר אותנו במשרדינו בכתובת ...', image: urlPrefix + '/Images/TradingRoom.jpg' });
            $scope.slides.push({ header: 'כתובת', text: 'אתה מוזמן לבקר אותנו במשרדינו בכתובת ...', image: urlPrefix + '/Images/AddressImage.jpg' });
        } else {
            $scope.slides.push({ header: 'Order Book', text: 'Find an order that best match your currency exchange requirements', image: urlPrefix + '/Images/' + $scope.LangDir + 'Crousel_OrderBook.png' });
            $scope.slides.push({ header: 'Create Order', text: 'Enter new currency exchange order and wait for other site user to match your order', image: urlPrefix + '/Images/' + $scope.LangDir + 'Crousel_CreateOrder.png' });
            $scope.slides.push({ header: 'Manage Deals', text: 'Manage your deals & orders. edit, cancel and explore.', image: urlPrefix + '/Images/' + $scope.LangDir + 'Crousel_ManageDeals.png' });
            $scope.slides.push({ header: 'View your Statistics', text: 'View your total volume, today volume, total savings, charts, ...', image: urlPrefix + '/Images/' + $scope.LangDir + 'Crousel_Statistics.png' });
            $scope.slides.push({ header: 'Create Deal', text: 'Create a deal to be performed immediately against FlatFX prices', image: urlPrefix + '/Images/' + $scope.LangDir + 'Crousel_SimpleExchangeCreate.png' });
            $scope.slides.push({ header: 'Deal Confirmation', text: 'Confirm your deal price and deal commission', image: urlPrefix + '/Images/' + $scope.LangDir + 'Crousel_SimpleExchangeConfirm.png' });
            $scope.slides.push({ header: 'OnLine Rates', text: 'See online rates and compare FlatFX prices with your bank prices', image: urlPrefix + '/Images/' + $scope.LangDir + 'Crousel_Rates.png' });
            $scope.slides.push({ header: 'Trading Room', text: 'You are invited to visit our office at ...', image: urlPrefix + '/Images/TradingRoom.jpg' });
            $scope.slides.push({ header: 'Address', text: 'You are invited to visit our office at ...', image: urlPrefix + '/Images/AddressImage.jpg' });
        }
    };
    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);

    $scope.ready = function () {
        var maxLoops = 3;
        var loop = 0;
        var foo = null;
        while (loop < maxLoops && (SharedDataService.Get().Currencies == null || SharedDataService.Get().Currencies.ILS.Mid === 1)) {
            if (loop === 0) {
                UpdateFeedService.RefreshRates();
            }
            loop = loop + 1;
            wait(100);
        }

        $scope.converterAmount = 100000;
        $scope.SendCurrencyISO = 'USD';
        $scope.ReceiveCurrencyISO = 'ILS';
        $scope.Rate = 0;
        $scope.CalcByReceive = true;
        $scope.CalcSave = 0;
        $scope.BankRate = 0;
        $scope.DirectionHeader = "You get";
        if ($scope.isRTL === "True") {
            $scope.DirectionHeader = "אתה מקבל";
        }
        $scope.DirectionSymbol = "";
        $scope.DirectionGetFlatFX = 0;
        $scope.DirectionGetBank = 0;
        $scope.ShowWhy = false;

        if (loop === maxLoops) {
            //location.reload();
        } else {
            $scope.calculateReceive();
        }

        var div1h = document.getElementById('sectionNum1').clientHeight;
        var div2h = document.getElementById('sectionNum2').clientHeight;
        if (div1h > div2h) {
            $('#sectionNum2').height(div1h);
        } else {
            $('#sectionNum1').height(div2h);
        }
    };

    $scope.$on('RateReady', function (event, args) {
        if ($scope.CalcByReceive) {
            $scope.calculateReceive();
        } else {
            $scope.calculateSend();
        }
    });

    $scope.BankCommission = function () {
        if ($scope.spreadDiscount === undefined) {
            $scope.spreadDiscount = 0;
        }
        if ($scope.exchangeDiscount === undefined) {
            $scope.exchangeDiscount = 0;
        }

        return (SharedDataService.Get().ILSUSD * $scope.amountUSD * 1000000 * 0.009 * (1 - (0.01 * $scope.spreadDiscount))) + (SharedDataService.Get().ILSUSD * $scope.amountUSD * 1000000 * 0.0022 * (1 - (0.01 * $scope.exchangeDiscount)));
    };
    $scope.FlatFXCommission = function () {
        return (SharedDataService.Get().ILSUSD * $scope.amountUSD * 1000000 * (0.001 * ($scope.flatFXCommission + $scope.bankCommission)));
    };
    $scope.FlatFXSaving = function () {
        return ($scope.BankCommission() - $scope.FlatFXCommission() - $scope.bankTransferFeeNIS);
    };
    $scope.FlatFXIncome = function () {
        return (SharedDataService.Get().ILSUSD * $scope.amountUSD * 1000000 * 0.001 * $scope.flatFXCommission);
    };
    $scope.BankIncome = function () {
        return (SharedDataService.Get().ILSUSD * $scope.amountUSD * 1000000 * 0.001 * $scope.bankCommission);
    };


    $scope.calculateReceive = function () {
        $scope.DirectionHeader = "You get";
        if ($scope.isRTL === "True") {
            $scope.DirectionHeader = "אתה מקבל";
        }
        $scope.DirectionSymbol = SharedDataService.Get().Currencies[$scope.ReceiveCurrencyISO].Symbol;
        $scope.CalcByReceive = true;

        if ($scope.SendCurrencyISO === 'USD') {
            $scope.Rate = SharedDataService.Get().Currencies[$scope.ReceiveCurrencyISO].BidOrder;
            $scope.IndicativeCalculatedAmount = $scope.converterAmount * $scope.Rate;
            $scope.BankRate = SharedDataService.Get().Currencies[$scope.ReceiveCurrencyISO].BidBank;
        } else if ($scope.ReceiveCurrencyISO === 'USD') {
            $scope.Rate = 1 / SharedDataService.Get().Currencies[$scope.SendCurrencyISO].AskOrder;
            $scope.IndicativeCalculatedAmount = $scope.converterAmount * $scope.Rate;
            $scope.BankRate = 1 / (SharedDataService.Get().Currencies[$scope.SendCurrencyISO].AskBank);
        } else {
            $scope.Rate = SharedDataService.Get().Currencies[$scope.ReceiveCurrencyISO].BidOrder * (1 / SharedDataService.Get().Currencies[$scope.SendCurrencyISO].AskOrder);
            $scope.IndicativeCalculatedAmount = $scope.converterAmount * $scope.Rate;
            $scope.BankRate = SharedDataService.Get().Currencies[$scope.ReceiveCurrencyISO].BidBank * (1 / SharedDataService.Get().Currencies[$scope.SendCurrencyISO].AskBank);
        }

        $scope.DirectionGetFlatFX = $scope.IndicativeCalculatedAmount;
        $scope.DirectionGetBank = $scope.converterAmount * $scope.BankRate;
        $scope.CalcSave = $scope.DirectionGetFlatFX - $scope.DirectionGetBank - $scope.GetTransferFee($scope.ReceiveCurrencyISO);
    };
    $scope.calculateSend = function () {
        $scope.DirectionHeader = "You get";
        if ($scope.isRTL === "True") {
            $scope.DirectionHeader = "אתה מקבל";
        }
        $scope.DirectionSymbol = SharedDataService.Get().Currencies[$scope.SendCurrencyISO].Symbol;
        $scope.CalcByReceive = false;

        if ($scope.SendCurrencyISO === 'USD') {
            $scope.Rate = 1 / SharedDataService.Get().Currencies[$scope.ReceiveCurrencyISO].BidOrder;
            $scope.converterAmount = $scope.IndicativeCalculatedAmount * $scope.Rate;
            $scope.BankRate = 1 / (SharedDataService.Get().Currencies[$scope.ReceiveCurrencyISO].BidBank);
        } else if ($scope.ReceiveCurrencyISO === 'USD') {
            $scope.Rate = SharedDataService.Get().Currencies[$scope.SendCurrencyISO].AskOrder;
            $scope.converterAmount = $scope.IndicativeCalculatedAmount * $scope.Rate;
            $scope.BankRate = SharedDataService.Get().Currencies[$scope.SendCurrencyISO].AskBank;
        } else {
            $scope.Rate = (1 / SharedDataService.Get().Currencies[$scope.ReceiveCurrencyISO].BidOrder) * SharedDataService.Get().Currencies[$scope.SendCurrencyISO].AskOrder;
            $scope.converterAmount = $scope.IndicativeCalculatedAmount * $scope.Rate;
            $scope.BankRate = (1 / SharedDataService.Get().Currencies[$scope.ReceiveCurrencyISO].BidBank) * SharedDataService.Get().Currencies[$scope.SendCurrencyISO].AskBank;
        }

        $scope.DirectionGetFlatFX = $scope.converterAmount;
        $scope.DirectionGetBank = $scope.IndicativeCalculatedAmount * $scope.BankRate;

        $scope.CalcSave = $scope.DirectionGetBank - $scope.DirectionGetFlatFX - $scope.GetTransferFee($scope.SendCurrencyISO);
    };
    $scope.GetTransferFee = function (ISO) {
        var TransferFee = $scope.bankTransferFeeNIS;
        if (ISO === 'USD') {
            TransferFee = $scope.bankTransferFeeUSD;
        } else if (ISO === 'EUR') {
            TransferFee = $scope.bankTransferFeeEUR;
        }
        return TransferFee;
    };
    $scope.changeSendCurrency = function (ISO) {
        if ($scope.ReceiveCurrencyISO === ISO) {
            $scope.ReceiveCurrencyISO = $scope.SendCurrencyISO;
        }

        $scope.SendCurrencyISO = ISO;
        if ($scope.CalcByReceive) {
            $scope.calculateReceive();
        } else {
            $scope.calculateSend();
        }
    };
    $scope.changeReceiveCurrency = function (ISO) {
        if ($scope.SendCurrencyISO === ISO) {
            $scope.SendCurrencyISO = $scope.ReceiveCurrencyISO;
        }

        $scope.ReceiveCurrencyISO = ISO;
        if ($scope.CalcByReceive) {
            $scope.calculateReceive();
        } else {
            $scope.calculateSend();
        }
    };
    $scope.ShowWhyFunc = function () {
        $scope.ShowWhy = !$scope.ShowWhy;
    };
});

// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('SimpleCurrencyExchange', function ($scope, $timeout, $interval, noty) {
    "use strict";
    $scope.init = function (WorkflowStage, isDemo, info, error, CCY1ISO) {
        $scope.isDemo = isDemo;
        $scope.info = info;
        $scope.error = error;
        $scope.CCY1 = CCY1ISO;
        $scope.CCY1Sign = getCurrencySign(CCY1ISO);
        if (WorkflowStage === 2) {
            $scope.CountDown = 60;
        } else {
            $scope.CountDown = 0;
        }
    };
    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);
    $scope.ready = function () {
        if ($scope.info !== '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'success', $scope.info);
        } else if ($scope.error !== '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'error', $scope.error);
        }
    };
    $interval(function () {
        if ($scope.CountDown < 1) {
            return;
        }

        if ($scope.CountDown === 1) {
            $('#confirm-submit').attr("disabled", true);
            $('#confirm-countdown').removeClass('countdown-enabled').addClass('countdown-disabled');
        }

        $scope.CountDown = $scope.CountDown - 1;
    }, 1000);
    $scope.changeCCY1 = function () {
        if ($scope.CCY1 === 'USD') {
            $scope.CCY1Sign = '$';
        } else if ($scope.CCY1 === 'EUR') {
            $scope.CCY1Sign = '€';
        } else if ($scope.CCY1 === 'ILS') {
            $scope.CCY1Sign = '₪';
        } else {
            $scope.CCY1Sign = '???';
        }
    };
});

// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('AdminManager', function ($scope, $timeout, noty) {
    "use strict";
    $scope.init = function (info, error) {
        $scope.info = info;
        $scope.error = error;
    };
    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);
    $scope.ready = function () {
        if ($scope.info !== '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'success', $scope.info);
        } else if ($scope.error !== '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'error', $scope.error);
        }
    };
});

// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('UserManager', function ($scope, $timeout, noty) {
    "use strict";
    $scope.init = function (info, error) {
        $scope.info = info;
        $scope.error = error;
    };
    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);
    $scope.ready = function () {
        if ($scope.info !== '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'success', $scope.info);
        } else if ($scope.error !== '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'error', $scope.error);
        }
    };
});

// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('OnLineRatesViewer', function ($scope, $http, $interval, $timeout, noty) {
    "use strict";
    $scope.init = function (feedUrl) {
        $scope.FeedRatesUrl = feedUrl;
        $scope.SimpleTradingUrl = urlPrefix + "/SimpleCurrencyExchange/StartTrade";
    };

    $scope.showError = function (error) {
        notyWrapper.generateResultMessage($('#resultDiv'), 'error', error);
    };

    $scope.ready = function () {
        $scope.refreshYahooDataFeed();
        $scope.isFirstLoad = true;
    };

    $scope.refreshYahooDataFeed = function () {
        $http.get($scope.FeedRatesUrl)
            .success(function (data, status, headers, config) {
                try {
                    //$scope.lastUpdate = new Date(parseInt(data.LastFeedUpdate.replace('/Date(', '').replace(')/', '')));
                    $scope.lastUpdate = new Date(data.LastFeedUpdate);
                    angular.forEach(data.Rates, function (value, key) {
                        value.LastUpdate = $scope.lastUpdate;
                    });

                    $scope.rates = data.Rates;
                } catch (err) {
                    $scope.rates = {};
                }
            })
            .error(function (data, status, header, config) {
                $scope.ResponseDetails = 'Data: ' + data + '<br />status: ' + status + '<br />headers: ' + jsonFilter(header) + '<br />config: ' + jsonFilter(config);
            });
    };


    $interval(function () {
        $scope.refreshYahooDataFeed();
    }, 60000);

    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);

    $scope.getHref = function (key, direction) {
        return $scope.SimpleTradingUrl + '?key=' + key + '&direction=' + direction;
    };
});

// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('Dashboard', function ($scope, $timeout, $http, noty, NgTableParams) {
    "use strict";
    $scope.init = function (tabName) {

        if (tabName === undefined || tabName === null) {
            alert('undefined');
            tabName = 'OpenDeals';
        }

        $scope.radioDataModel = tabName;

        $scope.companyChart = "Daily";
        $scope.siteChart = "Daily";
        $scope.GetCompanyVolumeUrl = urlPrefix + "/Dashboard/GetCompanyVolume";
        $scope.GetSiteVolumeUrl = urlPrefix + "/Dashboard/GetSiteVolume";

        $scope.dealsUrl = urlPrefix + "/Dashboard/GetDeals";
        $scope.orderByColumn = 'DealId';
        $scope.dealTableParams = new NgTableParams({ count: 10 }, { data: [] });
        $scope.onlyActiveDeals = true;

        $scope.ordersUrl = urlPrefix + "/Dashboard/GetOrders";
        $scope.orderByColumn2 = 'OrderId';
        $scope.orderTableParams = new NgTableParams({ count: 10 }, { data: [] });
        $scope.onlyActiveOrders = true;

        $scope.cancelUrl = urlPrefix + "/Dashboard/Cancel";

        $scope.CompanyVolume = "";
        $scope.CompanyTodayVolume = "";
        $scope.CompanySavings = "";
        $scope.CompanyNumberOfDeal = "";

        $scope.SiteTotalVolume = "";
        $scope.SiteTodayVolume = "";
        $scope.SiteTotalSavings = "";
        $scope.SiteTotalNumberOfDeals = "";

        $scope.BuySellFilterData = [{ id: '', title: 'All' }, { id: 1, title: 'Buy' }, { id: 2, title: 'Sell' }];
        $scope.DealStatusFilterData = [{ id: '', title: 'All' }, { id: 'None', title: 'None' }, { id: 'New', title: 'New' }, { id: 'CustomerTransfer', title: 'Customer Transfer' }, { id: 'FlatFXTransfer', title: 'FlatFX Transfer' }, { id: 'Closed', title: 'Closed' }, { id: 'Canceled', title: 'Canceled' }, { id: 'Problem', title: 'Problem' }];
        $scope.OrderStatusFilterData = [{ id: '', title: 'All' }, { id: 'None', title: 'None' }, { id: 'Waiting', title: 'Waiting' }, { id: 'Triggered', title: 'Triggered' }, { id: 'Closed_Successfully', title: 'Closed Successfully' }, { id: 'Canceled', title: 'Canceled' }, { id: 'Problem', title: 'Problem' }, { id: 'Expired', title: 'Expired' }, { id: 'Triggered_partially', title: 'Triggered partially' }];
        $scope.IsCancelFilterData = [{ id: '', title: 'All' }, { id: 'false', title: 'false' }, { id: 'true', title: 'true' }];
    };
    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);
    $scope.ready = function () {
        $scope.changeData();
    };
    $scope.updateChart = function (chartName, data, fillColor) {
        if (data !== null) {
            var labels = [];
            var dataList = [];

            var i;
            for (i = 0; i < data.length; i++) {
                labels.push(data[i].Key);
                dataList.push(data[i].Value);
            }

            $scope.monthlyCustomerChartLabels = labels;
            $scope.monthlyCustomerChartData = dataList;


            var dataSite = {
                labels: labels, //["July", "August", "September", "October", "November", "December", "January"],
                datasets: [
                    {
                        //label: "My Second dataset",
                        fillColor: fillColor, //rgba(151,187,305,0.8)
                        strokeColor: "rgba(151,187,205,0.8)",
                        highlightFill: "rgba(151,187,205,0.75)",
                        highlightStroke: "rgba(151,187,205,1)",
                        data: dataList //[230034, 400500, 500200, 800200, 1200400, 1400220, 900300]
                    }
                ]
            };

            var ctx = $(chartName).get(0).getContext("2d");
            var myNewChart = new Chart(ctx);
            myNewChart.Bar(dataSite, {
                scaleShowGridLines: false,
                responsive: true,
                scaleFontColor: "#000"
            });
        }
    };
    $scope.RefreshDeals = function () {
        $scope.onlyActiveDeals = ($scope.radioDataModel === 'OpenDeals');
        $http.get($scope.dealsUrl, { params: { onlyActiveDeals: $scope.onlyActiveDeals } })
            .success(function (data, status, headers, config) {
                try {
                    var tableParams = {
                        count: 10
                    };
                    var tableSettings = {
                        filterDelay: 0
                    };
                    tableSettings.data = data.Deals;
                    $scope.dealTableParams = new NgTableParams(tableParams, tableSettings);
                } catch (err) {
                    $scope.dealTableParams = [];
                    $scope.dealData = {
                    };
                }
            })
            .error(function (data, status, header, config) {
                $scope.ResponseDetails = 'Data: ' + data + '<br />status: ' + status + '<br />headers: ' + jsonFilter(header) + '<br />config: ' + jsonFilter(config);
            });
    };
    $scope.RefreshOrders = function () {
        $scope.onlyActiveOrders = ($scope.radioDataModel === 'OpenOrders');
        $http.get($scope.ordersUrl, { params: { onlyActiveOrders: $scope.onlyActiveOrders } })
            .success(function (data, status, headers, config) {
                try {
                    var tableParams = {
                        count: 10
                    };
                    var tableSettings = {
                        filterDelay: 0
                    };
                    tableSettings.data = data.Orders;
                    $scope.orderTableParams = new NgTableParams(tableParams, tableSettings);
                } catch (err) {
                    $scope.orderTableParams = [];
                }
            })
            .error(function (data, status, header, config) {
                $scope.ResponseDetails = 'Data: ' + data + '<br />status: ' + status + '<br />headers: ' + jsonFilter(header) + '<br />config: ' + jsonFilter(config);
            });
    };
    $scope.changeData = function () {
        if ($scope.radioDataModel === 'OpenDeals' || $scope.radioDataModel === 'DealHistory') {
            $scope.RefreshDeals();
        } else if ($scope.radioDataModel === 'OpenOrders' || $scope.radioDataModel === 'OrderHistory') {
            $scope.RefreshOrders();
        } else if ($scope.radioDataModel === 'Statistics') {
            $http.get($scope.GetCompanyVolumeUrl)
                .success(function (data) {
                    $scope.CompanyVolume = numberWithCommas(data.DashboardStatisticsViewModel.CompanyVolume) + "$";
                    $scope.CompanyTodayVolume = numberWithCommas(data.DashboardStatisticsViewModel.CompanyTodayVolume) + "$";
                    $scope.CompanySavings = numberWithCommas(data.DashboardStatisticsViewModel.CompanySavings) + "$";
                    $scope.CompanyNumberOfDeal = numberWithCommas(data.DashboardStatisticsViewModel.CompanyNumberOfDeal);

                    $scope.updateChart("#dashboardCompanyMonthlyChart", data.companyMonthlyVolume, "rgba(255,100,100,0.8)");
                    $scope.updateChart("#dashboardCompanyDailyChart", data.companyDailyVolume, "rgba(255,0,0,0.8)");
                })
                .error(function (data, status) {
                    //console.log("Error status : " + status);
                });
        } else if ($scope.radioDataModel === 'SiteStatistics') {
            $http.get($scope.GetSiteVolumeUrl)
                .success(function (data) {
                    $scope.SiteTotalVolume = numberWithCommas(data.DashboardStatisticsViewModel.SiteTotalVolume) + "$";
                    $scope.SiteTodayVolume = numberWithCommas(data.DashboardStatisticsViewModel.SiteTodayVolume) + "$";
                    $scope.SiteTotalSavings = numberWithCommas(data.DashboardStatisticsViewModel.SiteTotalSavings) + "$";
                    $scope.SiteTotalNumberOfDeals = numberWithCommas(data.DashboardStatisticsViewModel.SiteTotalNumberOfDeals);

                    $scope.updateChart("#dashboardSiteDailyChart", data.dailyVolume, "rgba(200,200,300,0.8)");
                    $scope.updateChart("#dashboardSiteMonthlyChart", data.monthlyVolume, "rgba(100,100,305,0.8)");
                })
                .error(function (data, status) {
                    //console.log("Error status : " + status);
                });
        } else {
            $scope.Deals = null;
        }
    };

    $scope.cancelOrder = function (orderId, order) {
        //if (!window.confirm("Are you sure you want to cancel order #" + orderId + "?"))
        //  return;

        $http.get($scope.cancelUrl, { params: { type: 'Order', id: orderId } })
            .success(function (data, status, headers, config) {
                try {
                    if (data.Error === "") {
                        notyWrapper.generateResultMessage($('#resultDiv'), 'success', 'Order #' + orderId + ' was canceled');
                        order.Status = "Canceled";
                    } else {
                        notyWrapper.generateResultMessage($('#resultDiv'), 'error', 'Failed to cancel order #' + orderId);
                    }
                } catch (err) {
                    notyWrapper.generateResultMessage($('#resultDiv'), 'error', 'Failed to cancel order #' + orderId);
                }
            })
            .error(function (data, status, header, config) {
                notyWrapper.generateResultMessage($('#resultDiv'), 'error', 'Failed to cancel order #' + orderId);
            });
    };

    $scope.EditOrder = function (orderId) {
        window.location.href = urlPrefix + "/Order/EditOrder?orderId=" + orderId;
    };
});

// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('OrderWorkflow', function ($scope, $timeout, $interval, noty) {
    "use strict";
    $scope.init = function (WorkflowStage, isDemo, info, error, amountCCY1, dExpiryDate, MinimalPartnerExecutionAmountCCY1, MatchMinAmount, MatchMaxAmount, Symbol) {
        $scope.isDemo = isDemo;
        $scope.info = info;
        $scope.error = error;
        $scope.actionDescription = "";
        $scope.amountCcy1 = amountCCY1;
        $scope.CCY1Sign = "$";
        $scope.MatchMinAmount = MatchMinAmount;
        $scope.MatchMaxAmount = MatchMaxAmount;
        $scope.CountDown = 60;
        $scope.WorkflowStage = WorkflowStage;
        $scope.Symbol = Symbol;

        if (dExpiryDate == 0) {
            $scope.expiryDateChkModel = false;
            $scope.ExpiryDateModel = '';
        } else {
            $scope.expiryDateChkModel = true;
            $scope.ExpiryDateModel = new Date(dExpiryDate);
        }

        if (MinimalPartnerExecutionAmountCCY1 == 0) {
            $scope.minimalPartnerExecutionAmountChkModel = false;
        } else {
            $scope.minimalPartnerExecutionAmountChkModel = true;
        }
    };
    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);
    $interval(function () {
        if ($scope.WorkflowStage != 2) {
            return;
        }

        if ($scope.CountDown < 1) {
            return;
        }

        if ($scope.CountDown === 1) {
            $('#confirm-submit').attr("disabled", true);
            $('#confirm-countdown').removeClass('countdown-enabled').addClass('countdown-disabled');
        }

        $scope.CountDown = $scope.CountDown - 1;
    }, 1000);
    $scope.ready = function () {
        if ($scope.info !== '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'success', $scope.info);
        } else if ($scope.error !== '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'error', $scope.error);
        }

        if ($scope.WorkflowStage === 1) {
            //$scope.Symbol = $('#Symbol').val();
            $scope.setAction();

            if ($scope.ExpiryDateModel == '') {
                $('#GTC').show();
                $("#ExpiryDate").hide();
            } else {
                $('#ExpiryDate').show();
                $("#GTC").hide();
            }

            if ($scope.minimalPartnerExecutionAmountChkModel == false) {
                $('#AllAmount').show();
                $("#MinimalPartnerExecutionAmountCCY1").hide();
            } else {
                $('#MinimalPartnerExecutionAmountCCY1').show();
                $("#AllAmount").hide();
            }
        }
    };

    $scope.getCustomerSaving = function () {
        return (((0.001 * 11) - (0.001 * 2)) * parseInt($scope.amountCcy1)) - 17;
    };

    $scope.setAction = function () {
        if ($scope.amountCcy1 === null) {
            return;
        }

        if ($scope.Symbol === undefined) {
            return;
        }

        if ($('input[name="BuySell"]:checked').val() === "Buy") {
            $scope.actionDescription = 'Buy ' + $scope.amountCcy1.toLocaleString() + ' ' + $scope.Symbol.substring(0, 3) + ' by selling ' + $scope.Symbol.substring(3, 6);
        } else {
            $scope.actionDescription = 'Buy ' + $scope.Symbol.substring(3, 6) + ' by selling ' + $scope.amountCcy1.toLocaleString() + ' ' + $scope.Symbol.substring(0, 3);
        }
        $scope.symbolDisplay = $scope.actionDescription;

        if ($scope.CCY1() === 'USD') {
            $scope.CCY1Sign = '$';
        } else if ($scope.CCY1() === 'EUR') {
            $scope.CCY1Sign = '€';
        } else if ($scope.CCY1() === 'ILS') {
            $scope.CCY1Sign = '₪';
        } else {
            $scope.CCY1Sign = '???';
        }
    };
    $scope.updateAction = function () {
        $scope.setAction($scope.Symbol);
    };
    $scope.CCY1 = function () {
        if (typeof $scope.Symbol != 'undefined') {
            return $scope.Symbol.substring(0, 3);
        } else {
            return '';
        }
    };
    $scope.CCY2 = function () {
        if (typeof $scope.Symbol != 'undefined') {
            return $scope.Symbol.substring(3, 6);
        } else {
            return '';
        }
    };
    $scope.glyphiconCCY1 = function () {
        if ($scope.CCY1 === "USD") {
            return 'glyphicon-usd';
        } else if (cope.CCY1 === "EUR") {
            return 'glyphicon-eur';
        } else {
            return '';
        }
    };
    $scope.expiryDateCheckboxEvent = function ($event) {
        if ($event) {
            var today = $scope.EndOfDay();
            $scope.ExpiryDateModel = today;
            $("#ExpiryDate").show();
            $('#GTC').hide();
        } else {
            $('#GTC').show();
            $scope.ExpiryDateModel = '';
            $("#ExpiryDate").hide();
        }
    };
    $scope.minimalPartnerCheckboxEvent = function ($event) {
        if ($event) {
            $('#AllAmount').hide();
            $("#MinimalPartnerExecutionAmountCCY1").show();
        } else {
            $('#AllAmount').show();
            $("#MinimalPartnerExecutionAmountCCY1").val('');
            $("#MinimalPartnerExecutionAmountCCY1").hide();
        }
    };
    $scope.EndOfDay = function () {
        var end = new Date();
        end.setHours(23, 59, 59, 999);
        return end;
    };
});

// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('OrderBook', function ($scope, $http, $interval, $timeout, noty) {
    "use strict";
    $scope.init = function (isDemo) {
        $scope.isDemo = isDemo;
        $scope.orderBookIndexUrl = urlPrefix + "/OrderBook/LoadData";
        $scope.newOrderWithMatchUrl = urlPrefix + "/Order/NewOrderWithMatch";

        $scope.Key = 'USDILS';
        $scope.KeyDisplay = 'USDILS';
        $scope.MidRate = 0;
        $scope.maxAmountBuy = 5000000;
        $scope.minAmountBuy = 0;
        $scope.orderBySell = 'MaxAmount';
        $scope.orderByBuy = 'MaxAmount';
        $scope.OrdersToBuy = {
        };
        $scope.OrdersToSell = {
        };
        $scope.Pairs = null;
    };

    $scope.ready = function () {
        $scope.refreshOrderBook();
    };

    $interval(function () {
        $scope.refreshOrderBook();
    }, 60000);

    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);

    $scope.refreshOrderBook = function () {
        $http.get($scope.orderBookIndexUrl, { params: { key: $scope.Key } })
            .success(function (data, status, headers, config) {
                try {
                    if ($scope.Pairs === null) {
                        $scope.Pairs = data.Pairs;
                    }
                    $scope.Key = data.Key;
                    $scope.KeyDisplay = data.KeyDisplay;
                    $scope.MidRate = data.MidRate;
                    $scope.OrdersToBuy = data.OrdersToBuy;
                    $scope.OrdersToSell = data.OrdersToSell;
                } catch (err) {
                    $scope.OrdersToBuy = {
                    };
                    $scope.OrdersToSell = {
                    };
                    $scope.MidRate = 0;
                }
            })
            .error(function (data, status, header, config) {
                $scope.ResponseDetails = 'Data: ' + data + '<br />status: ' + status + '<br />headers: ' + jsonFilter(header) + '<br />config: ' + jsonFilter(config);
            });
    };

    $scope.changePair = function () {
        $scope.refreshOrderBook();
    };

    $scope.changeSortingB = function (columnName) {
        if ($scope.orderByBuy === columnName) {
            $scope.orderByBuy = "-" + columnName;
        } else {
            $scope.orderByBuy = columnName;
        }
    };

    $scope.changeSortingS = function (columnName) {
        if ($scope.orderBySell === columnName) {
            $scope.orderBySell = "-" + columnName;
        } else {
            $scope.orderBySell = columnName;
        }
    };

    $scope.CCY1 = function () {
        if (typeof $scope.Key != 'undefined') {
            return $scope.Key.substring(0, 3);
        } else {
            return '';
        }
    };
    $scope.CCY2 = function () {
        if (typeof $scope.Key != 'undefined') {
            return $scope.Key.substring(3, 6);
        } else {
            return '';
        }
    };
    $scope.createNewOrderWithMatch = function (order, columnName) {
        if (columnName === 'Min') {
            window.location.href = $scope.newOrderWithMatchUrl + "Min?matchOrderId=" + order.OrderId + '&action=0';
        } else if (columnName === 'Max') {
            window.location.href = $scope.newOrderWithMatchUrl + "Max?matchOrderId=" + order.OrderId + '&action=1';
        }
    };
});

// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('OrderLayout', function ($scope, $http, $timeout, noty) {
    "use strict";
    $scope.init = function (isDemo) {
        $scope.isDemo = isDemo;
    };

    $scope.ready = function () {

    };

    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);

});

// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('OrderData', function ($scope, $timeout, $interval, $http, noty, NgTableParams) {
    "use strict";
    $scope.init = function (tabName) {

        if (tabName === undefined || tabName === null) {
            tabName = 'OpenOrders';
        }

        $scope.DataModel = tabName;

        $scope.ordersUrl = urlPrefix + "/Dashboard/GetOrders";
        $scope.orderByColumn2 = 'OrderId';
        $scope.orderTableParams = new NgTableParams({ count: 10 }, { data: [] });
        $scope.onlyActiveOrders = true;

        $scope.cancelUrl = urlPrefix + "/Dashboard/Cancel";

        $scope.BuySellFilterData = [{ id: '', title: 'All' }, { id: 1, title: 'Buy' }, { id: 2, title: 'Sell' }];
        $scope.OrderStatusFilterData = [{ id: '', title: 'All' }, { id: 'None', title: 'None' }, { id: 'Waiting', title: 'Waiting' }, { id: 'Triggered', title: 'Triggered' }, { id: 'Closed_Successfully', title: 'Closed Successfully' }, { id: 'Canceled', title: 'Canceled' }, { id: 'Problem', title: 'Problem' }, { id: 'Expired', title: 'Expired' }, { id: 'Triggered_partially', title: 'Triggered partially' }];
        $scope.IsCancelFilterData = [{ id: '', title: 'All' }, { id: 'false', title: 'false' }, { id: 'true', title: 'true' }];
    };
    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);
    $scope.ready = function () {
        $scope.changeData();
    };
    $scope.RefreshOrders = function () {
        $scope.onlyActiveOrders = ($scope.DataModel === 'OpenOrders');
        $http.get($scope.ordersUrl, { params: { onlyActiveOrders: $scope.onlyActiveOrders } })
            .success(function (data, status, headers, config) {
                try {
                    var tableParams = { count: 10 };
                    var tableSettings = { filterDelay: 0 };
                    tableSettings.data = data.Orders;
                    $scope.orderTableParams = new NgTableParams(tableParams, tableSettings);
                } catch (err) {
                    $scope.orderTableParams = [];
                }
            })
            .error(function (data, status, header, config) {
                $scope.ResponseDetails = 'Data: ' + data + '<br />status: ' + status + '<br />headers: ' + jsonFilter(header) + '<br />config: ' + jsonFilter(config);
            });
    };
    $scope.changeData = function () {
        $scope.RefreshOrders();
    };

    $scope.cancelOrder = function (orderId, order) {
        //if (!window.confirm("Are you sure you want to cancel order #" + orderId + "?"))
        //  return;

        $http.get($scope.cancelUrl, { params: { type: 'Order', id: orderId } })
            .success(function (data, status, headers, config) {
                try {
                    if (data.Error === "") {
                        notyWrapper.generateResultMessage($('#resultDiv'), 'success', 'Order #' + orderId + ' was canceled');
                        order.Status = "Canceled";
                    } else {
                        notyWrapper.generateResultMessage($('#resultDiv'), 'error', 'Failed to cancel order #' + orderId);
                    }
                } catch (err) {
                    notyWrapper.generateResultMessage($('#resultDiv'), 'error', 'Failed to cancel order #' + orderId);
                }
            })
            .error(function (data, status, header, config) {
                notyWrapper.generateResultMessage($('#resultDiv'), 'error', 'Failed to cancel order #' + orderId);
            });
    };

    $scope.EditOrder = function (orderId) {
        window.location.href = urlPrefix + "/Order/EditOrder?orderId=" + orderId;
    };
});

// -----------------------------------------------------------------------------------------------------------------------------------------------------
