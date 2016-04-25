angular.module('indexApp')
.controller('PositionCtrl', function ($scope, coreService, authoritiesService,alertFactory, dialogs) {
    $scope.gridInfo = {
        table: null,
        cols: [
              { name: 'ID', heading: 'ID', width: '0', isHidden: true },
              { name: 'Name', heading: 'Name', width: '30%' },
              { name: 'Description', heading: 'Description', width: '70%' },
              { name: 'Status', heading: 'Status', width: '0', isHidden: true },
              { name: 'StatusText', heading: 'Status', width: '0', isHidden: true }
        ],
        data: [],
        sysViewID: 4,
        searchQuery: '',
    }
    $scope.listRight = authoritiesService.get($scope.gridInfo.sysViewID);
    $scope.statusOptions = statusOptions;
    $scope.layout = {
        enableClear: false,
        enableButtonOrther: false
    }
    $scope.dataSeleted = { ID: 0, Name: "", Code: '', Description: "", Status: "0", Sys_ViewID: $scope.gridInfo.sysViewID };
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
                    //dialogs.notify(data[0][0].Name, data[0][0].Description);
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

})