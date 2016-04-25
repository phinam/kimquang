.directive('vmisTable', function () {
        return {
            restrict: "AE",
            templateUrl: function (elem, attrs) {
                return attrs["templateUrl"] || 'views/core.form.table.html';
            },
            scope: {
                view: '=vmisTable',
                model: '=viewModel'
            },
            controller: function ($scope, $element, $attrs, $q, DTOptionsBuilder, DTColumnBuilder, $timeout) {
                var controls = vmis.constant.CONTROLS_TYPE;
                $scope.dtOptions = DTOptionsBuilder.newOptions();
                $scope.dtColumns = standardFields($scope.view.fields);
                $scope.dtInstance = {};

                console.log('::vmisTable::controller', $scope.view.fields);

                //vm.dtColumns = [
                //    DTColumnBuilder.newColumn(null).withTitle(titleHtml).notSortable()
                //        .renderWith(function (data, type, full, meta) {
                //            vm.selected[full.id] = false;
                //            return '<input type="checkbox" ng-model="showCase.selected[' + data.id + ']" ng-click="showCase.toggleOne(showCase.selected)">';
                //        }),
                //    DTColumnBuilder.newColumn('id').withTitle('ID'),
                //    DTColumnBuilder.newColumn('firstName').withTitle('First name'),
                //    DTColumnBuilder.newColumn('lastName').withTitle('Last name').notVisible()
                //                ];


                $timeout(function () {
                    $scope.$watch('view.data.resultset', function (data, oldData) {
                        if (data) {
                            $scope.model[vmis.constant.ITEM_NAME] = data;
                            $scope.dtInstance.dataTable.fnAddData(data);
                        }
                    }, true);
                }, 100);

                function standardFields(fields) {
                    var columns = [];
                    for (var i = 0; i < fields.length; i++) {
                        var field = fields[i];
                        columns.push(standardField2Column(field));
                    }
                    return columns;
                }


                function standardField2Column(field) {
                    var col = DTColumnBuilder.newColumn(field.key);

                    col.withTitle(field.templateOptions.label);
                    col.withClass(field.className);
                    col.notSortable();

                    switch (field.type) {
                        case controls.ICON_AND_TEXT:
                            col.notSortable();
                            col.renderWith(function (data, type, full, meta) {
                                console.log(data)
                                return [
                                '<button type="button" class="btn btn-primary" ng-click="grid.appScope.action(row,col.filter)">',
                                    '<span><i class="fa ', field.classIcon, '"></i>', field.templateOptions.placeholder, '</span>',
                                '</button>'
                                ].join('');
                            });
                            break;

                        case controls.CHECKBOX:
                            col.notSortable();
                            col.renderWith(function (data, type, full, meta) {
                                console.log('::CHECKBOX', data, type, full, meta);
                                return [
                                    '<div class="checkbox checkbox-success">',
                                        '<input type="checkbox" id="chk-{{meta.row}}-{{meta.cold}}" ng-model="full[',
                                            'col.field]" />',
                                        '<label for="chk-{{meta.row}}-{{meta.cold}}"></label>',
                                    '</div>'
                                ].join('');
                            });
                            break;

                        default:

                            break;
                    }
                    return col;

                }

                function renderButton(data, type, full, meta) {
                    return [
                               '<button type="button" class="btn btn-primary" ng-click="grid.appScope.action(row,col.filter)">',
                                   '<span><i class="fa ', field.templateOptions.classIcon, '"></i>', field.templateOptions.label, '</span>',
                               '</button>'
                    ].join('');
                }

                function renderCheckbox(data, type, full, meta) {
                    console.log('checkbox', data, type, full, meta);
                    return [
                                '<div class="checkbox checkbox-success">',
                                    '<input type="checkbox" id="chk-{{col.field}}-{{row.uid}}" ng-model="row.entity[col.field]" />',
                                    '<label for="chk-{{col.field}}-{{row.uid}}">{{col.IsUpdate}}</label>',
                                '</div>'
                    ].join('');
                }
            }
        }
    })
    