
myApp.controller('RegisterAll', function ($scope) {
    //console.log('in controller');
});

// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('HomeIndex', function ($scope, $timeout, noty) {
    $scope.init = function () {
        $scope.amountUSD = 10;
        $scope.exchangeDiscount = 0;
        $scope.spreadDiscount = 0;
    };
    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);
    $scope.ready = function () {
        
    }
    $scope.BankCommission = function () {
        return (4 * $scope.amountUSD * 1000000 * 0.0075 * (1 - (0.01 * $scope.spreadDiscount))) + (4 * $scope.amountUSD * 1000000 * 0.002 * (1 - (0.01 * $scope.exchangeDiscount)));
    }
    $scope.FlatFXCommission = function () {
        return (4 * $scope.amountUSD * 1000000 * 0.002) + 50;
    }
    $scope.FlatFXSaving = function () {
        return $scope.BankCommission() - $scope.FlatFXCommission();
    }
});

// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('SimpleCurrencyExchange', function ($scope, $timeout, $interval, noty) {
    $scope.init = function (WorkflowStage, isDemo, info, error) {
        $scope.isDemo = isDemo;
        $scope.info = info;
        $scope.error = error;
        if (WorkflowStage == 2) {
            $scope.CountDown = 60;
        }
        else {
            $scope.CountDown = 0;
        }
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
        $('#confirm-reorder').hide();
    }
    $interval(function () {
        if ($scope.CountDown < 1)
            return;

        if ($scope.CountDown == 1) {
            $('#confirm-submit').attr("disabled", true);
            $('#confirm-countdown').removeClass('countdown-enabled').addClass('countdown-disabled');
            $('#confirm-reorder').fadeIn(500);
        }

        $scope.CountDown = $scope.CountDown - 1;
    }, 1000);
});


// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('AdminManager', function ($scope, $timeout, noty) {
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

myApp.controller('UserManager', function ($scope, $timeout, noty) {
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

myApp.controller('OnLineRatesViewer', function ($scope, $http, $interval, $timeout, noty, StringManipulationService) {
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

    $scope.refreshYahooDataFeed = function () {
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
    }, 60000);

    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);
});


// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('Dashboard', function ($scope, $timeout, $interval, noty) {
    $scope.init = function () {
        $scope.siteChartLabels = ["July", "August", "September", "October", "November", "December", "January"];
        $scope.siteChartData = [0, 0, 50020, 80020, 120040, 140022, 300300];
    };
    $timeout(function () { // Use it instead of javascript $(document).ready(
        $scope.ready();
    }, 0);
    $scope.ready = function () {

        var dataSite = {
            labels: ["July", "August", "September", "October", "November", "December", "January"],
            //@*@Html.Raw(Json.Encode(Model.CompanyMonthlyVolumeLabels)),*@
            datasets: [
                {
                    label: "My Second dataset",
                    fillColor: "rgba(151,187,305,0.8)",
                    strokeColor: "rgba(151,187,205,0.8)",
                    highlightFill: "rgba(151,187,205,0.75)",
                    highlightStroke: "rgba(151,187,205,1)",
                    //@*data: @Html.Raw(Json.Encode(Model.CompanyMonthlyVolumeData))*@
                    data: [230034, 400500, 500200, 800200, 1200400, 1400220, 900300]
                }
            ]
        };

        var ctx = $("#dashboardSiteChart").get(0).getContext("2d");
        var myNewChart = new Chart(ctx);
        myNewChart.Bar(dataSite, {
            scaleShowGridLines: false,
            responsive: true,
            scaleFontColor: "#FFF"
        });

        var dataCompany = {
            labels: $scope.siteChartLabels,
            datasets: [
                {
                    label: "My Second dataset",
                    fillColor: "rgba(151,187,305,0.8)",
                    strokeColor: "rgba(151,187,205,0.8)",
                    highlightFill: "rgba(151,187,205,0.75)",
                    highlightStroke: "rgba(151,187,205,1)",
                    data: $scope.siteChartData
                }
            ]
        };

        //var ctx2 = $("#dashboardCompanyChart").get(0).getContext("2d");
        //var myNewChart2 = new Chart(ctx2);
        //myNewChart2.Bar(dataCompany, {
        //    scaleShowGridLines: false,
        //    responsive: true,
        //    scaleFontColor: "#FFF"
        //});
    }
});

// -----------------------------------------------------------------------------------------------------------------------------------------------------

myApp.controller('OrderCurrencyExchange', function ($scope, $timeout, noty) {
    $scope.init = function (WorkflowStage, isDemo, info, error) {
        $scope.isDemo = isDemo;
        $scope.info = info;
        $scope.error = error;
        $scope.actionDescription = "";
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
        $scope.Symbol = $('#Symbol').val();
        $('#' + $scope.Symbol).click();

        $('#GTC').show();
        $scope.ExpiryDateModel = '';
        $("#ExpiryDate").hide();

        $('#AllAmount').show();
        $scope.minimalPartnerExecutionAmountChkModel = 0;
        $("#MinimalPartnerExecutionAmountCCY1").hide();
        
    }
    $scope.setAction = function (symbol) {
        $('#Symbol').val(symbol);
        $scope.symbol = symbol;
        $scope.actionDescription = $('input[name="BuySell"]:checked').val() + ' ' + symbol.substring(0, 3) + ' by ' + $('input[name="BuySell"]:unchecked').val() + 'ing ' + symbol.substring(3, 6);
        $scope.symbolDisplay = $scope.actionDescription;
    }
    $scope.updateAction = function () {
        $scope.actionDescription = $('input[name="BuySell"]:checked').val() + ' ' + $scope.symbol.substring(0, 3) + ' by ' + $('input[name="BuySell"]:unchecked').val() + 'ing ' + $scope.symbol.substring(3, 6);
    }
    $scope.CCY1 = function () {
        return $scope.symbol.substring(0, 3);
    }
    $scope.expiryDateCheckboxEvent = function ($event) {
        if ($event)
        {
            var today = $scope.EndOfDay();
            $scope.ExpiryDateModel = today;
            $("#ExpiryDate").show();
            $('#GTC').hide();
        }
        else
        {
            $('#GTC').show();
            $scope.ExpiryDateModel = '';
            $("#ExpiryDate").hide();
        }
    }
    $scope.minimalPartnerCheckboxEvent = function ($event) {
        if ($event) {
            $('#AllAmount').hide();
            $scope.minimalPartnerExecutionAmountChkModel = 0;
            $("#MinimalPartnerExecutionAmountCCY1").show();
        }
        else {
            $('#AllAmount').show();
            $scope.minimalPartnerExecutionAmountChkModel = 0;
            $("#MinimalPartnerExecutionAmountCCY1").hide();
        }
    }
    $scope.EndOfDay = function () {
        var end = new Date();
        end.setHours(23, 59, 59, 999);
        return end;
    }
});

