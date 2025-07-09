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
    $scope.taxes = [];
    $scope.dynamicAttributes = [];

    handlePaginateProduct();
    fetchProducts();
    fetchAllCategories();
    fetchAllTaxes();
    fetAllDynamicAttributes();

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
  VARIABLE_PRODUCT.http.get(`api/product?pageNumber=${VARIABLE_PRODUCT.scope.paginate.pageNumber}&limit=${VARIABLE_PRODUCT.scope.paginate.limit}`).then(function (res) {
    VARIABLE_PRODUCT.scope.products = res.data.products;
    handlePaginateProduct(+res.data.totalProducts);
  });
}

function fetchAllCategories() {
  VARIABLE_PRODUCT.http.get("api/category").then(function (res) {
    VARIABLE_PRODUCT.scope.categories = res.data.categories;
  });
}

function fetAllDynamicAttributes() {
  VARIABLE_PRODUCT.http.get("api/dynamicattribute").then(function (res) {
    VARIABLE_PRODUCT.scope.dynamicAttributes = res.data.dynamicAttributes;
  });
}

function fetchAllTaxes() {
  VARIABLE_PRODUCT.http.get("api/tax/active").then(function (res) {
    VARIABLE_PRODUCT.scope.taxes = res.data;
  });
}

function handlePopupProduct() {
  setTemplatePopupProduct();
  handlePopupDescriptionProduct();
  handlePopup_DynamicAttribute();
  handlePopup_Category();
  handlePopup_Tax();
  handlePopupCreateProduct();
  handlePopupUpdateProduct();
  handlePopupDeleteProduct();
}

function setTemplatePopupProduct() {
  VARIABLE_PRODUCT.scope.templateDescription =
    window.location.origin + "/app/templates/product/showDescription.html";
  VARIABLE_PRODUCT.scope.templateCategory =
    window.location.origin + "/app/templates/product/showCategory.html";
  VARIABLE_PRODUCT.scope.templateDynamicAttribute =
    window.location.origin + "/app/templates/product/showDynamicAttribute.html";
  VARIABLE_PRODUCT.scope.templateTax =
    window.location.origin + "/app/templates/product/showTax.html";
  VARIABLE_PRODUCT.scope.templateCreate =
    window.location.origin + "/app/templates/product/showCreate.html";
  VARIABLE_PRODUCT.scope.templateUpdate =
    window.location.origin + "/app/templates/product/showUpdate.html";
    VARIABLE_PRODUCT.scope.templateDelete =
    window.location.origin + "/app/templates/product/showDelete.html";
}

function handlePopupDescriptionProduct() {
  VARIABLE_PRODUCT.scope.description = null;
  VARIABLE_PRODUCT.scope.showPopupDescription = false;

  VARIABLE_PRODUCT.scope.openPopupDescription = function (productNow) {
    VARIABLE_PRODUCT.scope.productNow = productNow;
    VARIABLE_PRODUCT.scope.showPopupDescription = true;
  };

  VARIABLE_PRODUCT.scope.closePopupDescription = function () {
    VARIABLE_PRODUCT.scope.description = null;
    VARIABLE_PRODUCT.scope.showPopupDescription = false;
  };
}

function handlePopup_DynamicAttribute() {
  VARIABLE_PRODUCT.scope.dynamicAttributes = null;
  VARIABLE_PRODUCT.scope.showPopupDynamicAttribute = false;

  VARIABLE_PRODUCT.scope.openPopupDynamicAttribute = function (productNow) {
    VARIABLE_PRODUCT.scope.productNow = productNow;
    VARIABLE_PRODUCT.scope.showPopupDynamicAttribute = true;
  };

  VARIABLE_PRODUCT.scope.closePopupDynamicAttribute = function () {
    VARIABLE_PRODUCT.scope.dynamicAttributes = null;
    VARIABLE_PRODUCT.scope.showPopupDynamicAttribute = false;
  };
}

function handlePopup_Category() {
  VARIABLE_PRODUCT.scope.showPopupCategory = false;
  VARIABLE_PRODUCT.scope.productNow = null;

  VARIABLE_PRODUCT.scope.openPopupCategory = function (productNow) {
    VARIABLE_PRODUCT.scope.productNow = productNow;
    VARIABLE_PRODUCT.scope.showPopupCategory = true;
  };

  VARIABLE_PRODUCT.scope.closePopupCategory = function () {
    VARIABLE_PRODUCT.scope.showPopupCategory = false;
    VARIABLE_PRODUCT.scope.productNow = null;
  };
}

function handlePopup_Tax() {
  VARIABLE_PRODUCT.scope.showPopupTax = false;
  VARIABLE_PRODUCT.scope.productNow = null;

  VARIABLE_PRODUCT.scope.openPopupTax = function (productNow) {
    VARIABLE_PRODUCT.scope.productNow = productNow;
    calculatePriceAfterTax(VARIABLE_PRODUCT.scope.productNow);
    VARIABLE_PRODUCT.scope.showPopupTax = true;
  };

  VARIABLE_PRODUCT.scope.closePopupTax = function () {
    VARIABLE_PRODUCT.scope.productNow = null;
    VARIABLE_PRODUCT.scope.showPopupTax = false;
  };
}

