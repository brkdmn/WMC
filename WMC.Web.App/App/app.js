var app = angular.module('WMCWebApp', ['ngRoute', 'LocalStorageModule']);

app.config(function($routeProvider) {
    $routeProvider.when("/home",
    {
        controller: "homeController",
        templateUrl:"/app/views/home.html"
    });

    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "/app/views/login.html"
    });

    $routeProvider.otherwise({ redirectTo: "/home" });
});

var serviceBase = 'http://localhost:56993/';
app.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);