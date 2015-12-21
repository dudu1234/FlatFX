
myApp.controller('registerall', ['$scope', function ($scope) {
    console.log('in controller');
    $scope.greeting = 'Hola!';

    $scope.mycalc = function (x, y) {
        return x + y;
    }
}]);


myApp.controller('enterdata', function ($scope, noty) {
    $scope.init = function (isDemo) {
        $scope.isDemo = isDemo;
        if (isDemo == 'True')
            notyWrapper.generateAlertPlusButton($('#ddddd'), 'warning', 'bla bla bla');
        //.noty({ text: 'tsts', theme: 'defaultTheme', type: 'alert', template: '<div style="backgroud:blue" class="noty_message"><span class="noty_text"></span><div class="noty_close"></div></div>' });
    }
    $scope.showNotification = function () {
        noty.show('Test message', "success")
    };
    $scope.close = function () {
        noty.closeAll()
    }
});

myApp.controller('adminManager', function ($scope, noty) {
    $scope.init = function (info1, error1) {
        if (info1 != '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'success', info1);
        }
        else if (error1 != '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'error', error1);
        }
    }
});

myApp.controller('userManager', function ($scope, noty) {
    $scope.init = function (info1, error1) {
        if (info1 != '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'success', info1);
        }
        else if (error1 != '') {
            notyWrapper.generateResultMessage($('#resultDiv'), 'error', error1);
        }
    }
});