function calculatePriceAfterTax(productNow) {
  productNow.taxes = [];
  productNow.priceAfterTax = productNow.price;
  if (VARIABLE_PRODUCT.scope.taxes) {
    for (const tax of VARIABLE_PRODUCT.scope.taxes) {
      if (tax.isDefault || productNow.privateTaxIds.includes(tax.id)) {
        productNow.taxes.push(tax);
        productNow.priceAfterTax += productNow.price * tax.rate;
      }
    }
  }
}

function handlePopupCreateProduct() {
  VARIABLE_PRODUCT.scope.showPopupCreate = false;

  VARIABLE_PRODUCT.scope.createProduct = createProduct;

  VARIABLE_PRODUCT.scope.openPopupCreate = function () {
    VARIABLE_PRODUCT.scope.showPopupCreate = true;
  };

  VARIABLE_PRODUCT.scope.closePopupCreate = function () {
    VARIABLE_PRODUCT.scope.showPopupCreate = false;
  };
}

function createProduct() {
  const product = {};
  product.Name = document.getElementById("name").value;
  product.Quantity = document.getElementById("quantity").value;
  product.Price = document.getElementById("price").value;
  product.Discount = document.getElementById("discount").value;
  product.Description = document.getElementById("description").value;

  const categoryIds = document.getElementsByClassName("categoryIds");
  if (categoryIds) {
    product.CategoryIds = [];
    for (const categoryId of categoryIds) {
      if (categoryId.checked) {
        product.CategoryIds.push(categoryId.value);
      }
    }
  }

  const privateTaxIds = document.getElementsByClassName("privateTaxIds");
  if (privateTaxIds) {
    product.PrivateTaxIds = [];
    for (const taxId of privateTaxIds) {
      if (taxId.checked) {
        product.PrivateTaxIds.push(taxId.value);
      }
    }
  }

  const dynamicAttributes =
    document.getElementsByClassName("dynamicAttributes");
  if (dynamicAttributes) {
    product.DynamicAttributes = {};
    for (const dynamicAttribute of dynamicAttributes) {
      if (dynamicAttribute.value) {
        product.DynamicAttributes[dynamicAttribute.id.slice(2)] =
          dynamicAttribute.value;
      }
    }
  }

  VARIABLE_PRODUCT.http.post("api/product/create", product).then((res) => {
    fetchProducts();
  });
}

function handlePopupUpdateProduct() {
  VARIABLE_PRODUCT.scope.showPopupUpdate = false;

  VARIABLE_PRODUCT.scope.updateProduct = updateProduct;

  VARIABLE_PRODUCT.scope.openPopupUpdate = function (productNow) {
    VARIABLE_PRODUCT.scope.productNow = productNow;
    VARIABLE_PRODUCT.scope.showPopupUpdate = true;
  };

  VARIABLE_PRODUCT.scope.closePopupUpdate = function () {
    VARIABLE_PRODUCT.scope.productNow = null;
    VARIABLE_PRODUCT.scope.showPopupUpdate = false;
  };
}

function updateProduct() {
    const product = VARIABLE_PRODUCT.scope.productNow;
    product.Name = document.getElementById("name").value;
    product.Quantity = document.getElementById("quantity").value;
    product.Price = document.getElementById("price").value;
    product.Discount = document.getElementById("discount").value;
    product.Description = document.getElementById("description").value;

    const categoryIds = document.getElementsByClassName("categoryIds");
    if (categoryIds) {
      product.CategoryIds = [];
      for (const categoryId of categoryIds) {
        if (categoryId.checked) {
          product.CategoryIds.push(categoryId.value);
        }
      }
    }

    const privateTaxIds = document.getElementsByClassName("privateTaxIds");
    if (privateTaxIds) {
      product.PrivateTaxIds = [];
      for (const taxId of privateTaxIds) {
        if (taxId.checked) {
          product.PrivateTaxIds.push(taxId.value);
        }
      }
    }

    const dynamicAttributes =
      document.getElementsByClassName("dynamicAttributes");
    if (dynamicAttributes) {
      product.DynamicAttributes = {};
      for (const dynamicAttribute of dynamicAttributes) {
        if (dynamicAttribute.value) {
          product.DynamicAttributes[dynamicAttribute.id.slice(2)] =
            dynamicAttribute.value;
        }
      }
    }

    VARIABLE_PRODUCT.http.post(`api/product/update/${product.id}`, product).then((res) => {
      fetchProducts();
    });
}

function handlePopupDeleteProduct() {
  VARIABLE_PRODUCT.scope.showPopupDelete = false;

  VARIABLE_PRODUCT.scope.deleteProduct = deleteProduct;

  VARIABLE_PRODUCT.scope.openPopupDelete = function (productNow) {
    VARIABLE_PRODUCT.scope.productNow = productNow;
    VARIABLE_PRODUCT.scope.showPopupDelete = true;
  };

  VARIABLE_PRODUCT.scope.closePopupDelete = function () {
    VARIABLE_PRODUCT.scope.productNow = null;
    VARIABLE_PRODUCT.scope.showPopupDelete = false;
  };
}

function deleteProduct() {
    VARIABLE_PRODUCT.http.post(`api/product/delete/${VARIABLE_PRODUCT.scope.productNow.id}`).then((res) => {
      fetchProducts();
    });
}
