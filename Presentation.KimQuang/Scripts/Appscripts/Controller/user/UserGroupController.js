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
            { name: 'Status', heading: 'Trạng thái', className: 'text-center pd-0 break-word' },

{ name: 'Action1', heading: 'THAO TÁC', width: '100px', className: 'text-center pd-0 break-word', type: controls.LIST_ICON, listAction: [{ classIcon: 'fa-files-o text-primary', action: 'copy' }, { classIcon: 'fa-pencil-square-o', action: 'view' }, { classIcon: 'fa fa-times  text-danger', action: 'delete' }] }
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

    coreService.getListEx({ Sys_ViewID: 31 }, function (data) {

        var arr = data[1];
      //  console.log(arr)
        var roles = new Array();
        angular.forEach(arr, function (f, key) {
            //arrDocument.push({ Name: f.fileName, FileType: f.fileType, Default: f.default })
            //var obj = {ViewName:''};
            var y = _.some(roles, function (c) {
                return c.ViewName == f.ViewName;
            });
            //   console.log('yyyyy', y);
            if (!y) {
                var list = _.where(arr, { ViewName: f.ViewName });
                roles.push({ ViewName: f.ViewName, Action: list,ViewID:f.ID });
            }
        });
        $scope.roles = roles;
    });


    $scope.AddNewGroup = function () {
        //  var entry = angular.copy($scope.dataSeleted);
        var entry = { Action: 'INSERT', Sys_ViewID: 31, UserGroupID: 1 };
        entry.Action = 'INSERT';
        entry.Items = $scope.roles;
        coreService.actionEntry2(entry, function (data) { });
    }

    coreService.getListEx({ Sys_ViewID: 32 }, function (data) {

        var arr = data;
        console.log(arr)
        var roles = new Array();
        //angular.forEach(arr, function (f, key) {
        //    //arrDocument.push({ Name: f.fileName, FileType: f.fileType, Default: f.default })
        //    //var obj = {ViewName:''};
        //    var y = _.some(roles, function (c) {
        //        return c.ViewName == f.ViewName;
        //    });
        //    //   console.log('yyyyy', y);
        //    if (!y) {
        //        var list = _.where(arr, { ViewName: f.ViewName });
        //        roles.push({ ViewName: f.ViewName, Action: list, ViewID: f.ID });
        //    }
        //});
        //$scope.roles = roles;
    });
    $scope.districtList = [];
    coreService.getListEx({ CityID: 2, Sys_ViewID: 18 }, function (data) {
        $scope.districtList = data[1];
        console.log($scope.districtList);
    });

});