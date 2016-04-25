angular.module('AngularAuthApp')
.controller('loginController', ['$scope', '$location', 'authService', 'ngAuthSettings', 'modalFactory', '$rootScope', function ($scope, $location, authService, ngAuthSettings, modalFactory) {
    modalFactory.showAlert({
        id: 'ClientAccess',
        type: 'danger',
        message: 'aaa'
    });
    $scope.loginData = {
        userName: "",
        password: "",
        useRefreshTokens: false
    };

    $scope.message = "";

    $scope.login = function () {
        authService.login($scope.loginData).then(function (response) {
           // console.log(response)
            //$location.path('/orders');
            window.location.href = '/app.html';

        },
         function (err) {
             console.log('err', err);
             $scope.message = err.error_description;

             modalFactory.showAlert({
                 id: 'ClientAccess',
                 type: 'danger',
                 message: err.error_description
             });
         });
    };

    $scope.authExternalProvider = function (provider) {

        var redirectUri = location.protocol + '//' + location.host + '/authcomplete.html';

        var externalProviderUrl = ngAuthSettings.apiServiceBaseUri + "api/Account/ExternalLogin?provider=" + provider
                                                                    + "&response_type=token&client_id=" + ngAuthSettings.clientId
                                                                    + "&redirect_uri=" + redirectUri;
        window.$windowScope = $scope;

        var oauthWindow = window.open(externalProviderUrl, "Authenticate Account", "location=0,status=0,width=600,height=750");
    };

    $scope.authCompletedCB = function (fragment) {

        $scope.$apply(function () {

            if (fragment.haslocalaccount == 'False') {

                authService.logOut();

                authService.externalAuthData = {
                    provider: fragment.provider,
                    userName: fragment.external_user_name,
                    externalAccessToken: fragment.external_access_token
                };

                $location.path('/associate');

            }
            else {
                //Obtain access token and redirect to orders
                var externalData = { provider: fragment.provider, externalAccessToken: fragment.external_access_token };
                authService.obtainAccessToken(externalData).then(function (response) {

                    $location.path('/orders');

                },
             function (err) {
                 $scope.message = err.error_description;
             });
            }

        });
    }
}])
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
});