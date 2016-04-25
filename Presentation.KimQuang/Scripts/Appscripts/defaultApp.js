angular.module('loginApp', ['ngRoute', 'ui.bootstrap', 'ngResource','LocalStorageModule', 'angular-md5', 'app.service','ngSanitize', 'dialogs.main'])
.controller('loginController', function ($scope, $location, coreService, localStorageService, modalFactory, md5, dialogs) {
    $scope.loginData = {
        userName: "",
        password: "",
        useRefreshTokens: false
    };
     $scope.login = function () {
        var inputData = {
            UserName: $scope.loginData.userName,
            Password:md5.createHash($scope.loginData.password || '')
        }
        coreService.callServer('Core/CoreService.asmx', 'Login', inputData, function (data) {
            //vm.gridInfo.data = data[1];
            if (data != undefined && data.length > 1) {
                var response = data[1][0];
                var loginInfo = { isAuth: false };
                //console.log(data[2][0])
                if (parseInt(data[1][0].Result) > 0) {
                    data[2][0].isAuth = true;
                    localStorageService.set('authorizationData', data[2][0]);
                    window.location.href = '/app.html';
                } else {
                    localStorageService.set('authorizationData', loginInfo);
                    dialogs.error('Error', 'User Name or Password is not correct', { size: "md", animation: 'fadein' });

                }
            } else
                dialogs.error('Error', 'User Name or Password is not correct', { size: "md", animation: 'fadein' });

           
            $scope.$apply();
        });
    }


})

.factory('modalFactory', function ($http, $compile, $rootScope, $timeout, $location) {
    var modalFactory = {};
    modalFactory.modal = {
        show: function () {
            $('#modal-loading').show();
        },
        hide: function () {
            $('#modal-loading').hide();
        }
    };
    modalFactory.getBrowser = function () {
        var version = 0;
        var fn_getVersion = function () {
            var ua = navigator.userAgent, tem,
            M = ua.match(/(opera|chrome|safari|firefox|msie|trident(?=\/))\/?\s*([\d\.]+)/i) || [];
            if (/trident/i.test(M[1])) {
                tem = /\brv[ :]+(\d+(\.\d+)?)/g.exec(ua) || [];
                return 'IE ' + (tem[1] || '');
            }
            M = M[2] ? [M[1], M[2]] : [navigator.appName, navigator.appVersion, '-?'];
            if ((tem = ua.match(/version\/([\.\d]+)/i)) != null)
                M[2] = tem[1];
            return M.join(' ');
        };
        var arrVersion = fn_getVersion().split(' ');
        if (arrVersion && arrVersion.length) {
            name = arrVersion[0];
            version = arrVersion[1];
        }
        var userAgent = navigator.userAgent.toLowerCase();
        // Opera 8.0+ (UA detection to detect Blink/v8-powered Opera)
        var isOpera = !!window.opera || userAgent.indexOf(' opr/') >= 0;
        // Firefox 1.0+
        var isFirefox = typeof InstallTrigger !== 'undefined';
        // At least Safari 3+: "[object HTMLElementConstructor]"
        var isSafari = Object.prototype.toString.call(window.HTMLElement).indexOf('Constructor') > 0;
        // Chrome 1+
        var isChrome = !!window.chrome && !isOpera;
        // At least IE6
        var isIE = !!document.documentMode;

        var browser = {
            name: name,
            version: version,
            msie: isIE,
            firefox: isFirefox,
            mozilla: isFirefox,
            chrome: isChrome,
            safari: isSafari,
            opera: isOpera
        };
        return browser;
    };
    modalFactory.showAlert = function (opts) {
        console.log('$rootScope', $rootScope)
        console.log('$rootScope.$new', $rootScope.$new)
        var options = {
            id: '',
            type: 'success',
            title: 'RESULT',
            message: ''
        };
        angular.extend(options, opts);
        modalFactory.getContent({
            method: 'Get',
            url: 'templates/popup/Alert.html'
        }, function (content, status, headers, config) {
            var popupModalId = 'PopupAlert-' + options.id;
            var message = options.message;
            var title = 'RESULT';
            var type = options.type;
            var html = content.replace(/\[Id\]/gi, popupModalId).replace(/\[Type\]/gi, type).replace(/\[Title\]/gi, title).replace(/\[Messages\]/gi, message).replace(/\[OnClick\]/gi, '');
            //create DOM elements
            domElement = angular.element(html);
            var scope = $rootScope.$new();
            $compile(domElement)(scope);
            var $body = angular.element('body');
            $body.find('#' + popupModalId).remove();
            $body.append(domElement);
            angular.element('#' + popupModalId).modal('show');
        });
    };
    modalFactory.getContent = function (config, success, error) {
        $http(config).success(success).error(error);

    };
    return modalFactory;
})

