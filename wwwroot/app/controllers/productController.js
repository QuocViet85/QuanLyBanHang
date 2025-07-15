const VARIABLE_PRODUCT = {};
app.controller("productController", [
  "$scope",
  "$http",
  function ($scope, $http) {
    VARIABLE_PRODUCT.scope = $scope;
    VARIABLE_PRODUCT.http = $http;
    $scope.products = [];
    $scope.productNow = null;
    $scope.categories = [];
    $scope.productIdChecks = [];
    $scope.fetchProducts = fetchProducts;

    $scope.searchByName = '';
    $scope.searchByCode = '';
    $scope.searchByCategory = '';
    $scope.searchByUnit = '';

    $scope.message = {}
    $scope.message.success = false;
    $scope.message.error = false;

    handlePaginateProduct();
    setUnits();
    fetchProducts();
    fetchAllCategories();

    handlePopupProduct();
  },
]);

function handlePaginateProduct(totalProducts) {
    if (!totalProducts) {
      VARIABLE_PRODUCT.scope.paginate = {};
      VARIABLE_PRODUCT.scope.paginate.pageNumber = 1;
      VARIABLE_PRODUCT.scope.paginate.limit = 10;

      VARIABLE_PRODUCT.scope.paginate.minusPageNumber = () => {
        if (VARIABLE_PRODUCT.scope.paginate.pageNumber > 1) {
          VARIABLE_PRODUCT.scope.paginate.pageNumber -= 1;
        }else {
          VARIABLE_PRODUCT.scope.paginate.pageNumber = 1;
        }
        fetchProducts();
      }

      VARIABLE_PRODUCT.scope.paginate.plusPageNumber = () => {
        VARIABLE_PRODUCT.scope.paginate.pageNumber += 1;
        fetchProducts();
      }
    }else {
      VARIABLE_PRODUCT.scope.paginate.totalPages = Math.ceil(totalProducts / VARIABLE_PRODUCT.scope.paginate.limit);
    }
}

function fetchProducts() {
  VARIABLE_PRODUCT.http.get(`api/product?pageNumber=${VARIABLE_PRODUCT.scope.paginate.pageNumber}&limit=${VARIABLE_PRODUCT.scope.paginate.limit}&searchByName=${VARIABLE_PRODUCT.scope.searchByName}&searchByCode=${VARIABLE_PRODUCT.scope.searchByCode}&searchByCategory=${VARIABLE_PRODUCT.scope.searchByCategory}&searchByUnit=${VARIABLE_PRODUCT.scope.searchByUnit}`).then(function (res) {
    if (!res.data.products) {
      throw "Không tìm thấy sản phẩm";
    }
    VARIABLE_PRODUCT.scope.products = res.data.products;
    handlePaginateProduct(+res.data.totalProducts);

    for (var product of VARIABLE_PRODUCT.scope.products) {
      product.priceImportShow = formatCurrency(product.priceImport);
      product.priceWholesaleShow = formatCurrency(product.priceWholesale);
      product.priceRetailShow = formatCurrency(product.priceRetail);
    }
  })
  .catch(err => {
    showMessage(VARIABLE_PRODUCT.scope, err, false);
  });
}

function fetchAllCategories() {
  VARIABLE_PRODUCT.http.get("api/category").then(function (res) {
    VARIABLE_PRODUCT.scope.categories = res.data.categories;
  });
}

function setUnits() {
  VARIABLE_PRODUCT.scope.units = {
    1: 'Bao',
    2: 'Bộ',
    3: 'Cái',
    4: 'Cây',
    5: 'Chiếc',
    6: 'Lon',
    7: 'Thùng'
  };

}

function handlePopupProduct() {
  setTemplatePopupProduct();
  handlePopupDescriptionProduct();
  handlePopupCreateProduct();
  handlePopupUpdateProduct();
  handlePopupDeleteProduct();
  handlePopupActiveProduct();
}

