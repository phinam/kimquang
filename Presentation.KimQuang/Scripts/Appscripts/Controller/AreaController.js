angular.module('indexApp')
.controller('AreaCtrl', function ($scope, coreService, authoritiesService, alertFactory, dialogs, coreService) {
    $scope.gridInfo = {
        gridID: 'areagridid',
        table: null,
        cols: [
              { name: 'RowNumber', heading: 'RowNumber', width: '10px', isHidden: true},
               { name: 'Code', heading: 'Tên viết tắt', width: '30%', backColor: true, isSort: false },
              { name: 'Name', heading: 'Tên đầy đủ', width: '50%', isSort: false, isSort: false },
              { name: 'ManagerEmployeeName', heading: 'Người quản lý', width: '20%', isSort: false }
        ],
        data: [],
        sysViewID: 2,
        searchQuery: '',
    }
    $scope.listRight = authoritiesService.get($scope.gridInfo.sysViewID);
    $scope.parentareas = [];
    $scope.managers = [];
    coreService.getList(8, function (data) {
        $scope.parentareas = data[1];
    });
    coreService.getList(3, function (data) {
        $scope.managers = data[1];
    });
    $scope.statusOptions = statusOptions;
    $scope.layout = {
        enableClear: false,
        enableButtonOrther: false
    }
    $scope.dataSeleted = { ID: 0, Name: "", Code: "", Description: "", ManagerEmployeeID: "0", Status: "0", Sys_ViewID: $scope.gridInfo.sysViewID };
    $scope.init = function () {
        window.setTimeout(function () {
            $(window).trigger("resize")
        }, 200);

        $scope.customSettings = {
            letterCase: 'uppercase'
        };

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
            coreService.actionEntry(entry, function (data) {
                if (data[0].length > 0)
                    if (data[0][0]) {
                        coreService.getList($scope.gridInfo.sysViewID, function (data) {
                            $scope.gridInfo.data = angular.copy(data[1]);
                            $scope.$apply();

                        });

                        $scope.reset();
                        dialogs.notify(data[0][0].Name, data[0][0].Description);
                    }
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