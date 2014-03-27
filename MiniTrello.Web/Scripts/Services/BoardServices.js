'use strict';

angular.module('app.services').factory('BoardServices', ['$http', '$window', function ($http, $window) {

    var board = {};

    var baseRemoteUrl = "http://minitrelloclapi.apphb.com";
    var baseLocalUrl = "http://localhost:1416";
    var baseUrl = baseRemoteUrl;

    board.getBoardsForLoggedUser = function (organizationId) {
        return $http.get(baseUrl + '/boards/'+ organizationId+'/' + $window.sessionStorage.token);
    };

    board.addBoard = function (model) {
        return $http.post(baseUrl + '/boards/' + $window.sessionStorage.token, model);
    };

    board.removeBoard = function (model) {
        return $http.put(baseUrl + '/boards/' + $window.sessionStorage.token, model);
    };

    board.renameBoard = function (model) {
        return $http.put(baseUrl + '/boards/renameboard/' + $window.sessionStorage.token, model);
    };
    return board;

}]);