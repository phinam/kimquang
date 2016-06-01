'use strict';

var app = angular.module('indexApp', ['toaster', 'ngSanitize', 'ui.router','angularFileUpload', 'ngAnimate', 'LocalStorageModule', 'ngCookies', 'ngResource', 'angularGrid', 'app.service', 'datatables', 'datatables.fixedcolumns', 'ui.bootstrap', 'dialogs.main', 'ui.select', 'minicolors', 'angular-md5', 'ui.multiselect']);
//ui.router
app.config(function ($stateProvider, $urlRouterProvider, $locationProvider) {

    //    $urlRouterProvider.otherwise('/welcome');
    $urlRouterProvider.otherwise('/welcome');

    $stateProvider
             .state('account', {
                 url: '/account',
                 templateUrl: '/Templates/view/account/account-index.html'
                 //,resolve: {
                 //    "check": function (accessFac, $location) {   //, $route, localStorageService function to be resolved, accessFac and $location Injected
                 //        if (accessFac.checkPermission()) {    //check if the user has permission -- This happens before the page loads
                 //            // return true;
                 //        } else {
                 //            window.location.href = '/index.html';			//redirect user to home if it does not have permission.
                 //        }
                 //    }
                 //}
             })
        .state('assigncustomer', {
            url: '/assign-customer',
            templateUrl: '/Templates/view/assignCustomer/assign-customer-index.html',
            controller: 'AssignCustomerCtrl'
        })
        .state('employee', {
            url: '/employee',
            templateUrl: '/Templates/view/employee/employee-index.html'
        })
        .state('department', {
            url: '/department',
            templateUrl: '/Templates/view/department/department-index.html'
        })
         .state('position', {
             url: '/position',
             templateUrl: '/Templates/view/position/position-index.html'
         })
        .state('area', {
            url: '/area',
            templateUrl: '/Templates/view/area/area-index.html'
        })
        .state('nonepermission', {
            url: '/nonepermission',
            templateUrl: '/Templates/view/nonepermission/nonepermission-index.html'
        })
         .state('project', {
             url: '/project',
             templateUrl: '/Templates/view/project/project-index.html'
         })
        .state('welcome', {
            url: '/welcome',
            templateUrl: '/Templates/view/welcome/index.html'
        })
     .state('test', {
         url: '/test',
         templateUrl: '/Templates/view/chart/baseline-index.html'
     })
        //product
        .state('productlist', {
            url: '/product-list',
            templateUrl: '/Templates/view/product/product-index.html',
            controller: 'ProductCtrl'
        })
        .state('addproduct', {
            url: '/add-product',
            templateUrl: '/Templates/view/product/add-product.html',
            controller: 'ProductCtrl'
        })
        .state('editproduct', {
            //url: '/edit-product/:productId', // not hide productId
            url: '/edit-product',
            params: { productId: null},
            templateUrl: '/Templates/view/product/add-product.html',
            controller: function ($scope, $stateParams) {
                $scope.productId = $stateParams.productId;
            }
        })
         .state('importproduct', {
             //url: '/edit-product/:productId', // not hide productId
             url: '/import-product',
             params: { productId: null },
             templateUrl: '/Templates/view/product/import-product.html',
             controller: function ($scope, $stateParams) {
                 $scope.productId = $stateParams.productId;
             }
         })
        //customer
        .state('customerlist', {
            url: '/customer-list',
            templateUrl: '/Templates/view/customer/customer-index.html',
            controller: 'CustomerCtrl'
        })
        .state('addcustomer', {
            url: '/add-customer',
            templateUrl: '/Templates/view/customer/add-customer.html',
            controller: 'CustomerCtrl'
        })
        .state('editcustomer', {
            url: '/edit-customer',
            params: { customerId: null },
            templateUrl: '/Templates/view/customer/add-customer.html',
            controller: function ($scope, $stateParams) {
                $scope.customerId = $stateParams.customerId;
                console.log('$scope.customerId', $scope.customerId);
            }
        })
        //job
        .state('joblist', {
            url: '/job-list',
            templateUrl: '/Templates/view/job/job-index.html',
            controller: 'JobCtrl'
        })
        .state('addjob', {
            url: '/add-job',
            templateUrl: '/Templates/view/job/add-job.html',
            controller: 'JobCtrl'
        })
        .state('editjob', {
            url: '/edit-job',
            params: { jobId: null },
            templateUrl: '/Templates/view/customer/add-job.html',
            controller: function ($scope, $stateParams) {
                $scope.jobId = $stateParams.jobId;
                console.log('$scope.jobId', $scope.jobId);
            }
        })

})

app.run(function ($rootScope, $location, accessFac) {
    // Register listener to watch route changes. 
    $rootScope.$on('$stateChangeStart', function (event, next, current) {

        if (accessFac.checkPermission()) {    //check if the user has permission -- This happens before the page loads
            // return true;
        } else {
            window.location.href = '/index.html';			//redirect user to home if it does not have permission.
        }
        window.setTimeout(function () {
            $(window).trigger("resize")
        }, 100);
    });

});
app.run(function ($templateCache) {
    $templateCache.put('template/chart/popover.html', [
        '<div modal-render="{{$isRendered}}" tabindex="-1" role="dialog" class="modal app-modal" modal-animation-class="fade" ng-class="{in: animate}"',
            'ng-style="{\'z-index\': 1050 + index*10, display: \'block\'}" ng-click="close($event)">',
            '<div class="modal-dialog" ng-class="size ? \'modal-\' + size : \'\'">',
                '<div class="modal-content" modal-transclude></div>',
            '</div>',
        '</div>'
    ].join(''));
})

var statusOptions = [
        {
            name: 'InActive',
            value: '1'
        },
        {
            name: 'Active',
            value: '0'
        }
];

var potentialOptions = [
        {
            name: 'Không',
            value: '1'
        },
        {
            name: 'Có',
            value: '0'
        }
];

var customerTypeOptions = [
        {
            name: 'AssignedToMe',
            value: '0'
        },
        {
            name: 'General',
            value: '1'
        }
];

var contractStatus = [
        {
            name: 'NotReContactYet',
            value: '1'
        },
        {
            name: 'Processing',
            value: '2'
        },
        {
            name: 'Finish',
            value: '3'
        }
];

var yearSelectList = [
        {
            name: '2016',
            value: '1'
        },
        {
            name: '2015',
            value: '2'
        },
        {
            name: '2013',
            value: '3'
        }
];

var monthSelectList = [
        { name: '1', value: '1'},
        { name: '2', value: '2'},
        { name: '3', value: '3'},
        { name: '4', value: '4'},
        { name: '5', value: '5'},
        { name: '6', value: '6'},
        { name: '7', value: '7'},
        { name: '8', value: '8'},
        { name: '9', value: '9'},
        { name: '10', value: '10'},
        { name: '11', value: '11'},
        { name: '12', value: '12'}
];

var weekSelectList = [
        { name: '1', value: '1' },
        { name: '2', value: '2' },
        { name: '3', value: '3' },
        { name: '4', value: '4' },
        { name: '5', value: '5' }
];

/****CONSTANT*******************/
var controls = {
    BUTTON: 'button',
    ICON_AND_TEXT: 'button&text',
    LIST_ICON: 'listicon',
    CHECKBOX: 'checkbox'
}
