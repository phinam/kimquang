angular.module('indexApp')
.service("projectService", function ($http, coreService, dialogs) {
    var projectService = {};
    projectService.Sys_ViewID = 5;
    projectService.dataSelected = { Sys_ViewID: projectService.Sys_ViewID };
    projectService.gridData = [];
    projectService.areaList = [];
    projectService.areaManagerList = [];
   
    projectService.actionEntry = function (act) {
        if (typeof act != 'undefined') {
            var entry = angular.copy(projectService.dataSelected);
            var requestObjects = 0, shopsignPlacement=0;
            if (typeof entry.IsShopsign != 'undefined')
                requestObjects += parseInt(entry.IsShopsign);
            if (typeof entry.IsOutHouse != 'undefined')
                requestObjects += parseInt(entry.IsOutHouse);
            if (typeof entry.IsShelves != 'undefined')
                requestObjects += parseInt(entry.IsShelves);
            if (typeof entry.IsInHouse != 'undefined')
                requestObjects += parseInt(entry.IsInHouse);
            if (typeof entry.IsShopsignPoster != 'undefined')
                requestObjects += parseInt(entry.IsShopsignPoster);
            if (typeof entry.IsShopsignOther != 'undefined')
                requestObjects += parseInt(entry.IsShopsignOther);
            if (typeof entry.ShopsignPlacementFront != 'undefined')
                shopsignPlacement += parseInt(entry.ShopsignPlacementFront);
            if (typeof entry.ShopsignPlacementRight != 'undefined')
                shopsignPlacement += parseInt(entry.ShopsignPlacementRight);
            if (typeof entry.ShopsignPlacementLeft != 'undefined')
                shopsignPlacement += parseInt(entry.ShopsignPlacementLeft);

            entry.RequestObjects = requestObjects;
            entry.ShopsignPlacement = shopsignPlacement;
            entry.Action = act;
            entry.Sys_ViewID = projectService.Sys_ViewID;
            coreService.actionEntry2(entry, function (data) {
                if (data.Success) {
                    switch (act) {
                        case 'INSERT':
                            if (typeof data.Entry != 'undefined')
                                entry = data.Entry;
                            projectService.gridData.unshift(entry);
                            projectService.gridInfo.instance.addRow(entry);
                            break;
                        case 'UPDATE':
                            if (typeof data.Entry != 'undefined')
                                entry = data.Entry;
                            //angular.forEach(projectService.gridData, function (item, key) {
                            //    if (entry.ID == item.ID) {
                            //        projectService.gridData[key] = angular.copy(entry);

                            //    }
                            //});
                            projectService.gridInfo.instance.updateRow(entry);
                            break;

                    }
                    //$scope.reset();
                }
                dialogs.notify(data.Message.Name, data.Message.Description);

            });
        }
    };

    return projectService;
})