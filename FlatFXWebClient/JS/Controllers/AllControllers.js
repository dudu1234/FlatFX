
myApp.controller('registerall', ['$scope', function ($scope) {
    //console.log('in controller');
}]);

// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('simpleCurrencyExchange', function ($scope, $timeout, noty) {
    $scope.init = function (isDemo, info, error) {
        $scope.isDemo = isDemo;
        $scope.info = info;
        $scope.error = error;
    };
    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);
    $scope.ready = function () {
        if ($scope.info != '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'success', $scope.info);
        }
        else if ($scope.error != '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'error', $scope.error);
        }
    }
});


// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('adminManager', function ($scope, $timeout, noty) {
    $scope.init = function (info, error) {
        $scope.info = info;
        $scope.error = error;
    };
    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);
    $scope.ready = function () {
        if ($scope.info != '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'success', $scope.info);
        }
        else if ($scope.error != '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'error', $scope.error);
        }
    }
});


// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('userManager', function ($scope, $timeout, noty) {
    $scope.init = function (info, error) {
        $scope.info = info;
        $scope.error = error;
    }
    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);
    $scope.ready = function () {
        if ($scope.info != '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'success', $scope.info);
        }
        else if ($scope.error != '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'error', $scope.error);
        }
    };
});


// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('onLineRatesViewer', function ($scope, $http, $interval, $timeout, noty, StringManipulationService) {
    $scope.init = function (feedUrl) {
        $scope.FeedRatesUrl = feedUrl;
    };

    $scope.showError = function (error) {
        notyWrapper.generateResultMessage($('#resultDiv'), 'error', error);
    };

    $scope.ready = function () {
        $scope.refreshYahooDataFeed();
        $scope.isFirstLoad = true;
    };

    //$scope.$watch('$viewContentLoaded', function (event) { // Use it instead of javascript $(document).ready(
    //        alert($scope.FeedRatesUrl);
    //});

    $scope.refreshYahooDataFeed = function () {
        //$.getJSON($scope.FeedRatesUrl)
        //    .done(function (data) {

        //        try {
        //            $scope.lastUpdate = new Date(parseInt(data.LastFeedUpdate.substr(6)));

        //            angular.forEach(data.Rates, function (value, key) {
        //                value.LastUpdate = new Date(parseInt(value.LastUpdate.substr(6)));
        //            });

        //            $scope.rates = data.Rates;
        //            if ($scope.isFirstLoad == true) 
        //            {
        //                $scope.$apply();
        //                $scope.isFirstLoad = false;
        //            }
        //        }
        //        catch (err) {
        //            $scope.rates = {};
        //            //alert(err);
        //        }
        //    });


        $http.get($scope.FeedRatesUrl)
            .success(function (data, status, headers, config) {
                try {
                    $scope.lastUpdate = new Date(parseInt(data.LastFeedUpdate.substr(6)));

                    angular.forEach(data.Rates, function (value, key) {
                        value.LastUpdate = new Date(parseInt(value.LastUpdate.substr(6)));
                    });

                    $scope.rates = data.Rates;
                }
                catch (err) {
                    $scope.rates = {};
                }
            })
            .error(function (data, status, header, config) {
                $scope.ResponseDetails = "Data: " + data +
                    "<br />status: " + status +
                    "<br />headers: " + jsonFilter(header) +
                    "<br />config: " + jsonFilter(config);
            });
    };

    
    $interval(function () {
        $scope.refreshYahooDataFeed();
    }, 3000);

    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);
});
