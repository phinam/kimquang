angular.module('indexApp')
.controller('baseLineChartCtrl', function ($scope, $modalInstance, data, coreService, $filter) {
    $scope.title = data.titlePopup;
    if (typeof data == 'undefined')
        data = { viewID: 11, ProjectID: 1 };
    $scope.data = [];
    $scope.popoverData = {};
    coreService.getViewData({ Sys_ViewID: data.viewID, ProjectID: data.projectID }, function (pData) {
        modifyData(pData[1])
    });

    function modifyData(data) {
        //
        //debugger;
        //$scope.$apply();

        /*
       CompleteDate: "2015-08-05 00:00:00.000"
       IntendEndDate: "2015-08-05 00:00:00.000"
       IntendStartDate: "2015-08-01 00:00:00.000"
       StartDate: ""
       */
        if (data.IntendEndDate != '')
            data[0].IntendEndDate = data[0].IntendEndDate.replace('00:00:00.000', '23:59:59.999');
        if (data.CompleteDate != '')
            data[0].CompleteDate = data[0].CompleteDate.replace('00:00:00.000', '23:59:59.999');

        var minDate = $filter('getMinDate')(data[0].IntendStartDate, data[0].StartDate);
        var maxDate = $filter('getMaxDate')(data[0].IntendEndDate, data[0].CompleteDate);
        var totalDate = $filter('dateDiff')(minDate, maxDate, true);
        for (var i = 0; i < data.length; i++) {
            angular.extend(data[i], getStepInfo(data[i], minDate, maxDate, totalDate));
        }
        $scope.data = data;
        console.log(data);
        $scope.$apply();
        return data;


    }

    function getStepInfo(data, minDate, maxDate, totalDate) {
        //CompleteDate or CompleteDate === null
        var space, total, width;
        if (data.IntendEndDate != '')
            data.IntendEndDate = data.IntendEndDate.replace('00:00:00.000', '23:59:59.999');
        total = $filter('dateDiff')(data.IntendStartDate, data.IntendEndDate, true);
        space = total * 100 / totalDate;
        if (space == 100) space = 0;
        if (data.StartDate == '' || data.CompleteDate == '') {

            return {
                space: space,
                width: (total * 100 / totalDate),
                group1: { width: 0 },
                group2: {
                    css: '',
                    long: total,
                    width: 100
                },
                group3: { width: 0 }
            }
        }
        if (data.IntendEndDate != '')
            data.IntendEndDate = data.IntendEndDate.replace('00:00:00.000', '23:59:59.999');
        if (data.CompleteDate != '')
            data.CompleteDate = data.CompleteDate.replace('00:00:00.000', '23:59:59.999');
        var minStartDate = $filter('getMinDate')(data.IntendStartDate, data.StartDate);
        var maxStartDate = $filter('getMaxDate')(data.IntendStartDate, data.StartDate);
        var minEndDate = $filter('getMinDate')(data.IntendEndDate, data.CompleteDate);
        var maxEndDate = $filter('getMaxDate')(data.IntendEndDate, data.CompleteDate);
        total = $filter('dateDiff')(minStartDate, maxEndDate, true);
        width = total * 100 / totalDate;
        space = $filter('dateDiff')(minDate, minStartDate) * 100 / totalDate;

        var g1 = { width: 0 }, longG1, widthG1;
        var g2 = { width: 0 }, longG2, widthG3;
        var g3 = { width: 0 }, longG3, widthG3;
        //baseline-bar-red baseline-bar-gray baseline-bar-green

        /************Block 1 T1-T2==>G1*****************************************/
        //StartDate =min and enddate< intendStartDate==>ahead of schedule
        if (minStartDate == data.StartDate && data.CompleteDate < data.IntendStartDate) {
            longG1 = $filter('dateDiff')(data.StartDate, data.CompleteDate);
            widthG1 = longG1 == 0 ? 0 : longG1 * 100 / total;
            g1 = {
                css: 'baseline-bar-green',
                long: longG1,
                width: widthG1
            }
        }
        else { //StartDate =min and intendStartDate< intendStartDate==>ahead of schedule
            if (minStartDate == data.StartDate && data.IntendStartDate == maxStartDate) {
                longG1 = $filter('dateDiff')(data.StartDate, maxStartDate);
                widthG1 = longG1 == 0 ? 0 : longG1 * 100 / total;
                g1 = {
                    css: 'baseline-bar-green',
                    long: longG1,
                    width: widthG1
                }
            }
            else {
                //IntendStartDate =min and intendStartDate< intendStartDate==>ahead of schedule
                if (minStartDate == data.IntendStartDate && data.IntendEndDate == maxStartDate) {
                    longG1 = $filter('dateDiff')(data.IntendEndDate, maxStartDate);
                    widthG1 = longG1 == 0 ? 0 : longG1 * 100 / total;
                    g1 = {
                        css: ' ',
                        long: longG1,
                        width: widthG1
                    }
                }
                else {
                    //IntendStartDate =min and intendStartDate< intendStartDate==>ahead of schedule
                    if (minStartDate == data.IntendStartDate && data.StartDate == maxStartDate && data.IntendEndDate >= data.StartDate) {
                        longG1 = $filter('dateDiff')(data.IntendStartDate, maxStartDate);
                        widthG1 = longG1 == 0 ? 0 : longG1 * 100 / total;
                        g1 = {
                            css: 'baseline-bar-red',
                            long: longG1,
                            width: widthG1
                        }
                    }
                    else {
                        longG1 = $filter('dateDiff')(data.IntendStartDate, data.IntendEndDate, true);
                        widthG1 = longG1 == 0 ? 0 : longG1 * 100 / total;
                        g1 = {
                            css: 'baseline-bar-red',
                            long: longG1,
                            width: widthG1
                        }
                    }
                }
            }

        }

        /************Block 2 T3-T2==>G2*****************************************/
        // IntendStartDate--IntendEndDate
        if (data.IntendStartDate < data.StartDate && data.CompleteDate < data.IntendEndDate) {
            longG2 = $filter('dateDiff')(data.StartDate, data.CompleteDate);
            widthG2 = longG2 == 0 ? 0 : longG2 * 100 / total;
            g2 = {
                css: 'baseline-bar-gray',
                long: longG2,
                width: widthG2
            }
        }
        else {
            // IntendStartDate--CompletedDate
            if (maxStartDate < data.IntendEndDate && minEndDate < data.CompleteDate) {
                longG2 = $filter('dateDiff')(maxStartDate, minEndDate, true);
                widthG2 = longG2 == 0 ? 0 : longG2 * 100 / total;
                g2 = {
                    css: 'baseline-bar-gray',
                    long: longG2,
                    width: widthG2
                }
            }
            else {
                // StartDate--IntendEndDate
                if (maxStartDate == data.StartDate && minEndDate < data.IntendEndDate) {
                    longG2 = $filter('dateDiff')(minEndDate, data.StartDate);
                    widthG2 = longG2 == 0 ? 0 : longG2 * 100 / total;
                    g2 = {
                        css: 'baseline-bar-gray',
                        long: longG2,
                        width: widthG2
                    }
                }
                else {
                    // StartDate--IntendEndDate
                    if (maxStartDate == data.StartDate && minEndDate < data.CompleteDate && data.IntendEndDate > data.StartDate) {
                        longG2 = $filter('dateDiff')(minEndDate, maxStartDate);
                        widthG2 = longG2 == 0 ? 0 : longG2 * 100 / total;
                        g2 = {
                            css: 'baseline-bar-gray',
                            long: longG2,
                            width: widthG2
                        }
                    }
                    else {
                        // IntendStartDate--StartDate
                        if (data.IntendEndDate < data.StartDate) {
                            longG2 = $filter('dateDiff')(data.IntendEndDate, data.StartDate);
                            widthG2 = longG2 == 0 ? 0 : longG2 * 100 / total;
                            g2 = {
                                css: 'baseline-bar-red',
                                long: longG2,
                                width: widthG2
                            }
                        }
                        else {
                            // IntendStartDate--StartDate
                            if (data.CompleteDate < data.IntendStartDate) {
                                longG2 = $filter('dateDiff')(data.CompleteDate, data.IntendStartDate);
                                widthG2 = longG2 == 0 ? 0 : longG2 * 100 / total;
                                g2 = {
                                    css: 'baseline-bar-red',
                                    long: longG2,
                                    width: widthG2
                                }
                            }
                        }
                    }
                }
            }
        }

        /************Block 3 T3-T4==>G3*****************************************/
        //IntendEndDate<StartDate
        if (data.IntendEndDate < data.StartDate) {
            longG3 = $filter('dateDiff')(data.StartDate, data.CompleteDate, true);
            widthG3 = longG3 == 0 ? 0 : longG3 * 100 / total;
            g3 = {
                css: 'baseline-bar-red',
                long: longG3,
                width: widthG3
            }
        }
        else { //CompleteDate <IntendStartDate
            if (data.CompleteDate < data.IntendStartDate) {
                longG3 = $filter('dateDiff')(data.CompleteDate, data.IntendEndDate);
                widthG3 = longG3 == 0 ? 0 : longG3 * 100 / total;
                g3 = {
                    css: 'baseline-bar-grren',
                    long: longG3,
                    width: widthG3
                }
            }
            else { //IntendStartDate =min and intendStartDate< intendStartDate==>ahead of schedule
                if (minEndDate == data.CompleteDate && data.IntendEndDate == maxEndDate) {
                    longG3 = $filter('dateDiff')(minEndDate, maxEndDate);
                    widthG3 = longG3 == 0 ? 0 : longG3 * 100 / total;
                    g3 = {
                        css: 'baseline-bar-grren',
                        long: longG3,
                        width: widthG3
                    }
                }
                else {
                    //IntendStartDate =min and intendStartDate< intendStartDate==>ahead of schedule
                    if (minEndDate == data.IntendEndDate && data.CompleteDate == maxEndDate) {
                        longG3 = $filter('dateDiff')(minEndDate, maxEndDate);
                        widthG3 = longG3 == 0 ? 0 : longG3 * 100 / total;
                        g3 = {
                            css: 'baseline-bar-red',
                            long: longG3,
                            width: widthG3
                        }
                    }
                }
            }

        }

        g1.width = parseFloat(Math.floor(g1.width * 10)) / 10;
        g2.width = parseFloat(Math.floor(g2.width * 10)) / 10;
        g3.width = parseFloat(Math.floor(g3.width * 10)) / 10;
        // var group1 = $filter('dateDiff')(dataIntendStartDate, maxEndDate) * 100 / totalDate;


        if (g1.width == 0 && g1.width == 0 && g1.width == 0) {
            g2 = {
                css: 'baseline-bar-gray',
                long: total,
                width: 100
            }
        }
        return {
            space: space,
            longDate: total,
            group1: g1,
            group2: g2,
            group3: g3,
            width: width
        }
    }

    function dateDiff(str1, str2) {
        if (str2 == '')
            str2 = str1;
        return Math.floor((new Date(str2) - new Date(str1)) / 86400000);
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('Canceled');
    }; // end cancel

    $scope.setHoverData = function (data) {
        $scope.popoverData = converDatetoView(data);
    }
    function converDatetoView(data) {
        var item = angular.copy(data);
        if (item.IntendStartDate != '') {
            item.IntendStartDate = new Date(item.IntendStartDate);
        }
        if (item.CompleteDate != '') {
            item.CompleteDate = new Date(item.CompleteDate);
        }
        if (item.IntendEndDate != '') {
            item.IntendEndDate = new Date(item.IntendEndDate);
        }
        if (item.StartDate != '') {
            item.StartDate = new Date(item.StartDate);
        }
        item.overdueStartDate = dateDiff(data.StartDate, data.IntendStartDate);

        item.overdueEndDate = dateDiff(data.CompleteDate, data.IntendEndDate);
        item.processIntendDate = dateDiff(data.IntendStartDate, data.IntendEndDate) + 1;
        item.processRealityDate = dateDiff(data.StartDate, data.CompleteDate) + 1;
        if (data.CompleteDate == '')
            item.processRealityDate = '...';
        return item;
    }
})

.directive('baselinechartPopover', function () {
    return {
        restrict: 'A',
        template: '<span>{{label}}</span>',
        // templateUrl: '/Templates/directive/listFolderTreeView/listFolder-Parent.html',
        link: function (scope, el, attrs) {
            scope.label = attrs.popoverLabel;

            $(el).popover({
                trigger: 'mouseenter',
                html: true,
                content: attrs.popoverHtml,
                placement: attrs.popoverPlacement
            });
        }
    };
})
.directive('mypopover', function ($compile, $templateCache) {

    var getTemplate = function (contentType) {
        var template = '';
        switch (contentType) {
            case 'user':
                template = $templateCache.get("'/Templates/directive/form/right-Action.html");
                break;
        }
        return template;
    }
    return {
        restrict: "A",
        link: function (scope, element, attrs) {
            var popOverContent;
            popOverContent = $templateCache.get("template/chart/popover.html");

            var options = {
                content: popOverContent,
                placement: "right",
                html: true,
                date: scope.date,
                trigger: 'click'
            };
            $(element).popover(options);
        }
    };
});