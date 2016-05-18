angular.module('indexApp')
.service("productService", function ($rootScope, $http, coreService, dialogs) {
    var productService = {};
    productService.Sys_ViewID = 19;
    productService.ProductID = '';
    //    productService.dataSelected = { ProductID: productService.ProductID, Sys_ViewID: productService.Sys_ViewID };

    productService.broadcastProductData = function () {
        
    }

//    $scope.$watch('productId', function (newVal, oldVal) {
//        if (typeof newVal != 'undefined') {
//            productService.broadcastProductData();
//        }
//    })




    return productService;
})