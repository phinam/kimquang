angular.module('indexApp')
  .filter('dateDiff', function () {
      return function (d1, d2, isUp) {
          if (typeof isUp == 'undefined')
              isUp = false;
          if (d2 == '')
              d2 = d1;
          if (isUp)
              return Math.round((new Date(d2) - new Date(d1)) / 86400000);
          return Math.floor((new Date(d2) - new Date(d1)) / 86400000);
      };
  })
  .filter('parseDateFromDB', function () {
      return function (d) {
          if (typeof d == 'undefined' || d == '') return null;
          return new Date(d);
      };
  })
  .filter('parseDateToDB', function ($filter) {
      return function (d) {
          if (typeof d == 'undefined' || d == "")
              return "";
          if (typeof d != 'object') {
              var from = d.split("-");
              d = new Date(from[2], from[1] - 1, from[0]);
          }
          return $filter('date')(d, "yyyy-MM-dd");
      };
  })
.filter('getMinDate', function () {
    return function (d1, d2) {
        if (d2 == '')
            d2 = d1;
        return Math.floor((new Date(d2) - new Date(d1)) / 86400000) > 0 ? d1 : d2;
    }

})

.filter('getMaxDate', function () {
    return function (d1, d2) {
        if (d2 == '')
            d2 = d1;
        return Math.floor((new Date(d2) - new Date(d1)) / 86400000) > 0 ? d2 : d1;
    }

})
.filter('filterDistrictID', function () {
    return function (array, selectedDistrictID) {
        var tempArray = [];
        if (selectedDistrictID != '')
            angular.forEach(array, function (item, key) {
                if (angular.equals(item.DistrictID, selectedDistrictID)) {
                    tempArray.push(item);
                }
            });
        return tempArray;
    };
})

.filter('filterExcludeValue', function () {
    return function (array, value) {
        var tempArray = [];
        angular.forEach(array, function (item, key) {
            if (!angular.equals(item.Value, value)) {
                tempArray.push(item);
            }
        });

        return tempArray;
    };
})
