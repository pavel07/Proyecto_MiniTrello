'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers')
    .controller('BoardController', [
        '$scope', '$location', '$window', 'BoardServices', '$stateParams', function($scope, $location, $window, boardServices, $stateParams) {

            $scope.$root.title = '| MiniTrello Web';

            $scope.boards = [];

            $scope.getBoardsForLoggedUser = function () {
                boardServices.getBoardsForLoggedUser($stateParams.organizationId)
                    .success(function (data, status, headers, config) {
                        $scope.boards = data;
                        console.log(data);
                        
                    })
                    .error(function (data, status, headers, config) {
                        console.log(data);
                    });
            };

            $scope.getBoardsForLoggedUser();

            $scope.renameBoardModel = { Id:'', NewTitle:'' };

            $scope.renameBoard = function (boardId) {
                $scope.renameBoardModel.Id = boardId;
                boardServices.renameBoard($scope.renameBoardModel)
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
                            $scope.goToBoards();
                        }
                    })
                    .error(function (data, status, headers, config) {
                        console.log(data);
                        toastr.error(data);
                    });
            };

            $scope.addBoardModel = { organizationId: '', Title: '' };

            $scope.addBoard = function () {
                $scope.addBoardModel.organizationId = $stateParams.organizationId;
                boardServices.addBoard($scope.addBoardModel)
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
                            $scope.goToBoards();
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

            $scope.goToBoards = function () {
                $location.path('/boards/' + $stateParams.organizationId);
                $scope.getBoardsForLoggedUser();

            };

    $scope.$on('$viewContentLoaded', function () {
        $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
    });
}]);