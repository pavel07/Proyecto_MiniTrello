'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers')
    .controller('OrganizationController', [
        '$scope', '$location', '$window', 'OrganizationServices', '$stateParams', function($scope, $location, $window, organizationServices, $stateParams) {

            $scope.organizations = [];

            $scope.getOrganizationsForLoggedUser = function() {

                organizationServices
                    .getOrganizationForLoggedUser()
                    .success(function(data, status, headers, config) {
                        $scope.organizations = data;
                        console.log(data);
                    })
                    .error(function(data, status, headers, config) {
                        console.log(data);
                    });
            };

            $scope.getOrganizationsForLoggedUser();

            $scope.addOrganizationModel = { Title: '', Description: '' };

            $scope.addOrganization = function() {
                organizationServices.addOrganization($scope.addOrganizationModel)
                    .success(function(data, status, headers, config){
                        if (data.Status == 0) {
                            toastr.error(data.Message, data.Title, {
                                "closeButton": true,
                                "positionClass": "toast-bottom-full-width",
                                "showEasing": "swing",
                                "hideEasing": "swing",
                                "showMethod": "slideDown",
                                "hideMethod": "fadeOut"
                            });
                        }
                        if (data.Status == 2) {
                            toastr.success(data.Message, data.Title, {
                                "positionClass": "toast-bottom-full-width",
                                "showEasing": "swing",
                                "hideEasing": "swing",
                                "showMethod": "slideDown",
                                "hideMethod": "fadeOut"
                            });
                            $scope.goToOrganization();
                        }
                    })
                    .error(function(data, status, headers, config){
                        console.log(data);
                        toastr.error(data,"", {
                            "closeButton": true,
                            "positionClass": "toast-bottom-full-width",
                            "showEasing": "swing",
                            "hideEasing": "swing",
                            "showMethod": "slideDown",
                            "hideMethod": "fadeOut"
                        });
                    });
            };

            $scope.removeOrganization = function (organizationId) {
                organizationServices.removeOrganization(organizationId)
                    .success(function (data, status, headers, config) {
                        if (data.Status == 0) {
                            toastr.error(data.Message,"", {
                                "closeButton": true,
                                "positionClass": "toast-bottom-full-width",
                                "showEasing": "swing",
                                "hideEasing": "swing",
                                "showMethod": "slideDown",
                                "hideMethod": "fadeOut"
                            });
                        }
                        if (data.Status == 2) {
                            toastr.success(data.Message, "", {
                                "positionClass": "toast-bottom-full-width",
                                "showEasing": "swing",
                                "hideEasing": "swing",
                                "showMethod": "slideDown",
                                "hideMethod": "fadeOut"
                            });
                            $scope.goToOrganization();
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

            $scope.goToOrganization = function () {
                $location.path('/organization');
                $scope.getOrganizationsForLoggedUser();

            };

        $scope.$on('$viewContentLoaded', function () {
            $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
        });
    }]);