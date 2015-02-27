/// <reference path="../typings/angularjs/angular-resource.d.ts" />
/// <reference path="../typings/angularjs/angular.d.ts" />
/// <reference path="../typings/angularjs/angular-route.d.ts" />
var app = angular.module('SampleNancyFrontEnd', [
    'ngResource',
    'ngAnimate',
    'ngRoute',
    'tasks'
]);
app.controller('AppCtrl', [
    '$scope',
    function ($scope, tasks) {
        $scope.tasks = tasks;
    }
]);
// Base route config, the sub modules will add additional routes
app.config([
    '$routeProvider',
    function ($routeProvider) {
        // Default route
        $routeProvider.otherwise({ redirectTo: '/tasks/list' }); // For now go to leads, in future add dashboard view
    }
]);
// Execute bootstrapping code and any dependencies.
app.run(['$q', '$rootScope', function ($q, $rootScope) {
    $rootScope.loadingView = false;
}]);
//# sourceMappingURL=app.js.map