function setTemplatePopupProduct() {
  VARIABLE_PRODUCT.scope.templateDescription =
    window.location.origin + "/app/templates/product/showDescription.html";
  VARIABLE_PRODUCT.scope.templateCreate =
    window.location.origin + "/app/templates/product/showCreate.html";
  VARIABLE_PRODUCT.scope.templateUpdate =
    window.location.origin + "/app/templates/product/showUpdate.html";
  VARIABLE_PRODUCT.scope.templateDelete =
    window.location.origin + "/app/templates/product/showDelete.html";
    VARIABLE_PRODUCT.scope.templateActive =
    window.location.origin + "/app/templates/product/showActive.html";
    VARIABLE_PRODUCT.scope.templateMessage = 
    window.location.origin + "/app/templates/showMessage.html";
}

function handlePopupDescriptionProduct() {
  VARIABLE_PRODUCT.scope.showPopupDescription = false;

  VARIABLE_PRODUCT.scope.openPopupDescription = function (productNow) {
    VARIABLE_PRODUCT.scope.productNow = productNow;
    VARIABLE_PRODUCT.scope.showPopupDescription = true;
  };

  VARIABLE_PRODUCT.scope.closePopupDescription = function () {
    VARIABLE_PRODUCT.scope.productNow = null;
    VARIABLE_PRODUCT.scope.showPopupDescription = false;
  };
}

function handlePopupCreateProduct() {
  VARIABLE_PRODUCT.scope.showPopupCreate = false;

  VARIABLE_PRODUCT.scope.createProduct = createProduct;

  VARIABLE_PRODUCT.scope.openPopupCreate = function () {
    setFormatNumberInput();
    VARIABLE_PRODUCT.scope.showPopupCreate = true;
  };

  VARIABLE_PRODUCT.scope.closePopupCreate = function () {
    VARIABLE_PRODUCT.scope.showPopupCreate = false;
  };
}


function createProduct() {
  const product = {};
  product.Name = document.getElementById("nameProduct").value;
  product.Code = document.getElementById("codeProduct").value;
  product.Serial = document.getElementById("serialProduct").value;
  product.PriceImport = document.getElementById("priceImportProduct").value;
  product.PriceWholesale = document.getElementById("priceWholesaleProduct").value;
  product.PriceRetail = document.getElementById("priceRetailProduct").value;
  product.Unit = document.getElementById("unitProduct").value;
  product.InventoryStandard = document.getElementById("inventoryStandardProduct").value;
  product.Description = document.getElementById("descriptionProduct").value;
  product.IsActive = document.getElementById("isActiveProduct").checked;

  const categoryIdProduct = document.getElementById("categoryIdProduct");

  if (categoryIdProduct) {
    if (categoryIdProduct.value) {
      product.CategoryId = categoryIdProduct.value;
    }
  }

  VARIABLE_PRODUCT.http.post("api/product/create", product).then((res) => {
    fetchProducts();
    showMessage(VARIABLE_PRODUCT.scope, res.data, true);
  })
  .catch(err => {
    showMessage(VARIABLE_PRODUCT.scope, err.data, false)
  });
}

function handlePopupUpdateProduct() {
  VARIABLE_PRODUCT.scope.showPopupUpdate = false;

  VARIABLE_PRODUCT.scope.updateProduct = updateProduct;

  VARIABLE_PRODUCT.scope.openPopupUpdate = function (productNow) {
    VARIABLE_PRODUCT.scope.productNow = productNow;
    VARIABLE_PRODUCT.scope.showPopupUpdate = true;
    setFormatNumberInput();
  };

  VARIABLE_PRODUCT.scope.closePopupUpdate = function () {
    VARIABLE_PRODUCT.scope.productNow = null;
    VARIABLE_PRODUCT.scope.showPopupUpdate = false;
  };
}

