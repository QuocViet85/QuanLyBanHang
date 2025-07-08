const GLOBAL_VARIABLE = {};

app.controller("productController", [
  "$scope",
  "$http",
  function ($scope, $http) {
    GLOBAL_VARIABLE.scope = $scope;
    GLOBAL_VARIABLE.http = $http;
    $scope.products = [];
    $scope.productNow = null;
    $scope.categories = [];
    $scope.taxes = [];
    $scope.dynamicAttributes = [];

    
    handlePaginate();
    fetchProducts();
    fetchAllCategories();
    fetchAllTaxes();
    fetAllDynamicAttributes();

    handlePopup();
  },
]);

function handlePaginate(totalProducts) {
    if (!totalProducts) {
      GLOBAL_VARIABLE.scope.paginate = {};
      GLOBAL_VARIABLE.scope.paginate.pageNumber = 1;
      GLOBAL_VARIABLE.scope.paginate.limit = 10;

      GLOBAL_VARIABLE.scope.paginate.minusPageNumber = () => {
        if (GLOBAL_VARIABLE.scope.paginate.pageNumber > 1) {
          GLOBAL_VARIABLE.scope.paginate.pageNumber -= 1;
        }else {
          GLOBAL_VARIABLE.scope.paginate.pageNumber = 1;
        }
        fetchProducts();
      }

      GLOBAL_VARIABLE.scope.paginate.plusPageNumber = () => {
        GLOBAL_VARIABLE.scope.paginate.pageNumber += 1;
        fetchProducts();
      }
    }else {
      GLOBAL_VARIABLE.scope.paginate.totalPages = Math.ceil(totalProducts / GLOBAL_VARIABLE.scope.paginate.limit);
    }
    
}

function fetchProducts() {
  console.log(GLOBAL_VARIABLE.scope.limit, 'PRODUCT')
  GLOBAL_VARIABLE.http.get(`api/product?pageNumber=${GLOBAL_VARIABLE.scope.paginate.pageNumber}&limit=${GLOBAL_VARIABLE.scope.paginate.limit}`).then(function (res) {
    GLOBAL_VARIABLE.scope.products = res.data.products;
    handlePaginate(+res.data.totalProducts);
  });
}

function fetchAllCategories() {
  GLOBAL_VARIABLE.http.get("api/category").then(function (res) {
    GLOBAL_VARIABLE.scope.categories = res.data;
  });
}

function fetAllDynamicAttributes() {
  GLOBAL_VARIABLE.http.get("api/dynamicattribute").then(function (res) {
    GLOBAL_VARIABLE.scope.dynamicAttributes = res.data;
  });
}

function fetchAllTaxes() {
  GLOBAL_VARIABLE.http.get("api/tax/active").then(function (res) {
    GLOBAL_VARIABLE.scope.taxes = res.data;
  });
}

function handlePopup() {
  setTemplatePopup();
  handlePopupDescription();
  handlePopupDynamicAttribute();
  handlePopupCategory();
  handlePopupTax();
  handlePopupCreate();
  handlePopupUpdate();
  handlePopupDelete();
}

function setTemplatePopup() {
  GLOBAL_VARIABLE.scope.templateDescription =
    window.location.origin + "/app/templates/product/showDescription.html";
  GLOBAL_VARIABLE.scope.templateCategory =
    window.location.origin + "/app/templates/product/showCategory.html";
  GLOBAL_VARIABLE.scope.templateDynamicAttribute =
    window.location.origin + "/app/templates/product/showDynamicAttribute.html";
  GLOBAL_VARIABLE.scope.templateTax =
    window.location.origin + "/app/templates/product/showTax.html";
  GLOBAL_VARIABLE.scope.templateCreate =
    window.location.origin + "/app/templates/product/showCreate.html";
  GLOBAL_VARIABLE.scope.templateUpdate =
    window.location.origin + "/app/templates/product/showUpdate.html";
    GLOBAL_VARIABLE.scope.templateDelete =
    window.location.origin + "/app/templates/product/showDelete.html";
}

function handlePopupDescription() {
  GLOBAL_VARIABLE.scope.description = null;
  GLOBAL_VARIABLE.scope.showPopupDescription = false;

  GLOBAL_VARIABLE.scope.openPopupDescription = function (productNow) {
    GLOBAL_VARIABLE.scope.productNow = productNow;
    GLOBAL_VARIABLE.scope.showPopupDescription = true;
  };

  GLOBAL_VARIABLE.scope.closePopupDescription = function () {
    GLOBAL_VARIABLE.scope.description = null;
    GLOBAL_VARIABLE.scope.showPopupDescription = false;
  };
}

