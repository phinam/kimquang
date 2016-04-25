angular.module('indexApp')
 .controller('EmployeeCtrl', function ($scope, coreService, authoritiesService,alertFactory, dialogs) {
     $scope.gridInfo = {
         table: null,
         cols: [
               { name: 'ID', heading: 'ID', width: '0', isHidden: true },
               { name: 'Name', heading: 'Name', width: '30%' },
               { name: 'Email', heading: 'Email', width: '30%' },
               { name: 'Phone', heading: 'Phone', width: '30%' },
               { name: 'Status', heading: 'Status', width: '0', isHidden: true },
               { name: 'StatusText', heading: 'Status', width: '10%', isHidden: false }
         ],
         data: [],
         sysViewID: 3,
         searchQuery: '',
     }
     $scope.listRight = authoritiesService.get($scope.gridInfo.sysViewID);
      $scope.layout = {
          enableClear: false,
          enableButtonOrther: false
      }
     $scope.dataSeleted = { ID: 0, Name: "", Code: '', Description: "", Status: "0", Sys_ViewID: $scope.gridInfo.sysViewID };
     $scope.department = {};
     $scope.departments = [];
     $scope.position = {};
     $scope.positions = [];
     coreService.getList(1, function (data) {
         $scope.departments = data[1];
     });
     coreService.getList(4, function (data) {
         $scope.positions = data[1];
     });
     $scope.init = function () {
         window.setTimeout(function () {
             $(window).trigger("resize")
         }, 200);
     }
     $scope.setData = function (data) {
         if (typeof data != 'undefined') {
             $scope.dataSeleted = data;
             $scope.layout.enableClear = true;
             $scope.layout.enableButtonOrther = true;
         }
     }
     $scope.actionConfirm = function (act) {
         var dlg = dialogs.confirm('Confirmation', 'Confirmation required');
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
             entry.Sys_ViewID = $scope.gridInfo.sysViewID;
             coreService.actionEntry2(entry, function (data) {
                 if (data.Success){
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
         $scope.dataSeleted = { ID: 0, Name: '', Code: '', Description: "", Status: "0", Sys_ViewID: $scope.gridInfo.sysViewID };
         $scope.layout = {
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
         if ($scope.dataSeleted.Name == '' && $scope.dataSeleted.Description == '')
             $scope.layout.enableClear = false;
         else
             $scope.layout.enableClear = true;

         if ($scope.dataSeleted.Name == '')
             $scope.layout.enableButtonOrther = false;
         else
             $scope.layout.enableButtonOrther = true;

         // $scope.$apply();
     }
 })