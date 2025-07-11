const VARIABLE_ORDER = {};
app.controller("orderController", [
  "$scope",
  "$http",
  function ($scope, $http) {
    VARIABLE_ORDER.scope = $scope;
    VARIABLE_ORDER.http = $http;

    $scope.orders = [];
    $scope.orderNow = null;

    handlePaginateOrder();
    fetchOrders();
    handlePopupOrder();
    handlePopupDeleteOrder();
  },
]);

function handlePaginateOrder(totalOrders) {
    if (!totalOrders) {
      VARIABLE_ORDER.scope.paginate = {};
      VARIABLE_ORDER.scope.paginate.pageNumber = 1;
      VARIABLE_ORDER.scope.paginate.limit = 10;

      VARIABLE_ORDER.scope.paginate.minusPageNumber = () => {
        if (VARIABLE_ORDER.scope.paginate.pageNumber > 1) {
          VARIABLE_ORDER.scope.paginate.pageNumber -= 1;
        }else {
          VARIABLE_ORDER.scope.paginate.pageNumber = 1;
        }
        fetchOrders();
      }

      VARIABLE_ORDER.scope.paginate.plusPageNumber = () => {
        VARIABLE_ORDER.scope.paginate.pageNumber += 1;
        fetchOrders();
      }
    }else {
      VARIABLE_ORDER.scope.paginate.totalPages = Math.ceil(totalOrders / VARIABLE_ORDER.scope.paginate.limit);
    }
}

function fetchOrders() {
  VARIABLE_ORDER.http.get(`api/order?pageNumber=${VARIABLE_ORDER.scope.paginate.pageNumber}&limit=${VARIABLE_ORDER.scope.paginate.limit}`).then(function (res) {
    VARIABLE_ORDER.scope.orders = res.data.orders;
    handlePaginateOrder(+res.data.totalOrders);
  });
}

function handlePopupOrder() {
  setTemplatePopupOrder();
  handlePopupDetailOrder();
  handlePopupDestroyOrder();
  handlePopupCompletedOrder();
}

function setTemplatePopupOrder() {
  VARIABLE_ORDER.scope.templateDetail =
    window.location.origin + "/app/templates/order/showDetail.html";
    VARIABLE_ORDER.scope.templateCompleted =
    window.location.origin + "/app/templates/order/showCompleted.html";
    VARIABLE_ORDER.scope.templateDestroy =
    window.location.origin + "/app/templates/order/showDestroy.html";
    VARIABLE_ORDER.scope.templateDelete =
    window.location.origin + "/app/templates/order/showDelete.html";
}

function handlePopupDetailOrder() {
  VARIABLE_ORDER.scope.detail = null;
  VARIABLE_ORDER.scope.showPopupDetail = false;

  VARIABLE_ORDER.scope.openPopupDetail = function (orderNow) {
    VARIABLE_ORDER.scope.orderNow = orderNow;
    VARIABLE_ORDER.scope.showPopupDetail = true;
  };

  VARIABLE_ORDER.scope.closePopupDetail = function () {
    VARIABLE_ORDER.scope.orderNow = null;
    VARIABLE_ORDER.scope.showPopupDetail = false;
  };
}

function handlePopupDestroyOrder() {
  VARIABLE_ORDER.scope.showPopupDestroy = false;

  VARIABLE_ORDER.scope.destroyOrder = destroyOrder;

  VARIABLE_ORDER.scope.openPopupDestroy = function (orderNow) {
    VARIABLE_ORDER.scope.orderNow = orderNow;
    VARIABLE_ORDER.scope.showPopupDestroy = true;
  };

  VARIABLE_ORDER.scope.closePopupDestroy = function () {
    VARIABLE_ORDER.scope.orderNow = null;
    VARIABLE_ORDER.scope.showPopupDestroy = false;
  };
}

function destroyOrder() {
    VARIABLE_ORDER.http.post(`api/order/destroy/${VARIABLE_ORDER.scope.orderNow.id}`).then((res) => {
      fetchOrders();
    });
}

function handlePopupDeleteOrder() {
  VARIABLE_ORDER.scope.showPopupDelete = false;

  VARIABLE_ORDER.scope.deleteOrder = deleteOrder;

  VARIABLE_ORDER.scope.openPopupDelete = function (orderNow) {
    console.log('delete')
    VARIABLE_ORDER.scope.orderNow = orderNow;
    VARIABLE_ORDER.scope.showPopupDelete = true;
  };

  VARIABLE_ORDER.scope.closePopupDelete = function () {
    VARIABLE_ORDER.scope.orderNow = null;
    VARIABLE_ORDER.scope.showPopupDelete = false;
  };
}

function deleteOrder() {
    VARIABLE_ORDER.http.post(`api/order/delete/${VARIABLE_ORDER.scope.orderNow.id}`).then((res) => {
      fetchOrders();
    });
}

function handlePopupCompletedOrder() {
  VARIABLE_ORDER.scope.showPopupCompleted = false;

  VARIABLE_ORDER.scope.completedOrder = completedOrder;

  VARIABLE_ORDER.scope.openPopupCompleted = function (orderNow) {
    VARIABLE_ORDER.scope.orderNow = orderNow;
    VARIABLE_ORDER.scope.showPopupCompleted = true;
  };

  VARIABLE_ORDER.scope.closePopupCompleted = function () {
    VARIABLE_ORDER.scope.orderNow = null;
    VARIABLE_ORDER.scope.showPopupCompleted = false;
  };
}

function completedOrder() {
    VARIABLE_ORDER.http.post(`api/order/completed-order/${VARIABLE_ORDER.scope.orderNow.id}`, true).then((res) => {
      fetchOrders();
    });
}