function handlePopupDynamicAttribute() {
  GLOBAL_VARIABLE.scope.dynamicAttributes = null;
  GLOBAL_VARIABLE.scope.showPopupDynamicAttribute = false;

  GLOBAL_VARIABLE.scope.openPopupDynamicAttribute = function (productNow) {
    GLOBAL_VARIABLE.scope.productNow = productNow;
    GLOBAL_VARIABLE.scope.showPopupDynamicAttribute = true;
  };

  GLOBAL_VARIABLE.scope.closePopupDynamicAttribute = function () {
    GLOBAL_VARIABLE.scope.dynamicAttributes = null;
    GLOBAL_VARIABLE.scope.showPopupDynamicAttribute = false;
  };
}

function handlePopupCategory() {
  GLOBAL_VARIABLE.scope.showPopupCategory = false;
  GLOBAL_VARIABLE.scope.productNow = null;

  GLOBAL_VARIABLE.scope.openPopupCategory = function (productNow) {
    GLOBAL_VARIABLE.scope.productNow = productNow;
    GLOBAL_VARIABLE.scope.showPopupCategory = true;
  };

  GLOBAL_VARIABLE.scope.closePopupCategory = function () {
    GLOBAL_VARIABLE.scope.showPopupCategory = false;
    GLOBAL_VARIABLE.scope.productNow = null;
  };
}

function handlePopupTax() {
  GLOBAL_VARIABLE.scope.showPopupTax = false;
  GLOBAL_VARIABLE.scope.productNow = null;

  GLOBAL_VARIABLE.scope.openPopupTax = function (productNow) {
    GLOBAL_VARIABLE.scope.productNow = productNow;
    calculatePriceAfterTax(GLOBAL_VARIABLE.scope.productNow);
    GLOBAL_VARIABLE.scope.showPopupTax = true;
  };

  GLOBAL_VARIABLE.scope.closePopupTax = function () {
    GLOBAL_VARIABLE.scope.productNow = null;
    GLOBAL_VARIABLE.scope.showPopupTax = false;
  };
}

function calculatePriceAfterTax(productNow) {
  productNow.taxes = [];
  productNow.priceAfterTax = productNow.price;
  if (GLOBAL_VARIABLE.scope.taxes) {
    for (const tax of GLOBAL_VARIABLE.scope.taxes) {
      if (tax.isDefault || productNow.privateTaxIds.includes(tax.id)) {
        productNow.taxes.push(tax);
        productNow.priceAfterTax += productNow.price * tax.rate;
      }
    }
  }
}

function handlePopupCreate() {
  GLOBAL_VARIABLE.scope.showPopupCreate = false;

  GLOBAL_VARIABLE.scope.createProduct = createProduct;

  GLOBAL_VARIABLE.scope.openPopupCreate = function () {
    GLOBAL_VARIABLE.scope.showPopupCreate = true;
  };

  GLOBAL_VARIABLE.scope.closePopupCreate = function () {
    GLOBAL_VARIABLE.scope.showPopupCreate = false;
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

  GLOBAL_VARIABLE.http.post("api/product/create", product).then((res) => {
    fetchProducts();
  });
}

function handlePopupUpdate() {
  GLOBAL_VARIABLE.scope.showPopupUpdate = false;

  GLOBAL_VARIABLE.scope.updateProduct = updateProduct;

  GLOBAL_VARIABLE.scope.openPopupUpdate = function (productNow) {
    GLOBAL_VARIABLE.scope.productNow = productNow;
    GLOBAL_VARIABLE.scope.showPopupUpdate = true;
  };

  GLOBAL_VARIABLE.scope.closePopupUpdate = function () {
    GLOBAL_VARIABLE.scope.productNow = null;
    GLOBAL_VARIABLE.scope.showPopupUpdate = false;
  };
}

function updateProduct() {
    const product = GLOBAL_VARIABLE.scope.productNow;
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

    GLOBAL_VARIABLE.http.post(`api/product/update/${product.id}`, product).then((res) => {
      fetchProducts();
    });
}

function handlePopupDelete() {
  GLOBAL_VARIABLE.scope.showPopupDelete = false;

  GLOBAL_VARIABLE.scope.deleteProduct = deleteProduct;

  GLOBAL_VARIABLE.scope.openPopupDelete = function (productNow) {
    GLOBAL_VARIABLE.scope.productNow = productNow;
    GLOBAL_VARIABLE.scope.showPopupDelete = true;
  };

  GLOBAL_VARIABLE.scope.closePopupDelete = function () {
    GLOBAL_VARIABLE.scope.productNow = null;
    GLOBAL_VARIABLE.scope.showPopupDelete = false;
  };
}

function deleteProduct() {
    GLOBAL_VARIABLE.http.post(`api/product/delete/${GLOBAL_VARIABLE.scope.productNow.id}`).then((res) => {
      fetchProducts();
    });
}
