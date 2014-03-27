'use strict';

angular.module('app.services').factory('LaneServices', ['$http', '$window', function ($http, $window) {

    var lane = {};

    var baseRemoteUrl = "http://minitrelloclapi.apphb.com";
    var baseLocalUrl = "http://localhost:1416";
    var baseUrl = baseRemoteUrl;

    lane.getLanesForLoggedUser = function (boardId) {
        return $http.get(baseUrl + '/lanes/' + boardId + '/' + $window.sessionStorage.token);
    };

    lane.addLane = function (model) {
        return $http.post(baseUrl + '/lanes/' + $window.sessionStorage.token, model);
    };

    lane.removeLane= function (model) {
        return $http.put(baseUrl + '/lanes/removelane/' + $window.sessionStorage.token, model);
    };

    lane.renameLane = function (model) {
        return $http.put(baseUrl + '/lanes/renamelane/' + $window.sessionStorage.token, model);
    };

    lane.AddCard = function (model) {
        return $http.post(baseUrl + '/lanes/addcard/'+ $window.sessionStorage.token, model);
    };

    lane.removeCard = function (model) {
        return $http.put(baseUrl + '/lanes/removecard/'+ $window.sessionStorage.token, model);
    };
    return lane;

}]);