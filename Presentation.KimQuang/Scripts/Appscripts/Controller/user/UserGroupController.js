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
             { name: 'Contact', heading: 'Liên hệ', width: '152px', className: 'text-center pd-0 break-word' },
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
});