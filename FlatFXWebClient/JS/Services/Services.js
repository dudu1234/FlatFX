/// <reference path="../Functions/Charts.js" />

var myApp = angular.module('flatfx', ['notyModule', '../Functions/Charts.js']);
//var myApp = angular.module('flatfx', ['notyModule']);

myApp.service('StringManipulationService', function () {
    this.myFunction = function (str) {
        return 'ZZZZ';
    };
});

//myApp.config('$locationProvider', function ($locationProvider) {
//    $locationProvider.html5Mode(true);
//});