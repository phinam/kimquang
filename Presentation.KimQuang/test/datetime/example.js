angular.module('ui.bootstrap.demo', ['ngAnimate', 'ui.bootstrap']);
angular.module('ui.bootstrap.demo').controller('DatepickerDemoCtrl', function ($scope) {
    $scope.today = function () {
        $scope.dt = new Date();
    };
    $scope.today();

    $scope.clear = function () {
        $scope.dt = null;
    };

    // Disable weekend selection
    //$scope.disabled = function (date, mode) {
    //    return (mode === 'day' && (date.getDay() === 0 || date.getDay() === 6));
    //};

    //$scope.toggleMin = function () {
    //    $scope.minDate = $scope.minDate ? null : new Date();
    //};
    //$scope.toggleMin();

    $scope.open = function ($event) {
        $scope.status.opened = true;
    };

   

    $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
    $scope.format = $scope.formats[0];

    $scope.status = {
        opened: false
    };
    $scope.open = function ($event) {
        $scope.status.opened = true;
    };

    $scope.setDate = function () {
        var date = new Date(), y = date.getFullYear(), m = date.getMonth();
        $scope.dt = new Date(y, m, 1);
    };
});