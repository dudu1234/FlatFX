var myApp = angular.module('FlatFXAPP', ['notyModule']);

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