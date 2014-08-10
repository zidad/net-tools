/// <reference path="../typings/angularjs/angular-resource.d.ts" />
/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="../typings/angularjs/angular-route.d.ts" />

interface IApp extends ng.IModule { }

var app: IApp = angular.module('SampleNancyFrontEnd', [
    'ngResource',   // $resource for REST queries
    'ngAnimate',    // animations
    'ngRoute',       // routing
    'tasks'       // tasks
]);

interface IHeaderCtrlScope extends ng.IScope {
    location: any;
    isAuthenticated: boolean;
    isAdmin: boolean;
    isNavbarActive: (navBarPath: string) => boolean;
    hasPendingRequests: () => boolean;
    home: () => void;
}

interface IRunningTasks { }

app.controller('AppCtrl', [
    '$scope', 
    ($scope: any, tasks: IRunningTasks) => {

        $scope.tasks = tasks;

    }
]);

// Base route config, the sub modules will add additional routes
app.config([
    '$routeProvider', ($routeProvider: ng.route.IRouteProvider) => {
        // Default route
        $routeProvider.otherwise({ redirectTo: '/tasks/list' }); // For now go to leads, in future add dashboard view
    }
]);

// Execute bootstrapping code and any dependencies.
app.run(['$q', '$rootScope', ($q, $rootScope) => {
    $rootScope.loadingView = false;
}]);

