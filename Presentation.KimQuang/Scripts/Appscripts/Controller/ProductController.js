angular.module('indexApp')
.controller('ProductCtrl', function ($scope, $rootScope, coreService, authoritiesService, alertFactory, dialogs, $filter, $state, $timeout, modalUtils, productService) {
    $rootScope.showModal = false;
    var titleHtml = '<input type="checkbox" ng-model="vm.selectAll" ng-click="vm.toggleAll(vm.selectAll, vm.selected)">';
    $scope.gridInfo = {
        gridID: 'productgrid',
        table: null,
        cols: [
            { name: 'RowIndex', heading: 'RowIndex', isHidden: true },
            { name: 'ID', heading: 'ID', isHidden: true },
            { name: 'MultiSelect', heading: titleHtml, width: '50px', className: 'text-center pd-0 break-word', type: controls.CHECKBOX, listAction: [{ classIcon: 'form-control', action: 'multiSelect' }] },
            { name: 'LastUpdatedDateTime', heading: 'Ngày cập nhật', width: '90px', className: 'text-center pd-0 break-word' },
             { name: 'PriorityLevel', heading: 'UT', className: 'text-center pd-0 break-word' },
            { name: 'Name', heading: 'Tên', className: 'text-center pd-0 break-word' },
             { name: 'AreaDescription', heading: 'Diện tích trống', className: 'text-center pd-0 break-word' },
             { name: 'PriceDescription', heading: 'Lưu ý giá', className: 'text-center pd-0 break-word' },
             { name: 'HirePrice', heading: 'Giá thuê', className: 'text-center pd-0 break-word' },
             { name: 'HireManagermentFee', heading: 'PQL', className: 'text-center pd-0 break-word' },
             { name: 'TotalPrice', heading: 'Giá tổng(M2)', className: 'text-center pd-0 break-word' },
             { name: 'HireTotalAmount', heading: 'Giá tổng(DT)', className: 'text-center pd-0 break-word' },
             { name: 'HireFinalPrice', heading: 'Giá chốt', className: 'text-center pd-0 break-word' },
             { name: 'BasicInfo', heading: 'Thông tin cơ bản', width: '250px', className: 'text-center pd-0 break-word' },
             { name: 'Contact', heading: 'Liên hệ', width: '152px',className: 'text-center pd-0 break-word' },
            { name: 'HomeNumber', heading: 'Số nhà', className: 'text-center pd-0 break-word' },
            { name: 'StreetName', heading: 'Đường', className: 'text-center pd-0 break-word' },
              { name: 'WardName', heading: 'Phường', className: 'text-center pd-0 break-word' },
            { name: 'DistrictName', heading: 'Quận', className: 'text-center pd-0 break-word' },
          
        
            { name: 'Action1', heading: 'Sửa', width: '50px', className: 'text-center pd-0 break-word', type: controls.LIST_ICON, listAction: [{ classIcon: 'fa-pencil-square-o', action: 'view' }] },
            { name: 'Action2', heading: 'Xóa', width: '50px', className: 'text-center pd-0 break-word', type: controls.LIST_ICON, listAction: [{ classIcon: 'fa-times  text-danger', action: 'delete' }] }
        ],
        data: [],
        sysViewID: 20,
        searchQuery: '',
        onActionClick: function (row, act) {
            //            console.log(row, act);
            // row la data cua dong do
            //act la hanh dong em muon thao tac, mo cai swich ra xu ly ?? vd dum 1 cai 
            //  $scope.gridInfo.tableInstance.search(query).draw(); $scope.gridInfo.tableInstance.row( tr ).data();

            //alert('xem console view:' + act)
            switch (act) {
                case 'view':
                    //                    console.log('row', row);
                    $state.transitionTo('editproduct', { productId: row.ID || row });
                    // day neu em nhan vieư em data cua view , hoac neu em can update thi row la object data em dung de show len man hinh, ok ko
                    //                    alert('xem console view:' + act);
                    //coreService.getListEx({ ProductID: row.ID, Sys_ViewID: 19 }, function (data) {
                    //    console.log('ProductID', data)
                    //});
                    break;
                case 'delete':
                    console.log('row', row);
                    if (modalUtils.modalsExist())
                        modalUtils.closeAllModals();
                    var dlg = dialogs.confirm('Confirmation', 'Confirmation required');
                    dlg.result.then(function (btn) {
                        $scope.deleteId = row.ID || row;
                        $scope.actionEntry('DELETE');
                    }, function (btn) {
                        //                        console.log('no');
                    });


                    break;

                case 'multiSelect':
                    console.log();
                    break;

                case 'chart':
                    $scope.openDialogChart(rowID);


                    break;


            }
        }
    }

    $scope.listRight = authoritiesService.get($scope.gridInfo.sysViewID);
    $scope.statusOptions = statusOptions;
    $scope.Layout = {
        enableClear: false,
        enableButtonOrther: false
    }
    $scope.dataSelected = { ID: 0, Name: "", Code: '', Description: "", Status: "0", Sys_ViewID: $scope.gridInfo.sysViewID, layout: '', brokeragecontract: '', image1: '', bidcontracts: '', proposal: '', ortherpapers: '' };
    $scope.init = function () {
        window.setTimeout(function () {
            $(window).trigger("resize")
        }, 200);
    }
    $scope.setData = function (data) {
        if (typeof data != 'undefined') {
            $scope.dataSelected = data;
            $scope.Layout.enableClear = true;
            $scope.Layout.enableButtonOrther = true;
        }
    }
    $scope.actionConfirm = function (act) {
        $scope.actionEntry(act);
        //var dlg = dialogs.confirm('Confirmation', 'Confirmation required');
        //dlg.result.then(function (btn) {
        //    $scope.actionEntry(act);
        //}, function (btn) {
        //    //$scope.confirmed = 'You confirmed "No."';
        //});
    }

    $scope.reset = function (data) {
        $scope.dataSelected = { ID: 0, Name: '', Code: '', Description: "", Status: "0", Sys_ViewID: $scope.gridInfo.sysViewID };
        $scope.Layout = {
            enableClear: false,
            enableButtonOrther: false
        }
        // $scope.$apply();
    }

    $scope.searchTable = function () {
        var query = $scope.gridInfo.searchQuery;
        $scope.gridInfo.tableInstance.search(query).draw();
    };
    $scope.changeText = function () {
        if ($scope.dataSelected.Name == '')
            $scope.Layout.enableClear = false;
        else
            $scope.Layout.enableClear = true;

        if ($scope.dataSelected.Name == '')
            $scope.Layout.enableButtonOrther = false;
        else
            $scope.Layout.enableButtonOrther = true;

        // $scope.$apply();
    }

    $scope.launch = function (which) {
        switch (which) {
            case 'error':
                dialogs.error();
                break;
            case 'wait':
                var dlg = dialogs.wait(undefined, undefined, _progress);
                _fakeWaitProgress();
                break;
            case 'customwait':
                var dlg = dialogs.wait('Custom Wait Header', 'Custom Wait Message', _progress);
                _fakeWaitProgress();
                break;
            case 'notify':
                dialogs.notify();
                break;
            case 'confirm':
                var dlg = dialogs.confirm();
                dlg.result.then(function (btn) {
                    $scope.confirmed = 'You confirmed "Yes."';
                }, function (btn) {
                    $scope.confirmed = 'You confirmed "No."';
                });
                break;
            case 'custom':
                var dlg = dialogs.create('/dialogs/custom.html', 'customDialogCtrl', {}, { size: 'lg', keyboard: true, backdrop: false, windowClass: 'my-class' });
                dlg.result.then(function (name) {
                    $scope.name = name;
                }, function () {
                    if (angular.equals($scope.name, ''))
                        $scope.name = 'You did not enter in your name!';
                });
                break;
            case 'custom2':
                var dlg = dialogs.create('/dialogs/custom2.html', 'customDialogCtrl2', $scope.custom, { size: 'lg' });
                break;
        }
    }// end launch
    // $scope.launch('error');

    //phu viet cho nay
    coreService.getListEx({ Code: "BUILDINGDIRECTION", Sys_ViewID: 17 }, function (data) {
        //        console.log('BUILDINGDIRECTION', data);
        $scope.buildingDirectionIDSelectList = data[1];
    });

    coreService.getListEx({ Code: "FILETYPEUPLOAD", Sys_ViewID: 17 }, function (data) {
        //        console.log('FILETYPEUPLOAD', data);
        $scope.FileTypeSelectList = data[1];
    });


    //phu viet cho nay
    $scope.districtSelectList = null;
    $scope.wardSelectList = null;
    coreService.getListEx({ CityID: 2, Sys_ViewID: 18 }, function (data) {
        //console.log('District--ward', data);
        $scope.districtSelectList = data[1];
        $scope.wardSelectList = data[2];

        convertStringtoNumber($scope.districtSelectList, 'ID');
        convertStringtoNumber($scope.wardSelectList, 'DistrictID');
        convertStringtoNumber($scope.wardSelectList, 'ID');

        $scope.tempWardSelectList = angular.copy($scope.wardSelectList);
    });

    $scope.openDialog = function (act) {
        var dlg = dialogs.create('/templates/view/product/document-popup.html', 'productDialogCtrl', productService, { size: 'lg', keyboard: false, backdrop: false });
        dlg.result.then(function (refreshList) {
            //            console.log('dialogs', refreshList);
            if (refreshList) {
                if (typeof $scope.gridInfo.dtInstance == 'undefined') {
                    $timeout(function () {
                        $scope.gridInfo.dtInstance.reloadData();
                    }, 1000);
                } else {
                    $scope.gridInfo.dtInstance.reloadData();
                }
            }

        }, function () {
            if (angular.equals($scope.name, ''))
                $scope.name = 'You did not enter in your name!';
        });
    }
    $scope.$watch('productId', function (newVal, oldVal) {
        if (typeof newVal != 'undefined') {
            $rootScope.showModal = true;
            coreService.getListEx({ ProductID: $scope.productId, Sys_ViewID: 20 }, function (data) {
                console.log('ProductID', data);
                convertStringtoNumber(data[1], 'DistrictID');
                convertStringtoNumber(data[1], 'WardID');
                convertStringtoNumber(data[1], 'AreaPerFloor');
                convertStringtoBoolean(data[1], 'IsGroundFloor');
                convertStringtoBoolean(data[1], 'IsHiredWholeBuilding');

                $scope.dataSelected = data[1][0];
                $rootScope.showModal = false;
                $scope.$apply();
                //console.log('ProductID after', data[1]);
            });
        }
    })

    //var entry = { Name: 'thanh', WardID: 1, DistrictID: 1, Address: '537/7A Đường Tân Chánh Hiệp. P. Tân Chánh Hiệp. Q.12. TPHCM.' };
    //entry.Action = 'INSERT';
    //entry.Sys_ViewID = 19;
    //coreService.actionEntry2(entry, function (data) {
    //    console.log('InsertdataProduct', data)
    //});
    $scope.actionEntry = function (act) {
        if (typeof act != 'undefined') {
            var entry = angular.copy($scope.dataSelected);
            entry.UnAssignedName = tiengvietkhongdau(entry.Name); //coreService.toASCi(entry.Name);
            entry.UnAssignedStreet = tiengvietkhongdau(entry.StreetName); //coreService.toASCi(entry.Address);
            if (typeof entry.AvailableFrom!='undefined')
            if (entry.AvailableFrom!='')
                entry.AvailableFrom=  $filter('date')(entry.AvailableFrom, "yyyy-MM-dd");


            entry.Action = act;
            entry.Sys_ViewID = 19; //$scope.gridInfo.sysViewID;

            //console.log('entry', entry);
            for (var property in entry) {
                if (entry.hasOwnProperty(property)) {
                    if (entry[property] == '') {
                        delete entry[property];
                    }
                }
            }
            if (act == 'DELETE')
                entry.ID = $scope.deleteId;

            coreService.actionEntry2(entry, function (data) {
                if (data.Success) {
                    switch (act) {
                        case 'INSERT':
                            entry.ID = data.Result;
                            $scope.gridInfo.data.unshift(entry);
                            dialogs.notify(data.Message.Name, data.Message.Description);
                            break;
                        case 'UPDATE':
                            angular.forEach($scope.gridInfo.data, function (item, key) {
                                if (entry.ID == item.ID) {
                                    $scope.gridInfo.data[key] = angular.copy(entry);
                                }
                            });
                            $state.go('productlist', '', { reload: true });
                            break;
                        case 'DELETE':
                            var index = -1;
                            var i = 0;
                            angular.forEach($scope.gridInfo.data, function (item, key) {
                                if (entry.ID == item.ID)
                                    index = i;
                                i++;
                            });
                            if (index > -1)
                                $scope.gridInfo.data.splice(index, 1);

                            dialogs.notify(data.Message.Name, data.Message.Description);

                            if (typeof $scope.gridInfo.dtInstance == 'undefined') {
                                $timeout(function () {
                                    $scope.gridInfo.dtInstance.reloadData();
                                }, 1000);
                            } else {
                                $scope.gridInfo.dtInstance.reloadData();
                            }
                            break;
                    }
                    $scope.reset();

                }
                //thong bao ket qua
                //dialogs.notify(data.Message.Name, data.Message.Description);
                $scope.$apply();

            });
        }
    }

    $scope.searchEntry = {
        UnAssignedName: null,
        UnAssignedAddress: null,
        PriceFrom: null,
        PriceTo: null,
        DistrictID: null,
        WardID: null,
        Address: null,
        AvailableAreaFrom: null,
        AvailableAreaTo: null,
        PriceFrom: null,
        PriceTo: null,
        IsHiredWholeBuilding: null,
        IsGroundFloor: null,
        BuildingDirectionID: null,
        Status: null,
        Sys_ViewID: 20
    };

    $scope.search = function (searchEntry) {
        $rootScope.showModal = true;

       
        searchEntry.UnAssignedName = tiengvietkhongdau(searchEntry.Name);
        searchEntry.UnAssignedStreet = tiengvietkhongdau(searchEntry.StreetName); //coreService.toASCi(entry.Address);
        for (var property in searchEntry) {
            if (searchEntry.hasOwnProperty(property)) {
                if (searchEntry[property] == '' || searchEntry[property] == false || searchEntry[property] == null) {
                    delete searchEntry[property];
                }
            }
        }

        if ($rootScope.searchEntryFilter != null)
            searchEntry = $rootScope.searchEntryFilter;
        else
            $rootScope.searchEntryFilter = searchEntry;


        if (typeof $scope.gridInfo.dtInstance == 'undefined') {
            $timeout(function () {
                $scope.gridInfo.dtInstance.reloadData();
            }, 1000);
        } else {
            $scope.gridInfo.dtInstance.reloadData();
        }


       
    }

    if ($rootScope.searchEntryFilter != null && typeof $rootScope.searchEntryFilter != 'undefined' && $state.current.url == '/product-list') {
        $scope.searchEntry = $rootScope.searchEntryFilter;
      //  console.log('$scope.searchEntry', $scope.searchEntry);
        $scope.search($scope.searchEntry);

    }
   

    $scope.exportFile = function (exportType, sysViewId) {
        var selectedId = [];
        var selectedItems = $rootScope.selectedItems;
        for (var id in selectedItems) {
            if (selectedItems.hasOwnProperty(id)) {
                if (selectedItems[id]) {
                    selectedId.push(id);
                }
            }
        }
        //exportType = "Excel";//"Excel|Pdf
        //sysViewId = "26";// "26|27"
        var languageId = "129";//"Excel|Pdf
        var hiddenIframeId = "#hiddenDownloader";
        coreApp.CallFunctionFromiFrame(hiddenIframeId, "RunExport", { listId: selectedId.toString(), exportType: exportType, sysViewId: sysViewId, languageId: languageId }, function () { }, 100);
        //   thisObj._win.RunExport(_data);

        console.log('selectedId', selectedId.toString());
    };

    $scope.changeDistrict = function (districtID) {
        $scope.dataSelected.WardId = null;
        $scope.wardSelectList = $filter('filterDistrictID')($scope.tempWardSelectList, districtID);
    }

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
    $scope.files = [];
    $scope.upload = function () {
        alert($scope.files.length + " files selected ... Write your Upload Code");

    };

    $("#uploadImage1").click(function () {
        var finder = new CKFinder();
        finder.resourceType = 'Images';
        finder.startupPath = "Images:/Images/";
        finder.startupFolderExpanded = true;
        finder.selectActionFunction = function (fileUrl) {
            $scope.dataSelected.Image1 = fileUrl;
            $scope.$apply();
        };
        finder.popup();
    });
    $("#uploadLayout").click(function () {
        var finder = new CKFinder();
        //finder.resourceType = 'Office';
        //finder.startupPath = "Office:/Layout";
        finder.resourceType = 'Images';
        finder.startupPath = "Images:/Product/Thumbnail/";
        finder.startupFolderExpanded = true;
        finder.selectActionFunction = function (fileUrl) {
            $scope.dataSelected.Layout = fileUrl;
            $scope.$apply();
        };
        finder.popup();
    });
    $("#uploadbrokeragecontract").click(function () {
        var finder = new CKFinder();
        finder.resourceType = 'Office';
        finder.startupPath = "Images:/Product/Office/";
        finder.startupFolderExpanded = true;
        finder.selectActionFunction = function (fileUrl) {
            $scope.dataSelected.Brokeragecontract = fileUrl;
            $scope.$apply();
        };
        finder.popup();
    });

    $("#uploadleasescontract").click(function () {
        var finder = new CKFinder();
        finder.resourceType = 'Office';
        finder.startupPath = "Images:/Product/Office/";
        finder.startupFolderExpanded = true;
        finder.selectActionFunction = function (fileUrl) {
            $scope.dataSelected.LeasesContract = fileUrl;
            $scope.$apply();
        };
        finder.popup();
    });
    $("#uploadbidcontracts").click(function () {
        var finder = new CKFinder();
        finder.resourceType = 'Images';
        finder.startupPath = "Images:/Product/Image/";
        finder.startupFolderExpanded = true;
        finder.selectActionFunction = function (fileUrl) {
            $scope.dataSelected.BidContracts = fileUrl;
            $scope.$apply();
        };
        finder.popup();
    });
    $("#uploadproposal").click(function () {
        var finder = new CKFinder();
        finder.resourceType = 'Office';
        finder.startupPath = "Images:/Product/Image/";
        finder.startupFolderExpanded = true;
        finder.selectActionFunction = function (fileUrl) {
            $scope.dataSelected.Proposal = fileUrl;
            $scope.$apply();
        };
        finder.popup();
    });
    $("#uploadortherpapers").click(function () {
        var finder = new CKFinder();
        finder.resourceType = 'Office';
        finder.startupPath = "Images:/Product/Office/";
        finder.startupFolderExpanded = true;
        finder.selectActionFunction = function (fileUrl) {
            $scope.dataSelected.Ortherpapers = fileUrl;
            $scope.$apply();
        };
        finder.popup();
    });
    $scope.ChooseImage = function (objUpload) {

        switch (objUpload) {
            case 'image1':
                // $("#uploadImage1").click();
                $scope.openDialog();
                break;
            case 'layout':
              //  $("#uploadLayout").click();
                $scope.openDialog();
                break;
            case 'brokeragecontract':
                $("#uploadbrokeragecontract").click();
                break;
          
            case 'leasescontract':
                $("#uploadleasescontract").click();
                break;
            case 'bidcontracts':
                $("#uploadbidcontracts").click();
                break;
            case 'proposal':
                $("#uploadproposal").click();
                break;
            case 'ortherpapers':
                $("#uploadortherpapers").click();
                break;
        }
    }

 
})
.controller('productDialogCtrl', function ($scope, $rootScope, $modalInstance, productService, $timeout, coreService, dialogs, $filter) {
    $rootScope.showModal = true;
    $timeout(function () {
        console.log("$scope.dataSelected", $scope.dataSelected)
        $rootScope.showModal = false;
    }, 2000);

    $scope.title = 'Chỉnh sửa sản phẩm';
    $scope.refreshList = false;

    $scope.cancel = function () {
        $modalInstance.dismiss('Canceled');
    }; // end cancel

    $scope.save = function () {
        var act = "UPDATE";
        //console.log("save:data", $scope.dataSelected);
        //console.log("save:projectService", projectService.dataSelected);
        if ($scope.dataSelected.ID != undefined && $scope.dataSelected.ID > 0) act = "UPDATE";
        $scope.actionConfirm(act);
    }; // end save

    $scope.actionConfirm = function (act) {
        $scope.actionEntry(act);
        //var dlg = dialogs.confirm('Confirmation', 'Confirmation required');
        //dlg.result.then(function (btn) {
        //    $scope.actionEntry(act);
        //}, function (btn) {
        //    //$scope.confirmed = 'You confirmed "No."';
        //});
    }

    $scope.actionEntry = function (act) {
        if (typeof act != 'undefined') {
            var entry = angular.copy($scope.dataSelected);
            entry.UnAssignedName = tiengvietkhongdau(entry.Name); //coreService.toASCi(entry.Name);
            entry.UnAssignedAddress = tiengvietkhongdau(entry.Address); //coreService.toASCi(entry.Address);
            entry.Action = act;
            entry.Sys_ViewID = 19; //$scope.gridInfo.sysViewID;
            entry.Description && (entry.Description = entry.Description.replace(/\n\r?/g, '<br />'));
            //            console.log('entry', entry);

            for (var property in entry) {
                if (entry.hasOwnProperty(property)) {
                    if (entry[property] == '') {
                        delete entry[property];
                    }
                }
            }
            if (act == 'DELETE')
                entry.ID = $scope.deleteId;

            coreService.actionEntry2(entry, function (data) {
                if (data.Success) {
                    switch (act) {
                        case 'INSERT':
                            //                            entry.ID = data.Result;
                            //                            $scope.gridInfo.data.unshift(entry);
                            //                            dialogs.notify(data.Message.Name, data.Message.Description);
                            break;
                        case 'UPDATE':
                            $modalInstance.close($scope.refreshList);
                            break;
                        case 'DELETE':
                           
                            break;
                    }
                    //                    $scope.reset();

                }
                //thong bao ket qua
                //dialogs.notify(data.Message.Name, data.Message.Description);
                $scope.$apply();

            });
        }
    }

    
})