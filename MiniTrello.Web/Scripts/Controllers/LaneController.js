'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers')
    .controller('LaneController', [
        '$scope', '$location', '$window', 'LaneServices', '$stateParams', function($scope, $location, $window, laneServices, $stateParams) {

            $scope.$root.title = '| MiniTrello Web';
            $scope.lanes = [];

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

            $scope.renameLaneModel = { Id: '', NewTitle: '' };

            $scope.renameLane = function (laneId) {
                $scope.renameLaneModel.Id = laneId;
                laneServices.renameLane($scope.renameLaneModel)
                    .success(function (data, status, headers, config) {
                        if (data.Status == 0) {
                            toastr.error(data.Message, "", {
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
                            toastr.success(data.Message, "", {
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
                        toastr.error(data);
                    });
            };

            $scope.deleteLaneModel = { Id: '' };

            $scope.removeLane = function (laneId) {
                $scope.deleteLaneModel.Id = laneId;
                laneServices.removeLane($scope.deleteLaneModel)
                    .success(function (data, status, headers, config) {
                        if (data.Status == 0) {
                            toastr.error(data.Message, "Board", {
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
                            toastr.success(data.Message, "", {
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
                            "debug": false,
                            "positionClass": "toast-bottom-full-width",
                            "showEasing": "swing",
                            "hideEasing": "swing",
                            "showMethod": "slideDown",
                            "hideMethod": "fadeOut"
                        });
                    });
            };

            $scope.NewCardModel = { LaneId:'', Content: '' };

            $scope.AddCard = function (laneId) {
                $scope.NewCardModel.LaneId = laneId;
                laneServices
                    .AddCard($scope.NewCardModel)
                    .success(function(data, status, headers, config) {
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
                            $scope.NewCardModel.Content = '';
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

            $scope.deleteCardModel = { CardId:'' };

            $scope.removeCard = function (cardId) {
                $scope.deleteCardModel.CardId = cardId;
                laneServices.removeCard($scope.deleteCardModel)
                    .success(function (data, status, headers, config) {
                        if (data.Status == 0) {
                            toastr.error(data.Message, "Board", {
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
                            toastr.success(data.Message, "", {
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
                            "debug": false,
                            "positionClass": "toast-bottom-full-width",
                            "showEasing": "swing",
                            "hideEasing": "swing",
                            "showMethod": "slideDown",
                            "hideMethod": "fadeOut"
                        });
                    });
            };

            $scope.goToOrganizationBoards = function(boardId){
                $location.path('/boards/');
                $scope.getBoardsForLoggedUser();
            };

            $scope.goToLanes= function () {
                $location.path('/lanes/' + $stateParams.boardId);
                $scope.getLanesForLoggedUser();

            };

            $scope.$on('$viewContentLoaded', function () {
        $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
    });
}]);