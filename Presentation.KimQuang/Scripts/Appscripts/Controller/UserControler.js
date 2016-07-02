angular.module('indexApp')
 .controller('UserCtrl', function ($scope, $rootScope, coreService, authoritiesService, alertFactory, dialogs, md5, $timeout) {
     $scope.gridInfo = {
         gridID: 'usergrid',
         table: null,
         cols: [
               { name: 'UserName', heading: 'Tên đăng nhập', width: '20%', className: 'text-center pd-0 break-word', isHidden: false },
               { name: 'FullName', heading: 'Họ tên', wdth: '20%', className: 'text-center pd-0 break-word', isHidden: false },
               { name: 'GroupName', heading: 'Nhóm thành viên', wdth: '20%', className: 'text-center pd-0 break-word', isHidden: false },
               { name: 'StatusName', heading: 'Trạng thái', width: '30%', className: 'text-center pd-0 break-word' },
               { name: 'Action1', heading: 'THAO TÁC', width: '100px', className: 'text-center pd-0 break-word', type: controls.LIST_ICON, listAction: [{ classIcon: 'fa-pencil-square-o', action: 'view' }, { classIcon: 'fa fa-times  text-danger', action: 'delete' }] }

         ],
         data: [],
         sysViewID: 7,
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
                     //$state.transitionTo('editproduct', { productId: row.ID || row });
                     // day neu em nhan vieư em data cua view , hoac neu em can update thi row la object data em dung de show len man hinh, ok ko
                     //                    alert('xem console view:' + act);
                     var userId = row.ID || row;
                     $rootScope.showModal = true;
                     coreService.getListEx({ UserID: userId, Sys_ViewID: $scope.gridInfo.sysViewID }, function (data) {
                         $scope.dataSeleted = data[1][0];
                         $rootScope.showModal = false;
                         $scope.$apply();
                     });
                     console.log('view', new Date());
                     break;
                 case 'delete':
                     var dlg = dialogs.confirm('Xác nhận', 'Xóa tài khoản');
                     dlg.result.then(function (btn) {
                         $scope.dataSeleted.ID = row.ID || row;
                         $scope.actionEntry('DELETE');
                     }, function (btn) {
                         //                        console.log('no');
                     });

                     break;

                 case 'multiSelect':
                     console.log();
                     break;




             }
         }
     }
     $scope.listRight = authoritiesService.get($scope.gridInfo.sysViewID);
     $scope.userlist = [];
     $scope.employeelist = [{ ID: 0, Name: 'Active', Code: 'Active' }, { ID: 1, Name: 'InActive', Code: 'InActive', }];
     $scope.roles = [];
     $scope.dataSeleted = { ID: "0", UserName: "", Password: "", FullName: "", Status: "0", Sys_ViewID: $scope.gridInfo.sysViewID };
     //coreService.getList(3, function (data) {
     //    $scope.employeelist = data[1];
     //});

     $scope.listGroup = [];
     coreService.getListEx({ Sys_ViewID: 30 }, function (data) {
         $scope.listGroup = data[1];
         $scope.$apply();
     });
     $scope.loadUserRoles = function (userId) {

         coreService.getListEx({ Sys_ViewID: 9, UserID: userId }, function (data) {
             $scope.roles = data[1];
             $scope.$apply();
             //console.log("user roles::", data);
         });
     }
     $scope.resetPassword = function () {


         var hashPass = md5.createHash($scope.dataSeleted.Password || '');
         var entry = { UserID: $scope.dataSeleted.ID, Password: hashPass, Action: 'UPDATE::RESETPASS' };
         var dlg = dialogs.confirm('Confirmation', 'Confirmation required');
         dlg.result.then(function (btn) {
             coreService.actionEntry2(entry, function (data) {
                 dialogs.notify(data.Message.Name, data.Message.Description, function () {

                 });



                 $scope.$apply();
             });
         }, function (btn) {
             //$scope.confirmed = 'You confirmed "No."';
         });
     }
     $scope.checkRoleAll = function (propertyName) {

         if ($scope.roles == undefined) return;
         if ($scope.roles.length == 0) return;
         var isCheckAll = true;
         for (var i = 0; i < $scope.roles.length; i++) {
             if ($scope.roles[i][propertyName] == 'False' || $scope.roles[i][propertyName] == '') {
                 isCheckAll = false;
                 break;
             }
         }
         angular.forEach($scope.roles, function (item, key) {
             item[propertyName] = isCheckAll ? 'False' : 'True';
         });
     }


     $scope.setData = function (data) {
         if (typeof data != 'undefined') {
             $scope.dataSeleted = data;
             $scope.layout.enableClear = true;
             $scope.layout.enableButtonOrther = true;
             $scope.loadUserRoles(data.ID);
         }

     }
     $scope.actionConfirm = function (act) {
         var dlg = null;
         switch (act) {
             case 'INSERT':
                 dlg = dialogs.confirm('Thêm tài khoản', 'Bạn có muốn thêm tài khoản?');
                 break;
             case 'UPDATE':
                 dlg = dialogs.confirm('Điều chỉnh tài khoản', 'Bạn có muốn diều chỉnh tài khoản đang được chọn?');
                 break;
             case 'DELETE':
                 dlg = dialogs.confirm('Xóa tài khoản', 'Bạn có muốn xóa tài khoản đang được chọn?');
                 break;
         }
         dlg.result.then(function (btn) {
             $scope.actionEntry(act);
         }, function (btn) {
             //$scope.confirmed = 'You confirmed "No."';
         });
     }
     $scope.actionEntry = function (act) {
         if (typeof act != 'undefined') {
             var entry = angular.copy($scope.dataSeleted);
             entry.Action = act;
             //  entry.Roles = {};
             //  entry.Roles.Role = $scope.roles;
             entry.Sys_ViewID = $scope.gridInfo.sysViewID;
             entry.Password = md5.createHash(entry.Password || '');
             coreService.actionEntry2(entry, function (data) {
                 if (data.Success) {
                     switch (act) {
                         case 'INSERT':
                             entry.ID = data.Result;
                             $scope.gridInfo.data.unshift(entry);
                             break;
                         case 'UPDATE':
                             angular.forEach($scope.gridInfo.data, function (item, key) {
                                 if (entry.ID == item.ID) {
                                     $scope.gridInfo.data[key] = angular.copy(entry);

                                 }
                             });
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
                             break;
                     }
                     $scope.reset();
                 }
                 dialogs.notify(data.Message.Name, data.Message.Description);
                 $scope.$apply();

             });
         }
     }

     $scope.reset = function (data) {
         $scope.dataSeleted = { ID: "0", UserName: '', Password: '', FullName: "", Status: "0", Sys_ViewID: $scope.gridInfo.sysViewID };

         if (typeof $scope.gridInfo.dtInstance == 'undefined') {
             $timeout(function () {
                 $scope.gridInfo.dtInstance.reloadData();
             }, 1000);
         } else {
             $scope.gridInfo.dtInstance.reloadData();
         }

     }
     $scope.layout = {
         enableClear: true,
         enableButtonOrther: false
     }

     $scope.init = function () {
         window.setTimeout(function () {
             $(window).trigger("resize")
         }, 200);

       //  $scope.reset(null);
     }

     $scope.changeText = function () {
         if ($scope.dataSeleted.UserName == '')
             $scope.layout.enableClear = false;
         else
             $scope.layout.enableClear = true;

         if ($scope.dataSeleted.UserName == '')
             $scope.layout.enableButtonOrther = false;
         else
             $scope.layout.enableButtonOrther = true;
     }

     
 })