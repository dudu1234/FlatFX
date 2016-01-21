var myApp = angular.module('FlatFXAPP', ['notyModule']);

myApp.service('StringManipulationService', function () {
    this.myFunction = function (str) {
        return 'ZZZZ';
    };
});

//myApp.config('$locationProvider', function ($locationProvider) {
//    $locationProvider.html5Mode(true);
//});