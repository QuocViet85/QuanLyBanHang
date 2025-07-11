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
    $scope.defaultTaxes = [];
    $scope.privateTaxes = [];
    $scope.dynamicAttributes = [];
    $scope.customers = [];

    $scope.cart = []; //giỏ hàng

    handlePaginateProduct();
    fetchProducts();
    fetchAllCategories();
    fetchAllTaxes();
    fetAllDynamicAttributes();
    fetchAllCustomer();

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
    if (VARIABLE_PRODUCT.scope.taxes) {
      for (const tax of VARIABLE_PRODUCT.scope.taxes) {
        if (tax.isDefault) {
          VARIABLE_PRODUCT.scope.defaultTaxes.push(tax);
        }else {
          VARIABLE_PRODUCT.scope.privateTaxes.push(tax);
        }
      }
    }
  });
}

function fetchAllCustomer() {
  VARIABLE_PRODUCT.http.get("api/customer").then(function (res) {
    VARIABLE_PRODUCT.scope.customers = res.data.customers;
  })
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
  handlePopupSellProduct();
  handlePopupCartProduct();
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
  VARIABLE_PRODUCT.scope.templateSell =
  window.location.origin + "/app/templates/product/showSell.html";
  VARIABLE_PRODUCT.scope.templateCart =
  window.location.origin + "/app/templates/product/showCart.html";
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
  productNow.privateTaxes = [];
  var realPrice = productNow.price;
  if (productNow.discount) {
    realPrice -= productNow.discount;
  }

  productNow.priceAfterPrivateTaxes = realPrice;
  
  if (VARIABLE_PRODUCT.scope.privateTaxes) {
    for (const tax of VARIABLE_PRODUCT.scope.privateTaxes) {
      if (productNow.privateTaxIds.includes(tax.id)) {
        productNow.privateTaxes.push(tax);
        productNow.priceAfterPrivateTaxes *= (1 + tax.rate);
      }
    }
  }

  productNow.priceAfterPrivateTaxes = Math.round(productNow.priceAfterPrivateTaxes);

  productNow.priceAfterTaxes = productNow.priceAfterPrivateTaxes;

  if (VARIABLE_PRODUCT.scope.defaultTaxes) {
    for (const tax of VARIABLE_PRODUCT.scope.defaultTaxes) {
      productNow.priceAfterTaxes *= (1 + tax.rate);
    }
  }

  productNow.priceAfterTaxes = Math.round(productNow.priceAfterTaxes);
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

function handlePopupSellProduct() {
  VARIABLE_PRODUCT.scope.showPopupSell = false;

  VARIABLE_PRODUCT.scope.pushProductToCart = pushProductToCart;

  VARIABLE_PRODUCT.scope.openPopupSell = function (productNow) {
    VARIABLE_PRODUCT.scope.productNow = productNow;
    VARIABLE_PRODUCT.scope.showPopupSell = true;
  };

  VARIABLE_PRODUCT.scope.closePopupSell = function () {
    VARIABLE_PRODUCT.scope.productNow = null;
    VARIABLE_PRODUCT.scope.showPopupSell = false;
  };
}

function pushProductToCart(productNow) {
  var quantity = document.getElementById("quantityProduct").value;
  quantity = +quantity;

  if (quantity > 0) {
    var existInCart = false;

    for (var item of VARIABLE_PRODUCT.scope.cart) {
      if (productNow === item.product) {
        existInCart = true;

        if (quantity + item.quantity <= productNow.quantity) {
          item.quantity = quantity + item.quantity;
        }
      }
    }

    if (!existInCart) {
      if (quantity <= productNow.quantity) {
        const productInCart = {
        product: productNow,
        quantity: quantity
        }

        VARIABLE_PRODUCT.scope.cart.push(productInCart);
      }
      
    }
  }
  VARIABLE_PRODUCT.scope.closePopupSell();
}

function handlePopupCartProduct() {
  VARIABLE_PRODUCT.scope.showPopupCart = false;

  VARIABLE_PRODUCT.scope.createOrder = createOrder;
  VARIABLE_PRODUCT.scope.deleteCart = deleteCart;

  VARIABLE_PRODUCT.scope.openPopupCart = function () {
    calculatorTotalPriceInCart();
    VARIABLE_PRODUCT.scope.showPopupCart = true;
  };

  VARIABLE_PRODUCT.scope.closePopupCart = function () {
    VARIABLE_PRODUCT.scope.showPopupCart = false;
  };
}

function calculatorTotalPriceInCart() {
  VARIABLE_PRODUCT.scope.cart.totalBeforeDefaultTax = 0;
  VARIABLE_PRODUCT.scope.cart.totalAfterTax = 0;
  for (var item of VARIABLE_PRODUCT.scope.cart) {
    //tính giá 1 sản phẩm(bao gồm thuế riêng, thuế riêng + thuế chung)
    calculatePriceAfterTax(item.product); 

    //tính giá nhiều sản phẩm cùng loại trong giỏ hàng với số lượng đã chọn (bao gồm thuế riêng, thuế riêng + thuế chung)
    item.priceAfterPrivateTaxes = item.product.priceAfterPrivateTaxes * item.quantity;
    item.priceAfterTaxes = item.product.priceAfterTaxes * item.quantity;
    VARIABLE_PRODUCT.scope.cart.totalBeforeDefaultTax += item.priceAfterPrivateTaxes;
    VARIABLE_PRODUCT.scope.cart.totalAfterTax += item.priceAfterTaxes;
  }
}

function createOrder() {
  const order = {};
  order.Name = document.getElementById("orderName").value;
  order.CustomerName = document.getElementById("customerName").value;
  order.CustomerPhoneNumber = document.getElementById("customerPhoneNumber").value;
  order.Completed = document.getElementById("orderCompleted").checked;

  order.ProductInOrders = [];
  
  for (var item of VARIABLE_PRODUCT.scope.cart) {
    order.ProductInOrders.push({
      ProductId: item.product.id,
      Quantity: item.quantity
    });
  }

  VARIABLE_PRODUCT.http.post("api/order/create", order).then((res) => {
    if (order.Completed) {
      fetchProducts();
    }
  });
}

function deleteCart() {
  VARIABLE_PRODUCT.scope.cart = [];
}
