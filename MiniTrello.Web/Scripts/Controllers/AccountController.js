'use strict';

// Google Analytics Collection APIs Reference:
// https://developers.google.com/analytics/devguides/collection/analyticsjs/

angular.module('app.controllers')


// Path: /login
    .controller('AccountController', [
        '$scope', '$location', '$window', 'AccountServices', function($scope, $location, $window, AccountServices) {

            $scope.$root.title = 'MiniTrello Web | Login';
            $scope.hasError = false;
            $scope.errorMessage = '';
            $scope.isLogged = function() {
                return $window.sessionStorage.token != null;
            };

            $scope.loginModel = { Email: '', Password: '' };
            $scope.registerModel = { Email: '', Password: '', FirstName: '', LastName: '', ConfirmPassword: '' };

            // TODO: Authorize a user
            $scope.login = function() {
                AccountServices
                    .login($scope.loginModel)
                    .success(function(data, status, headers, config)
                        {
                            $window.sessionStorage.token = data.Token;
                            //$scope.loginresponsemodel = data;
                            //if (loginresponsemodel.status == 0) {
                              //  toastr.error(loginresponsemodel.Token);
                            //}
                            $location.path('/boards');
                        })
                    .error(function(data, status, headers, config)
                        {
                            delete $window.sessionStorage.token;

                            $scope.errorMesage = 'Error o Clave Incorrecta';
                            $scope.hasError = true;
                            $scope.message = 'Error: Invalid User or Password';
                        });
            //$location.path('/');
            };

            $scope.goToRegister = function() {
                $location.path('/register');
            };

            $scope.goToLogin = function() {
                $location.path('/login');
            };


            $scope.register = function() {
                AccountServices
                    .register($scope.registerModel)
                    .success(function(data, status, headers, config) {
                        console.log(data);
                        $scope.goToLogin();
                    })
                    .error(function(data, status, headers, config) {
                        console.log(data);
                    });
            };

        $scope.$on('$viewContentLoaded', function () {
            $window.ga('send', 'pageview', { 'page': $location.path(), 'title': $scope.$root.title });
        });
    }]);