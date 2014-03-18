﻿'use strict';

angular.module('app.services').factory('BoardServices', ['$http', '$window', function ($http, $window) {

    var board = {};

    var baseRemoteUrl = "http://minitrelloclapi.apphb.com";
    var baseLocalUrl = "http://localhost:1416";
    var baseUrl = baseLocalUrl;

    board.getBoardsForLoggedUser = function () {
        return $http.get(baseUrl + '/boards/' + $window.sessionStorage.token);
    };

    return board;

}]);