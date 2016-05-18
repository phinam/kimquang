angular.module('indexApp')
.service("productService", function ($rootScope, $http, coreService, dialogs) {
    var productService = {};
    productService.Sys_ViewID = 19;
    productService.ProductID = '';
    //    productService.dataSelected = { ProductID: productService.ProductID, Sys_ViewID: productService.Sys_ViewID };

    productService.broadcastProductData = function () {
        coreService.getListEx({ ProductID: productService.ProductID, Sys_ViewID: productService.Sys_ViewID }, function (data) {
            convertStringtoNumber(data[1], 'DistrictID');
            convertStringtoNumber(data[1], 'WardID');
            convertStringtoNumber(data[1], 'AreaPerFloor');
            convertStringtoBoolean(data[1], 'IsGroundFloor');
            convertStringtoBoolean(data[1], 'IsHiredWholeBuilding');

            productService.dataSelected = data[1][0];
            console.log('productService.dataSelected service', productService.dataSelected);
            //        $rootScope.showModal = false;
//                    $scope.$apply();
            //console.log('ProductID after', data[1]);
        });
        $rootScope.$broadcast('broadcastGetProductData');
    }

//    $scope.$watch('productId', function (newVal, oldVal) {
//        if (typeof newVal != 'undefined') {
//            productService.broadcastProductData();
//        }
//    })



    function convertStringtoNumber(array, fieldName) {
        angular.forEach(array, function (item, key) {
            if (!isNaN(item[fieldName]) && item[fieldName] != '')
                item[fieldName] = parseInt(item[fieldName]);
        });
    }
    function convertStringtoBoolean(array, fieldName) {
        angular.forEach(array, function (item, key) {
            if (item[fieldName] === "True") {
                item[fieldName] = true;
            } else {
                item[fieldName] = false;
            }

        });
    }

    function tiengvietkhongdau(str) {

        if (str == null || typeof str == 'undefined' || str == '')
            return "";

        /* str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
         str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ.+/g, "e");
         str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
         str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ.+/g, "o");
         str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
         str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
         str = str.replace(/đ/g, "d");
         */
        str = locdau(str);
        str = str.replace(/-+-/g, "-"); //thay thế 2- thành 1-
        str = str.replace(/^\-+|\-+$/g, "");
        return str;
    }
    function locdau(slug) {
        //Đổi ký tự có dấu thành không dấu
        slug = slug.replace(/á|à|ả|ạ|ã|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ/gi, "a");
        slug = slug.replace(/é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ/gi, "e");
        slug = slug.replace(/i|í|ì|ỉ|ĩ|ị/gi, "i");
        slug = slug.replace(/ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ/gi, "o");
        slug = slug.replace(/ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự/gi, "u");
        slug = slug.replace(/ý|ỳ|ỷ|ỹ|ỵ/gi, "y");
        slug = slug.replace(/đ/gi, "d");
        //Xóa các ký tự đặt biệt
        slug = slug.replace(/\`|\~|\!|\@|\#|\||\$|\%|\^|\&|\*|\(|\)|\+|\=|\,|\.|\/|\?|\>|\<|\"|\"|\:|\;|_/gi, "");
        //Đổi khoảng trắng thành ký tự gạch ngang
        slug = slug.replace(/ /gi, " ");
        //Đổi nhiều ký tự gạch ngang liên tiếp thành 1 ký tự gạch ngang
        //Phòng trường hợp người nhập vào quá nhiều ký tự trắng
        slug = slug.replace(/\-\-\-\-\-/gi, "-");
        slug = slug.replace(/\-\-\-\-/gi, "-");
        slug = slug.replace(/\-\-\-/gi, "-");
        slug = slug.replace(/\-\-/gi, "-");
        //Xóa các ký tự gạch ngang ở đầu và cuối
        slug = "@" + slug + "@";
        slug = slug.replace(/\@\-|\-\@|\@/gi, "");
        return slug;
    }


    //    productService.actionEntry = function (act) {
    //        if (typeof act != 'undefined') {
    //            var entry = angular.copy(productService.dataSelected);
    //
    //            entry.RequestObjects = requestObjects;
    //            entry.ShopsignPlacement = shopsignPlacement;
    //            entry.Action = act;
    //            entry.Sys_ViewID = productService.Sys_ViewID;
    //            coreService.actionEntry2(entry, function (data) {
    //                if (data.Success) {
    //                    switch (act) {
    //                        case 'INSERT':
    //                            if (typeof data.Entry != 'undefined')
    //                                entry = data.Entry;
    //                            productService.gridData.unshift(entry);
    //                            productService.gridInfo.instance.addRow(entry);
    //                            break;
    //                        case 'UPDATE':
    //                            if (typeof data.Entry != 'undefined')
    //                                entry = data.Entry;
    //                            //angular.forEach(productService.gridData, function (item, key) {
    //                            //    if (entry.ID == item.ID) {
    //                            //        productService.gridData[key] = angular.copy(entry);
    //
    //                            //    }
    //                            //});
    //                            productService.gridInfo.instance.updateRow(entry);
    //                            break;
    //
    //                    }
    //                    //$scope.reset();
    //                }
    //                dialogs.notify(data.Message.Name, data.Message.Description);
    //
    //            });
    //        }
    //    };

    return productService;
})