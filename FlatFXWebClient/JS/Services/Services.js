
var myApp = angular.module('flatfx', ['notyModule']);

myApp.service('StringManipulationService', function () {
    this.myFunction = function (str) {
        return 'ZZZZ';
    };
});