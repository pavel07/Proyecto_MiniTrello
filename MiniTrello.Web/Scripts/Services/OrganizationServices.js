'use strict';

angular.module('app.services').factory('OrganizationServices', ['$http', '$window', function ($http, $window) {

    var organization = {};

    var baseRemoteUrl = "http://minitrelloclapi.apphb.com";
    var baseLocalUrl = "http://localhost:1416";
    var baseUrl = baseLocalUrl;

    organization.getOrganizationForLoggedUser = function() {
        return $http.get(baseUrl + '/organization/' + $window.sessionStorage.token);
    };

    organization.addOrganization = function(model) {
        return $http.post(baseUrl + '/organization/' + $window.sessionStorage.token, model);
    };

    organization.removeOrganization = function (organizationId) {
        return $http.delete(baseUrl + '/organization/' + organizationId + '/'+$window.sessionStorage.token);
    };
    return organization;

}]);