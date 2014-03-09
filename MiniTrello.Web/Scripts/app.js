﻿'use strict';

// Declares how the application should be bootstrapped. See: http://docs.angularjs.org/guide/module
angular.module('app', ['ui.router', 'app.filters', 'app.services', 'app.directives', 'app.controllers'])

    // Gets executed during the provider registrations and configuration phase. Only providers and constants can be
    // injected here. This is to prevent accidental instantiation of services before they have been fully configured.
    .config(['$stateProvider', '$locationProvider', '$httpProvider', function ($stateProvider, $locationProvider, $httpProvider) {

        // UI States, URL Routing & Mapping. For more info see: https://github.com/angular-ui/ui-router
        // ------------------------------------------------------------------------------------------------------------

        $httpProvider.defaults.headers.common = { 'Content-Type': 'application/json' };
        $httpProvider.defaults.headers.post = { 'Content-Type': 'application/json' };
        $httpProvider.defaults.headers.put = { 'Content-Type': 'application/json' };
        $httpProvider.defaults.headers.patch = { 'Content-Type': 'application/json' };
        $httpProvider.defaults.headers.get = { 'Content-Type': 'application/json' };

        $stateProvider
            .state('home', {
                url: '/',
                templateUrl: '/views/index',
                controller: 'HomeController'
            })
            .state('about', {
                url: '/about',
                templateUrl: '/views/about',
                controller: 'AboutController'
            })
            .state('login', {
                url: '/login',
                layout: 'basic',
                templateUrl: '/views/login',
                controller: 'AccountController'
            })
            .state('register', {
                url: '/register',
                layout: 'basic',
                templateUrl: '/views/register',
                controller: 'AccountController'
            })
            .state('boards', {
                url: '/boards',
                templateUrl: '/views/boards',
                controller: 'BoardController'
            })
            .state('boardDetail', {
                url: '/board/:boardId',
                templateUrl: '/views/boardDetail',
                controller: 'BoardController'
            })
            .state('otherwise', {
                url: '*path',
                templateUrl: '/views/404',
                controller: 'ErrorController'
            });

        $locationProvider.html5Mode(true);

    }])

    // Gets executed after the injector is created and are used to kickstart the application. Only instances and constants
    // can be injected here. This is to prevent further system configuration during application run time.
    .run(['$templateCache', '$rootScope', '$state', '$stateParams', function ($templateCache, $rootScope, $state, $stateParams) {

        // <ui-view> contains a pre-rendered template for the current view
        // caching it will prevent a round-trip to a server at the first page load
        var view = angular.element('#ui-view');
        $templateCache.put(view.data('tmpl-url'), view.html());

        // Allows to retrieve UI Router state information from inside templates
        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;

        $rootScope.$on('$stateChangeSuccess', function (event, toState) {

            // Sets the layout name, which can be used to display different layouts (header, footer etc.)
            // based on which page the user is located
            $rootScope.layout = toState.layout;
        });
    }]);