'use strict';

angular.module('app.services').factory('OrganizationServices', ['$http', '$window', function ($http, $window) {

    var organization = {};

    var baseRemoteUrl = "http://minitrelloclapi.apphb.com";
    var baseLocalUrl = "http://localhost:1416";
    var baseUrl = baseRemoteUrl;

    organization.getOrganizationForLoggedUser = function() {
        return $http.get(baseUrl + '/organization/' + $window.sessionStorage.token);
    };

    organization.addOrganization = function(model) {
        return $http.post(baseUrl + '/organization/' + $window.sessionStorage.token, model);
    };

    organization.removeOrganization = function (model) {
        return $http.put(baseUrl + '/organization/'+$window.sessionStorage.token, model);
    };

    organization.renameOrganization = function (model) {
        return $http.put(baseUrl + '/organization/rename/' + $window.sessionStorage.token, model);
    };

    organization.updateProfile = function (model) {
        return $http.put(baseUrl + '/organization/updateprofile/' + $window.sessionStorage.token, model);
    };
    return organization;

}]);