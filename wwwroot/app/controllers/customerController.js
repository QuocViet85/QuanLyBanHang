const VARIABLE_CUSTOMER = {};
app.controller("customerController", [
  "$scope",
  "$http",
  function ($scope, $http) {
    VARIABLE_CUSTOMER.scope = $scope;
    VARIABLE_CUSTOMER.http = $http;
    $scope.customers = [];
    $scope.customerNow = null;

    handlePaginateCustomer();
    fetchCustomers();
    handlePopupCustomer();
  },
]);

function handlePaginateCustomer(totalCustomers) {
    if (!totalCustomers) {
      VARIABLE_CUSTOMER.scope.paginate = {};
      VARIABLE_CUSTOMER.scope.paginate.pageNumber = 1;
      VARIABLE_CUSTOMER.scope.paginate.limit = 10;

      VARIABLE_CUSTOMER.scope.paginate.minusPageNumber = () => {
        if (VARIABLE_CUSTOMER.scope.paginate.pageNumber > 1) {
          VARIABLE_CUSTOMER.scope.paginate.pageNumber -= 1;
        }else {
          VARIABLE_CUSTOMER.scope.paginate.pageNumber = 1;
        }
        fetchCustomers();
      }

      VARIABLE_CUSTOMER.scope.paginate.plusPageNumber = () => {
        VARIABLE_CUSTOMER.scope.paginate.pageNumber += 1;
        fetchCustomers();
      }
    }else {
      VARIABLE_CUSTOMER.scope.paginate.totalPages = Math.ceil(totalCustomers / VARIABLE_CUSTOMER.scope.paginate.limit);
    }
}

function fetchCustomers() {
  VARIABLE_CUSTOMER.http.get(`api/customer?pageNumber=${VARIABLE_CUSTOMER.scope.paginate.pageNumber}&limit=${VARIABLE_CUSTOMER.scope.paginate.limit}`).then(function (res) {
    VARIABLE_CUSTOMER.scope.customers = res.data.customers;
    handlePaginateCustomer(+res.data.totalCustomers);
  });
}

function handlePopupCustomer() {
  setTemplatePopupCustomer();
  handlePopupCreateCustomer();
  handlePopupUpdateCustomer();
  handlePopupDeleteCustomer();
}

function setTemplatePopupCustomer() {
  VARIABLE_CUSTOMER.scope.templateCreate =
    window.location.origin + "/app/templates/customer/showCreate.html";
  VARIABLE_CUSTOMER.scope.templateUpdate =
    window.location.origin + "/app/templates/customer/showUpdate.html";
    VARIABLE_CUSTOMER.scope.templateDelete =
    window.location.origin + "/app/templates/customer/showDelete.html";
}

function handlePopupCreateCustomer() {
  VARIABLE_CUSTOMER.scope.showPopupCreate = false;

  VARIABLE_CUSTOMER.scope.createCustomer = createCustomer;

  VARIABLE_CUSTOMER.scope.openPopupCreate = function () {
    VARIABLE_CUSTOMER.scope.showPopupCreate = true;
  };

  VARIABLE_CUSTOMER.scope.closePopupCreate = function () {
    VARIABLE_CUSTOMER.scope.showPopupCreate = false;
  };
}

function createCustomer() {
  const customer = {};
  customer.Name = document.getElementById("name").value;
  customer.PhoneNumber = document.getElementById("phoneNumber").value;
  customer.Address = document.getElementById("address").value;

  VARIABLE_CUSTOMER.http.post("api/customer/create", customer).then((res) => {
    fetchCustomers();
  });
}

function handlePopupUpdateCustomer() {
  VARIABLE_CUSTOMER.scope.showPopupUpdate = false;

  VARIABLE_CUSTOMER.scope.updateCustomer = updateCustomer;

  VARIABLE_CUSTOMER.scope.openPopupUpdate = function (customerNow) {
    VARIABLE_CUSTOMER.scope.customerNow = customerNow;
    VARIABLE_CUSTOMER.scope.showPopupUpdate = true;
  };

  VARIABLE_CUSTOMER.scope.closePopupUpdate = function () {
    VARIABLE_CUSTOMER.scope.customerNow = null;
    VARIABLE_CUSTOMER.scope.showPopupUpdate = false;
  };
}

function updateCustomer() {
    const customer = VARIABLE_CUSTOMER.scope.customerNow;
    customer.Name = document.getElementById("name").value;
    customer.PhoneNumber = document.getElementById("phoneNumber").value;
    customer.Address = document.getElementById("address").value;

    VARIABLE_CUSTOMER.http.post(`api/customer/update/${customer.id}`, customer).then((res) => {
      fetchCustomers();
    });
}

function handlePopupDeleteCustomer() {
  VARIABLE_CUSTOMER.scope.showPopupDelete = false;

  VARIABLE_CUSTOMER.scope.deleteCustomer = deleteCustomer;

  VARIABLE_CUSTOMER.scope.openPopupDelete = function (customerNow) {
    VARIABLE_CUSTOMER.scope.customerNow = customerNow;
    VARIABLE_CUSTOMER.scope.showPopupDelete = true;
  };

  VARIABLE_CUSTOMER.scope.closePopupDelete = function () {
    VARIABLE_CUSTOMER.scope.customerNow = null;
    VARIABLE_CUSTOMER.scope.showPopupDelete = false;
  };
}

function deleteCustomer() {
    VARIABLE_CUSTOMER.http.post(`api/customer/delete/${VARIABLE_CUSTOMER.scope.customerNow.id}`).then((res) => {
      fetchCustomers();
    });
}
