'use strict';

angular.module('app.services').factory('LaneServices', ['$http', '$window', function ($http, $window) {

    var lane = {};

    var baseRemoteUrl = "http://minitrelloclapi.apphb.com";
    var baseLocalUrl = "http://localhost:1416";
    var baseUrl = baseLocalUrl;

    lane.getLanesForLoggedUser = function (boardId) {
        return $http.get(baseUrl + '/lanes/' + boardId + '/' + $window.sessionStorage.token);
    };

    lane.addLane = function (model) {
        return $http.post(baseUrl + '/lanes/' + $window.sessionStorage.token, model);
    };

    lane.removeBoard = function (model) {
        return $http.put(baseUrl + '/boards/' + $window.sessionStorage.token, model);
    };

    lane.renameBoard = function (model) {
        return $http.put(baseUrl + '/boards/renameboard/' + $window.sessionStorage.token, model);
    };
    return lane;

}]);