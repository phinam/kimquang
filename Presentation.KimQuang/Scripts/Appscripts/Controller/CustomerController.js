angular.module('indexApp')
.controller('CustomerCtrl', function ($scope, $rootScope, coreService, FileUploader, authoritiesService, alertFactory, dialogs, $filter, $state, $timeout, modalUtils, localStorageService) {
    $rootScope.showModal = false;
    $scope.isNewHeader = true;
    //phu viet cho nay
    $scope.customerGroup = [];
    $scope.listCareNote = [{ id:0, name: '', text: '', date: '' }];
    $scope.listCustomerContact = [{ id: 0, name: '', text: '', date: '' }];
    $scope.listStrategyCare = [{ id: 0, name: '', text: '', date: '' }];

    $scope.leasingAreaGroup = [];
    $scope.officeRankingSelectList = [];
    $scope.floorList = [];
    $scope.leasingCapabilitiesList = [];
    $scope.buildingDirectionIDSelectList = [];
    $scope.dataSelected = { ID: 0 };
    $scope.areaGroupIDSelectList = [];
    $scope.searchEntry = {listleasingAreaGroupID:[]};
    $scope.gridInfo = {
        gridID: 'customergrid',
        table: null,
        cols: [
            { name: 'RowIndex', heading: 'RowIndex', isHidden: true },
            { name: 'ID', heading: 'ID', isHidden: true },
            { name: 'MultiSelect', heading: '', width: '50px', className: 'text-center pd-0 break-word', type: controls.CHECKBOX, listAction: [{ classIcon: 'form-control', action: 'multiSelect' }] },
            { name: 'ReceiveDate', heading: 'NGÀY NHẬN KHÁCH', width: '90px', className: 'text-center pd-0 break-word' },
             { name: 'Name', heading: 'CÔNG TY THUÊ', className: 'text-center pd-0 break-word' },
            { name: 'DemandNode', heading: 'NHU CẦU THUÊ', className: 'text-center pd-0 break-word' },
             { name: 'CareNote', heading: 'QUÁ TRÌNH CHĂM SÓC', className: 'text-center pd-0 break-word' },
             { name: 'Feedback', heading: 'PHẢN HỒI KHÁCH HÀNG', className: 'text-center pd-0 break-word'},
             { name: 'StrategyCare', heading: 'CHIẾN LƯỢC CHĂM SÓC', className: 'text-center pd-0 break-word' },
             { name: 'ContactInfo', heading: 'THÔNG TIN LIÊN HỆ', className: 'text-center pd-0 break-word' },
            { name: 'Action1', heading: 'THAO TÁC', width: '100px', className: 'text-center pd-0 break-word', type: controls.LIST_ICON, listAction: [{ classIcon: 'fa fa-eye text-primary', action: 'linkview', fieldName: "LinkProduct" }, { classIcon: 'fa-files-o text-primary', action: 'copy' }, { classIcon: 'fa-pencil-square-o', action: 'view' }, { classIcon: 'fa fa-times  text-danger', action: 'delete' }] }
        ],
        data: [],
        sysViewID: 35,
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
                    $state.transitionTo('editcustomer', { customerId: row.ID || row, action: act });
                    break;
               
                case 'copy':
                    //                    console.log('row', row);
                    $state.transitionTo('editproduct', { productId: row.ID || row, action: act });
                    break;
                case 'delete':
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





    coreService.getListEx({ Code: "CUSTOMER_STATE|CUSTOMER_LEASINGAREAGROUP|BUILDINGDIRECTION|CUSTOMER_STATE", Sys_ViewID: 16 }, function (data) {
        $scope.customerGroup = data[1];
        $scope.leasingAreaGroup = data[2];
        $scope.buildingDirectionIDSelectList = data[3];
        $scope.areaGroupIDSelectList = data[4];
    });
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

    $scope.init = function () {
        window.setTimeout(function () {
            $(window).trigger("resize")
        }, 200);
    }

    var uploader = $scope.uploader = new FileUploader({
        url: 'DocumentFile.aspx'
    });

    // FILTERS
    uploader.filters.push({
        name: 'customFilter',
        fn: function (item /*{File|FileLikeObject}*/, options) {
            return this.queue.length < 10;
        }
    });

    uploader.onCompleteItem = function (fileItem, response, status, headers) {
        //var fileType = "";
        //for (var i = 0; i < $scope.FileTypeSelectList.length; i++)
        //    if ($scope.FileTypeSelectList[i].Value == $scope.dataSelected.FileType)
        //        fileType = $scope.FileTypeSelectList[i].Name;
        //var item = { fileName: fileItem.file.name, fileType: fileType };
        //$scope.arrFiles.unshift(item);
    };
    uploader.onAfterAddingAll = function (addedFileItems) {
        // console.info('onAfterAddingAll', addedFileItems);
        $rootScope.showModal = true;
        uploader.uploadAll();
    };
    uploader.onSuccessItem = function (fileItem, response, status, headers) {
        var fileType = "";
        for (var i = 0; i < $scope.FileTypeSelectList.length; i++)
            if ($scope.FileTypeSelectList[i].Value == $scope.dataSelected.FileType)
                fileType = $scope.FileTypeSelectList[i].Name;

        var item = { name: fileItem._file.name, fileType: fileType, fileName: response };
        $scope.arrFiles.unshift(item);

    };
    uploader.onErrorItem = function (fileItem, response, status, headers) {
        console.log('onErrorItem', fileItem, response, status, headers);
    };

    uploader.onCompleteAll = function () {
        $rootScope.showModal = false;
        console.log('onCompleteAll');
    };
    $scope.deleteFile = function (index, file) {
        var dlg = dialogs.confirm('Xác nhận xóa file', 'Bạn muốn xóa file ' + file.fileName);
        dlg.result.then(function (btn) {
            $scope.arrFiles.splice(index, 1);
        }, function (btn) {
            //                        console.log('no');
        });


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

    $scope.actionEntry = function (act) {
        if (typeof act != 'undefined') {
            var entry = angular.copy($scope.dataSelected);
            entry.UnAssignedName = tiengvietkhongdau(entry.Name); //coreService.toASCi(entry.Name);
          //  entry.UnAssignedStreet = tiengvietkhongdau(entry.StreetName); //coreService.toASCi(entry.Address);
            try {
                if (typeof entry.ReceiveDate != 'undefined')
                    if (entry.ReceiveDate != '')
                        entry.ReceiveDate = $filter('date')(entry.ReceiveDate, "yyyy-MM-dd");
                if (typeof entry.HireDate != 'undefined')
                    if (entry.HireDate != '')
                        entry.HireDate = $filter('date')(entry.HireDate, "yyyy-MM-dd");
                if (typeof entry.AvailableTo != 'undefined')
                    if (entry.HireExpireDate != '')
                        entry.HireExpireDate = $filter('date')(entry.HireExpireDate, "yyyy-MM-dd");
                if (typeof entry.YearFrom != 'undefined')
                    if (entry.YearFrom != '')
                        entry.YearFrom = $filter('date')(entry.YearFrom, "yyyy-MM-dd");
                if (typeof entry.YearTo != 'undefined')
                    if (entry.YearTo != '')
                        entry.YearTo = $filter('date')(entry.YearTo, "yyyy-MM-dd");
                if (typeof entry.LastUpdatedDateTimeFrom != 'undefined')
                    if (entry.LastUpdatedDateTimeFrom != '')
                        entry.LastUpdatedDateTimeFrom = $filter('date')(entry.LastUpdatedDateTimeFrom, "yyyy-MM-dd");
                if (typeof entry.LastUpdatedDateTimeTo != 'undefined')
                    if (entry.LastUpdatedDateTimeTo != '')
                        entry.LastUpdatedDateTimeTo = $filter('date')(entry.LastUpdatedDateTimeTo, "yyyy-MM-dd");

            }
            catch (ex) {

            }


            entry.Action = act;
            entry.Sys_ViewID = 34; //$scope.gridInfo.sysViewID;

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
                            dialogs.notify(data.Message.Name, data.Message.Description);
                            $scope.gridInfo.data.unshift(entry);
                            $scope.UpdateProductDocumment(entry.ID);
                            break;
                        case 'UPDATE':
                            angular.forEach($scope.gridInfo.data, function (item, key) {
                                if (entry.ID == item.ID) {
                                    $scope.gridInfo.data[key] = angular.copy(entry);
                                }
                            });

                            $scope.UpdateProductDocumment(entry.ID);

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

    $scope.search = function (searchEntry) {
        $rootScope.showModal = true;


        //var searchEntry = angular.copy(entry);
        //console.log(searchEntry);


        searchEntry.UnAssignedName = tiengvietkhongdau(searchEntry.Name);
        searchEntry.UnBusinessArea = tiengvietkhongdau(searchEntry.BusinessArea); //coreService.toASCi(entry.Address);
        searchEntry.BuildingDirectionID = '';
        if (searchEntry.listleasingAreaGroupID.length > 0) {
            var listBD = new Array();

            for (var i = 0; i < searchEntry.listleasingAreaGroupID.length; i++) {
                listBD.push(searchEntry.listleasingAreaGroupID[i].Value);
            }
            searchEntry.LeasingAreaGroupID = listBD.toString();
        }

        //searchEntry.OfficeDirectionID = '';

        //if ($scope.listOfficeDirectionID.length > 0) {
        //    var listOBD = new Array();
        //    for (var i = 0; i < $scope.listOfficeDirectionID.length; i++) {
        //        listOBD.push($scope.listOfficeDirectionID[i].Value);
        //    }

        //    searchEntry.OfficeDirectionID = listOBD.toString();
        //}
        //searchEntry.OfficeRankingID = '';
        //if ($scope.listOfficeRankingID.length > 0) {
        //    var listOF = new Array();
        //    for (var i = 0; i < $scope.listOfficeRankingID.length; i++) {
        //        listOF.push($scope.listOfficeRankingID[i].Value);
        //    }
        //    searchEntry.OfficeRankingID = listOF.toString();
        //}

        //searchEntry.OtherFeeTypeID = '';
        //if ($scope.listOtherFeeType.length > 0) {
        //    var listOFT = new Array();
        //    for (var i = 0; i < $scope.listOtherFeeType.length; i++) {
        //        listOFT.push($scope.listOtherFeeType[i].Value);
        //    }
        //    searchEntry.OtherFeeTypeID = listOFT.toString();
        //}



        try {

            if (typeof searchEntry.ReceiveDateFrom != 'undefined')
                if (searchEntry.ReceiveDateFrom != '')
                    searchEntry.ReceiveDateFrom = $filter('date')(searchEntry.ReceiveDateFrom, "yyyy-MM-dd");
            if (typeof searchEntry.ReceiveDateTo != 'undefined')
                if (searchEntry.ReceiveDateTo != '')
                    searchEntry.ReceiveDateTo = $filter('date')(searchEntry.ReceiveDateTo, "yyyy-MM-dd");
            if (typeof searchEntry.CareDateFrom != 'undefined')
                if (searchEntry.CareDateFrom != '')
                    searchEntry.CareDateFrom = $filter('date')(searchEntry.CareDateFrom, "yyyy-MM-dd");
            if (typeof searchEntry.CareDateTo != 'undefined')
                if (searchEntry.CareDateTo != '')
                    searchEntry.CareDateTo = $filter('date')(searchEntry.CareDateTo, "yyyy-MM-dd");
            if (typeof searchEntry.DepositDateFrom != 'undefined')
                if (searchEntry.DepositDateFrom != '')
                    searchEntry.DepositDateFrom = $filter('date')(searchEntry.DepositDateFrom, "yyyy-MM-dd");
            if (typeof searchEntry.DepositDateTo != 'undefined')
                if (searchEntry.DepositDateTo != '')
                    searchEntry.DepositDateTo = $filter('date')(searchEntry.DepositDateTo, "yyyy-MM-dd");
            if (typeof searchEntry.HireDateFrom != 'undefined')
                if (searchEntry.HireDateFrom != '')
                    searchEntry.HireDateFrom = $filter('date')(searchEntry.HireDateFrom, "yyyy-MM-dd");
            if (typeof searchEntry.HireDateTo != 'undefined')
                if (searchEntry.HireDateTo != '')
                    searchEntry.HireDateTo = $filter('date')(searchEntry.HireDateTo, "yyyy-MM-dd");
            if (typeof searchEntry.ActionToDate != 'undefined')
                if (searchEntry.ActionToDate != '')
                    searchEntry.ActionToDate = $filter('date')(searchEntry.ActionToDate, "yyyy-MM-dd");

        }
        catch (ex) {

        }



        for (var property in searchEntry) {
            if (searchEntry.hasOwnProperty(property)) {
                if (searchEntry[property] == '' || searchEntry[property] == false || searchEntry[property] == null) {
                    delete searchEntry[property];
                }
            }
        }
        if (typeof searchEntry.HavePackingCarUI != 'undefined')
            searchEntry.HavePackingCar = searchEntry.HavePackingCarUI * 1 - 100;

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

    $scope.removeItemList = function (arr, index) {
        return arr.splice(index, 1);
    }
    $scope.addItemList = function (arr) {
        return arr.push({ name: '', text: '', date: '' });
    }
});