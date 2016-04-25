
var app = angular.module('AngularAuthApp', ['ngRoute', 'ui.bootstrap','LocalStorageModule', 'angular-loading-bar']);
var serviceBase = 'http://nippon.vn/api/';
//var serviceBase = 'http://ngauthenticationapi.azurewebsites.net/';
app.constant('ngAuthSettings', {
    apiServiceBaseUri: serviceBase,
    clientId: 'ngAuthApp'
})


app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);


