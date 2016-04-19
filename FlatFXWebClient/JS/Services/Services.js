var urlPrefix2 = "";
//if (location.hostname == 'localhost') {
if (location.href.indexOf('localhost/FlatFXWebClient') > -1) {
    urlPrefix2 = "/FlatFXWebClient";
}


var myApp = angular.module('FlatFXAPP', ['ui.bootstrap', 'notyModule', 'ngTable', 'ngStorage', 'fcsa-number']);

//https://github.com/FCSAmerica/angular-fcsa-number/blob/master/README.md


//myApp.service('StringManipulationService', function () {
//    this.myFunction = function (str) {
//        return 'ZZZZ';
//    };
//});

//myApp.config('$locationProvider', function ($locationProvider) {
//    $locationProvider.html5Mode(true);
//});

myApp.filter('rangeFilter', function () {
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

myApp.service('SharedDataService', function ($http, $localStorage) {
    if ($localStorage.FFXShared == null || $localStorage.FFXShared == undefined) {
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
});

myApp.service('UpdateFeedService', function ($http, $timeout, $interval, SharedDataService) {

    var RatesUrl = urlPrefix2 + "/OnLineFXRates/GetRates";

    $interval(function () {
        refreshRates();
    }, 60000);

    $timeout(function () { 
        if (SharedDataService.Currencies['ILS'].Mid == 1) {
            refreshRates();
        }
    }, 0);

    var refreshRates = function () {
        $http.get(RatesUrl)
            .success(function (data, status, headers, config) {
                try {
                    angular.forEach(data.Rates, function (value, key) {
                        var currency = value.Key.replace("USD", "");
                        var isOpposite = false;
                        if (currency == "EUR" || currency == "GBP" || currency == "AUD" || currency == "NZD" || currency == "XAU" || currency == "XAG") {
                            isOpposite = true;
                        }
                        if (currency == "EUR" || currency == "ILS") {
                            SharedDataService.Currencies[currency].Mid = (isOpposite) ? 1 / value.Mid : value.Mid;
                            SharedDataService.Currencies[currency].Bid = (isOpposite) ? 1 / value.Bid : value.Bid;
                            SharedDataService.Currencies[currency].Ask = (isOpposite) ? 1 / value.Ask : value.Ask;
                            SharedDataService.Currencies[currency].AskBank = (isOpposite) ? (1 / value.Mid * 1.0112) : (value.Mid * 1.0112);
                            SharedDataService.Currencies[currency].BidBank = (isOpposite) ? (1 / value.Mid * 0.9888) : (value.Mid * 0.9888);
                            SharedDataService.Currencies[currency].AskOrder = (isOpposite) ? (1 / value.Mid * 1.002) : (value.Mid * 1.002);
                            SharedDataService.Currencies[currency].BidOrder = (isOpposite) ? (1 / value.Mid * 0.998) : (value.Mid * 0.998);

                            if (currency == "ILS") {
                                SharedDataService.ILSUSD = value.Mid;
                            }
                        }
                    });
                }
                catch (err) {

                }
            })
            .error(function (data, status, header, config) {
                console.log("Data: " + data +
                    "<br />status: " + status +
                    "<br />headers: " + jsonFilter(header) +
                    "<br />config: " + jsonFilter(config));
            });
    };

    refreshRates();
});

myApp.run(function (UpdateFeedService) {
    var f = UpdateFeedService.refreshRates;
});