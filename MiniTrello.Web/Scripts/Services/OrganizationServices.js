'use strict';

angular.module('app.services').factory('OrganizationServices', ['$http', '$window', function ($http, $window) {

    var organization = {};

    var baseRemoteUrl = "http://minitrelloclapi.apphb.com";
    var baseLocalUrl = "http://localhost:1416";
    var baseUrl = baseLocalUrl;

    organization.getOrganizationForLoggedUser = function() {
        return $http.get(baseUrl + '/organization/' + $window.sessionStorage.token);
    };

    return organization;

}]);