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
}]);


