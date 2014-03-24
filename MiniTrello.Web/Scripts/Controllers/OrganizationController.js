'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers')
    .controller('OrganizationController', ['$scope', '$location', '$window', 'OrganizationServices', '$stateParams', function ($scope, $location, $window, organizationServices, $stateParams) {

        $scope.organizations = [];
        $scope.getOrganizationsForLoggedUser = function () {

            organizationServices
                .getOrganizationForLoggedUser()
              .success(function (data, status, headers, config) {
                  $scope.organizations = data;
                    console.log(data);
                })
              .error(function (data, status, headers, config) {
                  console.log(data);
              });
        };

        $scope.getOrganizationsForLoggedUser();

        $scope.$on('$viewContentLoaded', function () {
            $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
        });
    }]);