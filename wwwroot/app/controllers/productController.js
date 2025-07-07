app.controller('productController', ['$scope', '$http', function($scope, $http) {
  $scope.products = [];
  $scope.productNow = null;
  $scope.categories = [];
  $scope.taxes = [];
  
  fetchAllProducts($scope, $http);
  fetchAllCategories($scope, $http);
  fetchAllTaxes($scope, $http);

  handlePopup($scope, $http);
}]);

function fetchAllProducts($scope, $http) {
    $http.get('api/product').then(function(res) {
    $scope.products = res.data;
    });
}

function fetchAllCategories($scope, $http) {
    $http.get('api/category')
    .then(function(res) {
      $scope.categories = res.data;
    });
}

function fetchAllTaxes($scope, $http) {
    $http.get('api/tax/active')
    .then(function(res) {
      $scope.taxes = res.data;
    });
}


function handlePopup($scope, $http) {
    handlePopUpDescription($scope);
    handlePopUpDynamicAttribute($scope);
    handlePopUpCategory($scope);
    handlePopupTax($scope);
}

function handlePopUpDescription($scope) {
    $scope.description = null;
    $scope.showPopupDescription = false;

    $scope.openPopupDescription = function (productNow) {
      $scope.productNow = productNow;
      console.log(productNow);
      $scope.showPopupDescription = true;
    };

    $scope.closePopupDescription = function () {
      $scope.description = null;
      $scope.showPopupDescription = false;
    };
}

function handlePopUpDynamicAttribute($scope) {
    $scope.dynamicAttributes = null;
    $scope.showPopupDynamicAttribute = false;

    $scope.openPopupDynamicAttribute = function (productNow) {
      $scope.productNow = productNow;
      $scope.showPopupDynamicAttribute = true;
    };

    $scope.closePopupDynamicAttribute = function () {
      $scope.dynamicAttributes = null;
      $scope.showPopupDynamicAttribute = false;
    };
}

function handlePopUpCategory($scope) {
  $scope.showCategory = false;
  $scope.productNow = null;

  $scope.openPopUpCategory = function (productNow) {
    $scope.productNow = productNow;
    $scope.showCategory = true;
  };

  $scope.closePopupCategory = function () {
      $scope.showCategory = false;
      $scope.productNow = null;
  };
}

function handlePopupTax($scope) {
  $scope.showTax = false;
  $scope.productNow = null;

  $scope.openPopUpTax = function (productNow) {
    $scope.productNow = productNow;
    $scope.productNow.priceAfterTax = $scope.productNow.price;
    $scope.showTax = true;
  };

  $scope.closePopupTax = function () {
      $scope.productNow = null;
      $scope.showTax = false;
  };

  $scope.calculatePriceAfterTax = function(taxRate) {
    $scope.productNow.priceAfterTax += $scope.productNow.price * taxRate;
  }
}

