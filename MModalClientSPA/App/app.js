//  Filename: app.js

var app = angular.module('mmRecApp', ['ngRoute'])
.config(['$routeProvider', function ($routeProvider) {
    $routeProvider
        .when('/', {
            templateUrl: 'app/views/recorder.html',
            controller: 'recorderController'
        })
        .otherwise({
            redirectTo: '/app/views/recorder.html'
        })
}])
.controller('mainController', function ($scope) {
    $scope.message = 'Main Controller';
});