function updateProduct() {
    const product = {};
    product.Name = document.getElementById("nameProduct").value;
    product.Code = document.getElementById("codeProduct").value;
    product.Serial = document.getElementById("serialProduct").value;
    product.PriceImport = document.getElementById("priceImportProduct").value;
    product.PriceWholesale = document.getElementById("priceWholesaleProduct").value;
    product.PriceRetail = document.getElementById("priceRetailProduct").value;
    product.Unit = document.getElementById("unitProduct").value;
    product.InventoryStandard = document.getElementById("inventoryStandardProduct").value;
    product.Description = document.getElementById("descriptionProduct").value;
    product.IsActive = document.getElementById("isActiveProduct").checked;

    const categoryIdProduct = document.getElementById("categoryIdProduct");

    if (categoryIdProduct) {
      if (categoryIdProduct.value) {
        product.CategoryId = categoryIdProduct.value;
      }
    }

    VARIABLE_PRODUCT.http.post(`api/product/update/${VARIABLE_PRODUCT.scope.productNow.id}`, product).then((res) => {
      fetchProducts();
      showMessage(VARIABLE_PRODUCT.scope, res.data, true);
    })
    .catch(err => {
      showMessage(VARIABLE_PRODUCT.scope, err.data, false);
    });
}

function setFormatNumberInput() {
    formatNumberInput('priceImportProductShow', 'priceImportProduct');
    formatNumberInput('priceWholesaleProductShow', 'priceWholesaleProduct');
    formatNumberInput('priceRetailProductShow', 'priceRetailProduct');
    formatNumberInput('inventoryStandardProductShow', 'inventoryStandardProduct');
}

function handlePopupDeleteProduct() {
  VARIABLE_PRODUCT.scope.showPopupDelete = false;

  VARIABLE_PRODUCT.scope.deleteProduct = deleteProduct;

  VARIABLE_PRODUCT.scope.openPopupDelete = function () {
    VARIABLE_PRODUCT.scope.showPopupDelete = true;
  };

  VARIABLE_PRODUCT.scope.closePopupDelete = function () {
    VARIABLE_PRODUCT.scope.showPopupDelete = false;
  };
}

function deleteProduct() {
    VARIABLE_PRODUCT.http.post(`api/product/delete`, VARIABLE_PRODUCT.scope.productIdChecks).then((res) => {
      fetchProducts();
      showMessage(VARIABLE_PRODUCT.scope, res.data, true);
    })
    .catch(err => {
      showMessage(VARIABLE_PRODUCT.scope, err.data, false);
    });
}

function handlePopupActiveProduct() {
  VARIABLE_PRODUCT.scope.showPopupActive = false;

  VARIABLE_PRODUCT.scope.activeProduct = activeProduct;

  VARIABLE_PRODUCT.scope.openPopupActive = function () {
    VARIABLE_PRODUCT.scope.showPopupActive = true;
  };

  VARIABLE_PRODUCT.scope.closePopupActive = function () {
    VARIABLE_PRODUCT.scope.showPopupActive = false;
  };
}

function activeProduct() {
    VARIABLE_PRODUCT.http.post(`api/product/active-unactive`, VARIABLE_PRODUCT.scope.productIdChecks).then((res) => {
      fetchProducts();
      document.getElementById('checkBoxAllProduct').checked = false;
      showMessage(VARIABLE_PRODUCT.scope, res.data, true);
    })
    .catch(err => {
      showMessage(VARIABLE_PRODUCT.scope, err.data, false);
    });
}

function checkAllProducts() {
  const checkBoxAllProduct = document.getElementById('checkBoxAllProduct');

  const checkBoxProducts = document.getElementsByClassName('checkboxProduct');

  VARIABLE_PRODUCT.scope.productIdChecks = [];

  if (checkBoxAllProduct.checked) {
      for (const checkBoxProduct of checkBoxProducts) {
      checkBoxProduct.checked = checkBoxAllProduct.checked;
      VARIABLE_PRODUCT.scope.productIdChecks.push(parseInt(checkBoxProduct.id.slice('5')));
    }
  }else {
    for (const checkBoxProduct of checkBoxProducts) {
      checkBoxProduct.checked = checkBoxAllProduct.checked;
    }
  }
}

function setProductCheck(element) {
  const productId = parseInt(element.id.slice('5'));
  if (element.checked) {
    VARIABLE_PRODUCT.scope.productIdChecks.push(productId);
  }else {
    const index = VARIABLE_PRODUCT.scope.productIdChecks.indexOf(productId);
    if (index !== -1) {
      VARIABLE_PRODUCT.scope.productIdChecks.splice(index);
    }
  }
}


