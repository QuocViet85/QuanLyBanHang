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
    .when('/dynamicattribute', {
      templateUrl: window.location.origin + '/app/templates/dynamicAttribute/dynamicAttribute.html',
      controller: 'dynamicAttributeController'
    })
    .when('/tax', {
      templateUrl: window.location.origin + '/app/templates/tax/tax.html',
      controller: 'taxController'
    })
    .when('/customer', {
      templateUrl: window.location.origin + '/app/templates/customer/customer.html',
      controller: 'customerController'
    })
}]);


