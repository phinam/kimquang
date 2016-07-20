angular.module('indexApp')
.controller('UserGroupCtrl', function ($scope, $rootScope, coreService, authoritiesService, alertFactory, dialogs, $filter, $state, $timeout, modalUtils) {
    var titleHtml = '<input type="checkbox" ng-model="vm.selectAll" ng-click="vm.toggleAll(vm.selectAll, vm.selected)">';
    $scope.gridInfo = {
        gridID: 'productgrid',
        table: null,
        cols: [
            { name: 'RowIndex', heading: 'RowIndex', isHidden: true },
            { name: 'ID', heading: 'ID', isHidden: true },
            { name: 'MultiSelect', heading: titleHtml, width: '50px', className: 'text-center pd-0 break-word', type: controls.CHECKBOX, listAction: [{ classIcon: 'form-control', action: 'multiSelect' }] },

             { name: 'Name', heading: 'Name', width: '152px', className: 'text-center pd-0 break-word' },

              { name: 'Description', heading: 'Mô tả', className: 'text-center pd-0 break-word' },
            { name: 'StatusName', heading: 'Trạng thái', className: 'text-center pd-0 break-word' },
          { name: 'Action1', heading: 'THAO TÁC', width: '100px', className: 'text-center pd-0 break-word', type: controls.LIST_ICON, listAction: [{ classIcon: 'fa-pencil-square-o', action: 'view' }, { classIcon: 'fa fa-times  text-danger', action: 'delete' }] }
        ],
        data: [],
        sysViewID: 30,
        searchQuery: '',
        onActionClick: function (row, act) {
            //            console.log(row, act);
            // row la data cua dong do
            //act la hanh dong em muon thao tac, mo cai swich ra xu ly ?? vd dum 1 cai 
            //  $scope.gridInfo.tableInstance.search(query).draw(); $scope.gridInfo.tableInstance.row( tr ).data();

            //alert('xem console view:' + act)
            $scope.tabActive = '1';
            switch (act) {
                case 'view':
                    //                    console.log('row', row);
                    //$state.transitionTo('editproduct', { productId: row.ID || row });
                    // day neu em nhan vieư em data cua view , hoac neu em can update thi row la object data em dung de show len man hinh, ok ko
                    //                    alert('xem console view:' + act);

                    var groupId = row.ID || row;
                    $rootScope.showModal = true;
                    coreService.getListEx({ ID: groupId, Sys_ViewID: $scope.gridInfo.sysViewID }, function (data) {
                        $scope.entry = data[1][0];
                        $rootScope.showModal = false;
                        $scope.$apply();
                    });
                    loadUiPermission(groupId);
                    loadDataPermission(groupId);
                    loadCapabilitiesDataPermission(groupId);
                    break;
                case 'delete':
                    if (modalUtils.modalsExist())
                        modalUtils.closeAllModals();
                    var dlg = dialogs.confirm('Xác nhận', 'xóa nhóm');
                    dlg.result.then(function (btn) {
                        $scope.entry.ID = row.ID || row;
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

    /**********************************************  BEGIN  UI PERMISSSION **********************************************************/
    function loadUiPermission(groupId) {
        if (typeof groupId == 'undefined')
            groupId = $scope.entry.ID;
        coreService.getListEx({ Sys_ViewID: 31, UserGroupID: groupId }, function (data) {
            var arr = data[1];
            //  console.log(arr)
            var roles = new Array();
            angular.forEach(arr, function (f, key) {
                var y = _.some(roles, function (c) {
                    return c.ViewName == f.ViewName;
                });
                //   console.log('yyyyy', y);
                if (!y) {
                    var list = _.where(arr, { ViewName: f.ViewName });
                    roles.push({ ViewName: f.ViewName, Action: list, ViewID: f.ID });
                }
            });
            $scope.roles = roles;
            $scope.$apply();
        });
    }

    function actionGroupView(groupId) {
        if (typeof groupId == 'undefined')
            groupId = $scope.entry.ID;
        var entry = angular.copy($scope.dataSeleted);
        var roles = angular.copy($scope.roles);
        var entry = { Action: 'INSERT', Sys_ViewID: 31, UserGroupID: groupId };
        entry.Action = 'INSERT';
        for (view in roles) {
            angular.forEach(roles[view].Action, function (item, key) {
                if (item.HasPermision == '') {
                    delete roles[view].Action[key];
                }
            });
        }
        entry.Items = roles;
        $rootScope.showModal = true;
        coreService.actionEntry2(entry, function (data) {
            $scope.reset();
            $rootScope.showModal = false;
            $scope.$apply();
        });


    }
    $scope.actionViewActionRole = function () {
        actionGroupView()
    }
    /**********************************************  END  UI PERMISSSION **********************************************************/


    /**********************************************  BEGIN  DATA DISTRICT PERMISSSION **********************************************************/

    //loadDistrict
    coreService.getListEx({ CityID: 2, Sys_ViewID: 33 }, function (data) {

        // a.unshift(34);

        $scope.districtList = data[1];

        var objUnknow = { Code: "-UNKNOW", ID: "11", Name: "KHÔNG XÁC ĐỊNH" };
        $scope.districtList.push(objUnknow);
    });


    function loadDataPermission(groupId) {

        $scope.districtPersmision.list = angular.copy($scope.districtList);
        coreService.getListEx({ UserGroupID: groupId, TableName: "app.district", Sys_ViewID: 32 }, function (data) {
            if (data[1].length == 1) {
                if (data[1][0].DataValueID == '') {
                    $scope.districtPersmision.allowAll = data[1][0].AllowAll;
                    $scope.districtPersmision.denyAll = data[1][0].DenyAll;
                    angular.forEach($scope.districtPersmision.list, function (item, key) {
                        item.checked = $scope.districtPersmision.allowAll;
                    });
                }
            }
            else {
                angular.forEach(data[1], function (item, key) {
                    var filteredGoal = _.where($scope.districtPersmision.list, { ID: item.DataValueID });
                    if (filteredGoal != null) {
                        if (item.Deny == "1")
                            filteredGoal[0].checked = '0';
                        else
                            filteredGoal[0].checked = '1';
                    }
                    // console.log('filteredGoal', filteredGoal[0]);

                });
            }
            // console.log('filteredGoal', $scope.districtPersmision.list);
        });
    }

    function actionGroupData(groupId) {
        if (typeof groupId == 'undefined')
            groupId = $scope.entry.ID;
        var district = angular.copy($scope.districtPersmision.list);
        var roles = angular.copy($scope.roles);
        var entry = { Action: 'INSERT', Sys_ViewID: 32, UserGroupID: groupId };
        entry.Items = { Table: [{ TableName: "app.district", ColumnName: "ID", AllowAll: $scope.districtPersmision.allowAll, DenyAll: $scope.districtPersmision.denyAll }] };
        // for (view in roles) {
        var list = new Array();
        angular.forEach(district, function (item, key) {
            if (item.hasOwnProperty('checked')) {
                var obj = { Value: item.ID, Allow: "1", Deny: "0" }
                if (item.checked == "0")
                    obj = { Value: item.ID, Allow: "0", Deny: "1" }
                list.push(obj);
            }
        });
        //}
        entry.Items.Table[0].Data = list;

        $rootScope.showModal = true;
        coreService.actionEntry2(entry, function (data) {
            $rootScope.showModal = false;
            dialogs.notify(data.Message.Name, data.Message.Description);
        });


    }

    $scope.actionGroupData = function (groupId) {
        actionGroupData(groupId);
    }
    /**********************************************  END  DATA DISTRICT PERMISSSION **********************************************************/


    /**********************************************  BEGIN  DATA apabilities PERMISSSION **********************************************************/
    $scope.capabilitiesPersmision = { list: [], allowAll: '0', denyAll: '0' }
    //loadDistrict
    coreService.getListEx({ Code: "LEASINGCAPABILITIES", Sys_ViewID: 16 }, function (data) {
        $scope.capabilitiesList = data[1];
        var objUnknow = { Code: "-UNKNOW", Value: "11", Name: "KHÔNG XÁC ĐỊNH" };
        $scope.capabilitiesList.push(objUnknow);
    });

    $scope.checkAllowAlCapabilities = function () {
        var checked = ''
        if ($scope.capabilitiesPersmision.allowAll == "1") {
            $scope.capabilitiesPersmision.denyAll = "0";
            checked = '1';
        }
        angular.forEach($scope.capabilitiesPersmision.list, function (item, key) {
            item.checked = checked;
        });
    }
    $scope.checkItenCapabilities = function () {
        var alowAll = "1";

        angular.forEach($scope.capabilitiesPersmision.list, function (item, key) {
            if (item.hasOwnProperty('checked')) {
                if (item.checked == "0" || item.checked == "")
                    alowAll = "0";
            }
            else
                alowAll = "0";
        });

        $scope.capabilitiesPersmision.allowAll = alowAll;
    }
    function loadCapabilitiesDataPermission(groupId) {

        $scope.capabilitiesPersmision.list = angular.copy($scope.capabilitiesList);
        coreService.getListEx({ UserGroupID: groupId, TableName: "LEASINGCAPABILITIES", Sys_ViewID: 32 }, function (data) {
            console.log('loadCapabilitiesDataPermission', data);
            if (data[1].length == 1) {
                if (data[1][0].DataValueID == '') {
                    $scope.capabilitiesPersmision.allowAll = data[1][0].AllowAll;
                    $scope.capabilitiesPersmision.denyAll = data[1][0].DenyAll;
                    angular.forEach($scope.capabilitiesPersmision.list, function (item, key) {
                        item.checked = $scope.capabilitiesPersmision.allowAll;
                    });
                }
            }
            else {
                angular.forEach(data[1], function (item, key) {
                    var filteredGoal = _.where($scope.capabilitiesPersmision.list, { Value: item.DataValueID });
                    if (filteredGoal != null) {
                        if (item.Deny == "1")
                            filteredGoal[0].checked = '0';
                        else
                            filteredGoal[0].checked = '1';
                    }
                    // console.log('filteredGoal', filteredGoal[0]);

                });
            }
            // console.log('filteredGoal', $scope.districtPersmision.list);
        });
    }

    function actionGroupataCapabilities(groupId) {
        if (typeof groupId == 'undefined')
            groupId = $scope.entry.ID;
        var district = angular.copy($scope.capabilitiesPersmision.list);
        console.log($scope.capabilitiesPersmision.list)
        var roles = angular.copy($scope.roles);
        var entry = { Action: 'INSERT', Sys_ViewID: 32, UserGroupID: groupId };
        entry.Items = { Table: [{ TableName: "LEASINGCAPABILITIES", ColumnName: "ID", AllowAll: $scope.capabilitiesPersmision.allowAll, DenyAll: $scope.capabilitiesPersmision.denyAll }] };
        // for (view in roles) {
        var list = new Array();
        angular.forEach(district, function (item, key) {
            if (item.hasOwnProperty('checked')) {
                var obj = { Value: item.Value, Allow: "1", Deny: "0" }
                if (item.checked == "0")
                    obj = { Value: item.Value, Allow: "0", Deny: "1" }
                list.push(obj);
            }
        });

      
        //}
        entry.Items.Table[0].Data = list;

        $rootScope.showModal = true;
        coreService.actionEntry2(entry, function (data) {
            $rootScope.showModal = false;
            dialogs.notify(data.Message.Name, data.Message.Description);
        });


    }

    $scope.actionGroupDataCapabilitiesD = function (groupId) {
        actionGroupDataCapabilitiesD(groupId);
    }
    /**********************************************  END  DATA apabilities PERMISSSION **********************************************************/


    /**********************************************  BEGIN  INFO GROUP**********************************************************/

    //$scope.AddNewGroup = function () {
    //    //  var entry = angular.copy($scope.dataSeleted);
    //    var entry = { Action: 'INSERT', Sys_ViewID: 31, UserGroupID: 1 };
    //    entry.Action = 'INSERT';
    //    entry.Items = $scope.roles;
    //    coreService.actionEntry2(entry, function (data) { });
    //}

    $scope.actionConfirm = function (act) {
        var dlg = null;
        switch (act) {
            case 'INSERT':
                dlg = dialogs.confirm('Thêm nhóm', 'Bạn có muốn thêm nhóm?');
                break;
            case 'UPDATE':
                dlg = dialogs.confirm('Điều chỉnh nhóm', 'Bạn có muốn diều chỉnh nhóm đang được chọn?');
                break;
            case 'DELETE':
                dlg = dialogs.confirm('Xóa tài nhóm', 'Bạn có muốn xóa nhóm đang được chọn?');
                break;
            case 'INSERTDATAPERMISION':
                dlg = dialogs.confirm('Phân quyền dữ liệu', 'Bạn có muốn cập nhật thông tin đang được chọn?');
                break;
            case 'INSERTCAPABILITIESDATAPERMISION':
                dlg = dialogs.confirm('Phân quyền dữ liệu', 'Bạn có muốn cập nhật thông tin đang được chọn?');
                break;

        }
        dlg.result.then(function (btn) {
            switch (act) {
                case 'INSERTDATAPERMISION':
                    actionGroupData(); 
                    break;
                case 'INSERTCAPABILITIESDATAPERMISION':
                    actionGroupataCapabilities();
                    break;
                default:
                    $scope.actionEntry(act);

            }
            
        }, function (btn) {
            //$scope.confirmed = 'You confirmed "No."';
        });
    }
    $scope.actionEntry = function (act) {
        if (typeof act != 'undefined') {
            var entry = angular.copy($scope.entry);
            entry.Action = act;
            entry.Sys_ViewID = $scope.gridInfo.sysViewID;

            for (var property in entry) {
                if (entry.hasOwnProperty(property)) {
                    if (entry[property] == '' || entry[property] == false || entry[property] == null) {
                        delete entry[property];
                    }
                }
            }
            $rootScope.showModal = true;
            coreService.actionEntry2(entry, function (data) {
                if (data.Success) {
                    switch (act) {
                        case 'INSERT':
                            entry.ID = data.Result;
                            $scope.gridInfo.data.unshift(entry);
                            actionGroupView(entry.ID);
                            break;
                            //  $scope.reset();
                        case 'UPDATE':
                            angular.forEach($scope.gridInfo.data, function (item, key) {
                                if (entry.ID == item.ID) {
                                    $scope.gridInfo.data[key] = angular.copy(entry);

                                }
                            });
                            actionGroupView(entry.ID);
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
                            $scope.reset();
                            break;

                    }

                    $rootScope.showModal = false;


                }
                dialogs.notify(data.Message.Name, data.Message.Description);
                $scope.$apply();

            });
        }
    }

    $scope.checkItenDistrict = function () {
        var alowAll = "1";

        angular.forEach($scope.districtPersmision.list, function (item, key) {
            if (item.hasOwnProperty('checked')) {
                if (item.checked == "0" || item.checked == "")
                    alowAll = "0";
            }
            else
                alowAll = "0";
        });

        $scope.districtPersmision.allowAll = alowAll;
    }
    $scope.reset = function (data) {
        $scope.entry = { Status: "0", ID: "0" };
        $scope.tabActive = '1';
        loadUiPermission();
        loadDataPermission();
        if (typeof $scope.gridInfo.dtInstance == 'undefined') {
            $timeout(function () {
                $scope.gridInfo.dtInstance.reloadData();
            }, 1000);
        } else {
            $scope.gridInfo.dtInstance.reloadData();
        }
    }
    /**********************************************  END  INFO GROUP **********************************************************/


    $scope.checkAllowAllDistrict = function () {
        var checked = ''
        if ($scope.districtPersmision.allowAll == "1") {
            $scope.districtPersmision.denyAll = "0";
            checked = '1';
        }
        angular.forEach($scope.districtPersmision.list, function (item, key) {
            item.checked = checked;
        });
    }
    $scope.checkDenyAllDistrict = function () {
        var checked = '1'
        if ($scope.districtPersmision.denyAll == "1") {
            $scope.districtPersmision.allowAll = "0";
            checked = '';
        }
        angular.forEach($scope.districtPersmision.list, function (item, key) {
            item.checked = checked;
        });

    }
    $scope.districtList = [];
    $scope.districtPersmision = { list: [], allowAll: '0', denyAll: '0' }
    $scope.statusOptions = [];
    $scope.entry = { Status: "0", ID: "0" };
    $scope.tabActive = '1';
    function Init() {
        loadUiPermission();
        //  loadDataPermission();
        coreService.getListEx({ Code: "STATUS", Sys_ViewID: 16 }, function (data) {
            $scope.statusOptions = data[1];
        });
    }
    Init();
});