.factory('authService', ['$http', '$q', 'localStorageService', function ($http, $q, localStorageService) {

    var serviceBase = ngAuthSettings.apiServiceBaseUri;
    var authServiceFactory = {};

    var _authentication = {
        isAuth: false,
        userName: "",
        useRefreshTokens: false
    };

    var _externalAuthData = {
        provider: "",
        userName: "",
        externalAccessToken: ""
    };

    var _saveRegistration = function (registration) {

        _logOut();

        return $http.post(serviceBase + 'api/account/register', registration).then(function (response) {
            return response;
        });

    };

    var _login = function (loginData) {

        var data = "grant_type=password&username=" + loginData.userName + "&password=" + loginData.password;

        if (loginData.useRefreshTokens) {
            data = data + "&client_id=" + ngAuthSettings.clientId;
        }

        var deferred = $q.defer();

        $http.post(serviceBase + 'Core/CoreService.asmx/', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

            if (loginData.useRefreshTokens) {
                localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName, refreshToken: response.refresh_token, useRefreshTokens: true });
            }
            else {
                localStorageService.set('authorizationData', { token: response.access_token, userName: loginData.userName, refreshToken: "", useRefreshTokens: false });
            }
            _authentication.isAuth = true;
            _authentication.userName = loginData.userName;
            _authentication.useRefreshTokens = loginData.useRefreshTokens;

            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    var _logOut = function () {

        localStorageService.remove('authorizationData');

        _authentication.isAuth = false;
        _authentication.userName = "";
        _authentication.useRefreshTokens = false;

    };

    var _fillAuthData = function () {

        var authData = localStorageService.get('authorizationData');
        if (authData) {
            _authentication.isAuth = true;
            _authentication.userName = authData.userName;
            _authentication.useRefreshTokens = authData.useRefreshTokens;
        }

    };

    var _refreshToken = function () {
        var deferred = $q.defer();

        var authData = localStorageService.get('authorizationData');

        if (authData) {

            if (authData.useRefreshTokens) {

                var data = "grant_type=refresh_token&refresh_token=" + authData.refreshToken + "&client_id=" + ngAuthSettings.clientId;

                localStorageService.remove('authorizationData');

                $http.post(serviceBase + 'token', data, { headers: { 'Content-Type': 'application/x-www-form-urlencoded' } }).success(function (response) {

                    localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, refreshToken: response.refresh_token, useRefreshTokens: true });

                    deferred.resolve(response);

                }).error(function (err, status) {
                    _logOut();
                    deferred.reject(err);
                });
            }
        }

        return deferred.promise;
    };

    var _obtainAccessToken = function (externalData) {

        var deferred = $q.defer();

        $http.get(serviceBase + 'api/account/ObtainLocalAccessToken', { params: { provider: externalData.provider, externalAccessToken: externalData.externalAccessToken } }).success(function (response) {

            localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, refreshToken: "", useRefreshTokens: false });

            _authentication.isAuth = true;
            _authentication.userName = response.userName;
            _authentication.useRefreshTokens = false;

            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    var _registerExternal = function (registerExternalData) {

        var deferred = $q.defer();

        $http.post(serviceBase + 'api/account/registerexternal', registerExternalData).success(function (response) {

            localStorageService.set('authorizationData', { token: response.access_token, userName: response.userName, refreshToken: "", useRefreshTokens: false });

            _authentication.isAuth = true;
            _authentication.userName = response.userName;
            _authentication.useRefreshTokens = false;

            deferred.resolve(response);

        }).error(function (err, status) {
            _logOut();
            deferred.reject(err);
        });

        return deferred.promise;

    };

    authServiceFactory.saveRegistration = _saveRegistration;
    authServiceFactory.login = _login;
    authServiceFactory.logOut = _logOut;
    authServiceFactory.fillAuthData = _fillAuthData;
    authServiceFactory.authentication = _authentication;
    authServiceFactory.refreshToken = _refreshToken;

    authServiceFactory.obtainAccessToken = _obtainAccessToken;
    authServiceFactory.externalAuthData = _externalAuthData;
    authServiceFactory.registerExternal = _registerExternal;

    return authServiceFactory;
}]);