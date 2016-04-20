var urlPrefix2 = "";
if (location.href.indexOf('localhost/FlatFXWebClient') > -1) {
    urlPrefix2 = "/FlatFXWebClient";
}


var myApp = angular.module('FlatFXAPP', ['ui.bootstrap', 'notyModule', 'ngTable', 'ngStorage', 'fcsa-number']);

myApp.filter('rangeFilter', function () {
    "use strict";
    return function (items, minColumn, min, maxColumn, max) {
        var filtered = [];
        angular.forEach(items, function (item) {
            if (min <= item[minColumn] && item[maxColumn] <= max) {
                filtered.push(item);
            }
        });
        return filtered;
    };
});

myApp.factory('broadcastService', function ($rootScope) {
    return {
        send: function (msg, data) {
            $rootScope.$broadcast(msg, data);
        }
    }
});

myApp.service('SharedDataService', function ($localStorage) {
    "use strict";
    this.Get = function () {
        if ($localStorage.FFXShared === null || $localStorage.FFXShared === undefined) {
            $localStorage.FFXShared = {
                Currencies: {
                    'USD': { ISO: 'USD', Name: 'United States Dollar', Symbol: '$', Img: urlPrefix2 + '/Images/Flags/USD.gif', Bid: 1, Ask: 1, Mid: 1, BidBank: 1, AskBank: 1, BidOrder: 1, AskOrder: 1 },
                    'ILS': { ISO: 'ILS', Name: 'Israeli New Shekel', Symbol: '₪', Img: urlPrefix2 + '/Images/Flags/ILS.png', Bid: 1, Ask: 1, Mid: 1, BidBank: 1, AskBank: 1, BidOrder: 1, AskOrder: 1 },
                    'EUR': { ISO: 'EUR', Name: 'Euro', Symbol: '€', Img: urlPrefix2 + '/Images/Flags/EUR.gif', Bid: 1, Ask: 1, Mid: 1, BidBank: 1, AskBank: 1, BidOrder: 1, AskOrder: 1 }
                },
                ILSUSD: 3.8
            };
        }
        return $localStorage.FFXShared;
    };
});

myApp.service('UpdateFeedService', function ($http, $interval, SharedDataService, broadcastService) {
    "use strict";
    var me = this;
    var RatesUrl = urlPrefix2 + "/OnLineFXRates/GetRates";

    this.RefreshRates = function () {
        $http.get(RatesUrl)
            .success(function (data, status, headers, config) {
                try {
                    angular.forEach(data.Rates, function (value, key) {
                        var currency = value.Key.replace("USD", "");
                        var isOpposite = false;
                        if (currency === "EUR" || currency === "GBP" || currency === "AUD" || currency === "NZD" || currency === "XAU" || currency === "XAG") {
                            isOpposite = true;
                        }
                        if (currency === "EUR" || currency === "ILS") {

                            if (isOpposite) {
                                SharedDataService.Get().Currencies[currency].Mid = 1 / value.Mid;
                                SharedDataService.Get().Currencies[currency].Bid = 1 / value.Bid;
                                SharedDataService.Get().Currencies[currency].Ask = 1 / value.Ask;
                                SharedDataService.Get().Currencies[currency].AskBank = 1 / value.Mid * 1.0112;
                                SharedDataService.Get().Currencies[currency].BidBank = 1 / value.Mid * 0.9888;
                                SharedDataService.Get().Currencies[currency].AskOrder = 1 / value.Mid * 1.002;
                                SharedDataService.Get().Currencies[currency].BidOrder = 1 / value.Mid * 0.998;
                            } else {
                                SharedDataService.Get().Currencies[currency].Mid = value.Mid;
                                SharedDataService.Get().Currencies[currency].Bid = value.Bid;
                                SharedDataService.Get().Currencies[currency].Ask = value.Ask;
                                SharedDataService.Get().Currencies[currency].AskBank = value.Mid * 1.0112;
                                SharedDataService.Get().Currencies[currency].BidBank = value.Mid * 0.9888;
                                SharedDataService.Get().Currencies[currency].AskOrder = value.Mid * 1.002;
                                SharedDataService.Get().Currencies[currency].BidOrder = value.Mid * 0.998;
                            }

                            if (currency === "ILS") {
                                SharedDataService.Get().ILSUSD = value.Mid;
                            }
                        }
                    });

                    broadcastService.send('RateReady', {});

                } catch (err) {

                }
            })
            .error(function (data, status, header, config) {
                console.log('Data: ' + data + '<br />status: ' + status + '<br />headers: ' + jsonFilter(header) + '<br />config: ' + jsonFilter(config));
            });
    };

    $interval(function () {
        me.RefreshRates();
    }, 600000);
});
