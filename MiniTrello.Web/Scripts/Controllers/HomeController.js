'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers',[])

    // Path: /
    .controller('HomeController', ['$scope', '$location', '$window', function ($scope, $location, $window) {
        $scope.$root.title = 'MiniTrello Web';

        $scope.goToRegister = function () {
            $location.path('/register');
        };

        $scope.goToLogin = function () {
            $location.path('/login');
        };
        $scope.$on('$viewContentLoaded', function () {
            $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
        });
    }]);