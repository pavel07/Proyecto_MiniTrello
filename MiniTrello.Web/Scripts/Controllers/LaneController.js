'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers')
    .controller('LaneController', [
        '$scope', '$location', '$window', 'LaneServices', '$stateParams', function($scope, $location, $window, laneServices, $stateParams) {

            $scope.$root.title = '| MiniTrello Web';

            $scope.lanes = [];

            $scope.cards = [];

            $scope.getLanesForLoggedUser = function () {
                laneServices.getLanesForLoggedUser($stateParams.boardId)
                    .success(function (data, status, headers, config) {
                        $scope.lanes = data;
                        console.log(data);

                    })
                    .error(function (data, status, headers, config) {
                        console.log(data);
                    });
            };

            $scope.getLanesForLoggedUser();

            $scope.$on('$viewContentLoaded', function () {
        $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
    });
}]);