const app = angular.module('myApp', ['ngRoute']);

app.config(['$locationProvider', function($locationProvider) {
  $locationProvider.html5Mode(true); // bật HTML5 mode
}]);

app.config(['$routeProvider', function($routeProvider) {
  $routeProvider
    .when('/product', {
      templateUrl: window.location.origin + '/app/templates/product/product.html',
      controller: 'productController'
    })
    .when('/category', {
      templateUrl: window.location.origin + '/app/templates/category/category.html',
      controller: 'categoryController'
    })
}]);


function showMessage($scope, message, success = true) {
  if (success) {
    $scope.message.success = message;
  }else {
    $scope.message.error = message;
  }
  setTimeout(() => {
      $scope.$apply(function() {
        $scope.message.success = false;
        $scope.message.error = false;
    })
  }, 5000);
}