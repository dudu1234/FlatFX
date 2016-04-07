var urlPrefix2 = "";
//if (location.hostname == 'localhost') {
if (location.href.indexOf('localhost/FlatFXWebClient') > -1) {
    urlPrefix2 = "/FlatFXWebClient";
}


var myApp = angular.module('FlatFXAPP', ['ui.bootstrap', 'notyModule', 'ngTable', 'fcsa-number']);

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

myApp.service('SharedDataService', function ($http, $interval) {

    var RatesUrl = urlPrefix2 + "/OnLineFXRates/GetRates";

    var Shared = {
        Currencies : {
            'USD': { ISO: 'USD', Name: 'United States Dollar', Symbol: '$', Img: urlPrefix2 + '/Images/Flags/USD.gif', Bid: 1, Ask: 1, Mid: 1, BidBank: 1, AskBank: 1, BidOrder: 1, AskOrder: 1 },
            'ILS': { ISO: 'ILS', Name: 'Israeli New Shekel', Symbol: '₪', Img: urlPrefix2 + '/Images/Flags/ILS.png', Bid: 1, Ask: 1, Mid: 1, BidBank: 1, AskBank: 1, BidOrder: 1, AskOrder: 1 },
            'EUR': { ISO: 'EUR', Name: 'Euro', Symbol: '€', Img: urlPrefix2 + '/Images/Flags/EUR.gif', Bid: 1, Ask: 1, Mid: 1, BidBank: 1, AskBank: 1, BidOrder: 1, AskOrder: 1 }
        },
        ILSUSD : 3.8
    };

    $interval(function () {
        refreshRates();
    }, 60000);

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
                            Shared.Currencies[currency].Mid = (isOpposite) ? 1 / value.Mid : value.Mid;
                            Shared.Currencies[currency].Bid = (isOpposite) ? 1 / value.Bid : value.Bid;
                            Shared.Currencies[currency].Ask = (isOpposite) ? 1 / value.Ask : value.Ask;
                            Shared.Currencies[currency].AskBank = (isOpposite) ? (1 / value.Mid * 1.0112) : (value.Mid * 1.0112);
                            Shared.Currencies[currency].BidBank = (isOpposite) ? (1 / value.Mid * 0.9888) : (value.Mid * 0.9888);
                            Shared.Currencies[currency].AskOrder = (isOpposite) ? (1 / value.Mid * 1.002) : (value.Mid * 1.002);
                            Shared.Currencies[currency].BidOrder = (isOpposite) ? (1 / value.Mid * 0.998) : (value.Mid * 0.998);

                            if (currency == "ILS") {
                                Shared.ILSUSD = value.Mid;
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

    return Shared;
});