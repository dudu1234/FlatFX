
var myApp = angular.module('flatfx', []);

myApp.controller('registerall', ['$scope', function ($scope) {
    console.log('in controller');
    $scope.greeting = 'Hola!';

    $scope.mycalc = function (x, y) {
        return x + y;
    }
}]);