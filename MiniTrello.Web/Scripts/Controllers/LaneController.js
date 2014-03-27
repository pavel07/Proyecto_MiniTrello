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

            $scope.addLaneModel = { BoardId: '', Title: '' };

            $scope.addLane = function () {
                $scope.addLaneModel.BoardId = $stateParams.boardId;
                laneServices.addLane($scope.addLaneModel)
                    .success(function (data, status, headers, config) {
                        if (data.Status == 0) {
                            toastr.error(data.Message, data.Title, {
                                "closeButton": true,
                                "debug": false,
                                "positionClass": "toast-bottom-right",
                                "showEasing": "swing",
                                "hideEasing": "swing",
                                "showMethod": "slideDown",
                                "hideMethod": "fadeOut"
                            });
                        }
                        if (data.Status == 2) {
                            toastr.success(data.Message, data.Title, {
                                "closeButton": true,
                                "debug": false,
                                "positionClass": "toast-bottom-right",
                                "showEasing": "swing",
                                "hideEasing": "swing",
                                "showMethod": "slideDown",
                                "hideMethod": "fadeOut"
                            });
                            $scope.goToLanes();
                        }
                    })
                    .error(function (data, status, headers, config) {
                        console.log(data);
                        toastr.error(data, "", {
                            "closeButton": true,
                            "positionClass": "toast-bottom-full-width",
                            "showEasing": "swing",
                            "hideEasing": "swing",
                            "showMethod": "slideDown",
                            "hideMethod": "fadeOut"
                        });
                    });
            };

            $scope.goToOrganizationBoards = function(){
                $location.path('/boards/' + $stateParams.boardId);
                $scope.getBoardsForLoggedUser();
            };

            $scope.goToLanes= function () {
                $location.path('/lanes/' + $stateParams.boardId);
                $scope.getBoardsForLoggedUser();

            };

            $scope.$on('$viewContentLoaded', function () {
        $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
    });
}]);