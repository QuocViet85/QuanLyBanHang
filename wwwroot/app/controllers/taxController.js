const VARIABLE_TAX = {};
app.controller("taxController", [
  "$scope",
  "$http",
  function ($scope, $http) {
    VARIABLE_TAX.scope = $scope;
    VARIABLE_TAX.http = $http;
    $scope.taxes = [];
    $scope.taxNow = null;

    handlePaginateTax();
    fetchTaxes();
    handlePopupTax();
  },
]);

function handlePaginateTax(totalTaxes) {
    if (!totalTaxes) {
      VARIABLE_TAX.scope.paginate = {};
      VARIABLE_TAX.scope.paginate.pageNumber = 1;
      VARIABLE_TAX.scope.paginate.limit = 10;

      VARIABLE_TAX.scope.paginate.minusPageNumber = () => {
        if (VARIABLE_TAX.scope.paginate.pageNumber > 1) {
          VARIABLE_TAX.scope.paginate.pageNumber -= 1;
        }else {
          VARIABLE_TAX.scope.paginate.pageNumber = 1;
        }
        fetchTaxes();
      }

      VARIABLE_TAX.scope.paginate.plusPageNumber = () => {
        VARIABLE_TAX.scope.paginate.pageNumber += 1;
        fetchTaxes();
      }
    }else {
      VARIABLE_TAX.scope.paginate.totalPages = Math.ceil(totalTaxes / VARIABLE_TAX.scope.paginate.limit);
    }
}

function fetchTaxes() {
  VARIABLE_TAX.http.get(`api/tax?pageNumber=${VARIABLE_TAX.scope.paginate.pageNumber}&limit=${VARIABLE_TAX.scope.paginate.limit}`).then(function (res) {
    VARIABLE_TAX.scope.taxes = res.data.taxes;
    handlePaginateTax(+res.data.totalTaxes);
  });
}

function handlePopupTax() {
  setTemplatePopupTax();
  handlePopupDescriptionTax();
  handlePopupCreateTax();
  handlePopupUpdateTax();
  handlePopupDeleteTax();
}

function setTemplatePopupTax() {
  VARIABLE_TAX.scope.templateDescription =
    window.location.origin + "/app/templates/tax/showDescription.html";
  VARIABLE_TAX.scope.templateCreate =
    window.location.origin + "/app/templates/tax/showCreate.html";
  VARIABLE_TAX.scope.templateUpdate =
    window.location.origin + "/app/templates/tax/showUpdate.html";
    VARIABLE_TAX.scope.templateDelete =
    window.location.origin + "/app/templates/tax/showDelete.html";
}

function handlePopupDescriptionTax() {
  VARIABLE_TAX.scope.description = null;
  VARIABLE_TAX.scope.showPopupDescription = false;

  VARIABLE_TAX.scope.openPopupDescription = function (taxNow) {
    VARIABLE_TAX.scope.taxNow = taxNow;
    VARIABLE_TAX.scope.showPopupDescription = true;
  };

  VARIABLE_TAX.scope.closePopupDescription = function () {
    VARIABLE_TAX.scope.description = null;
    VARIABLE_TAX.scope.showPopupDescription = false;
  };
}

function handlePopupCreateTax() {
  VARIABLE_TAX.scope.showPopupCreate = false;

  VARIABLE_TAX.scope.createTax = createTax;

  VARIABLE_TAX.scope.openPopupCreate = function () {
    VARIABLE_TAX.scope.showPopupCreate = true;
  };

  VARIABLE_TAX.scope.closePopupCreate = function () {
    VARIABLE_TAX.scope.showPopupCreate = false;
  };
}

function createTax() {
  const tax = {};
  tax.Name = document.getElementById("name").value;
  tax.Rate = document.getElementById("rate").value / 100;
  tax.Code = document.getElementById("code").value;
  tax.IsActive = document.getElementById("isActive").checked;
  tax.IsDefault = document.getElementById("isDefault").checked;
  tax.Description = document.getElementById("description").value;

  VARIABLE_TAX.http.post("api/tax/create", tax).then((res) => {
    fetchTaxes();
  });
}

function handlePopupUpdateTax() {
  VARIABLE_TAX.scope.showPopupUpdate = false;

  VARIABLE_TAX.scope.updateTax = updateTax;

  VARIABLE_TAX.scope.openPopupUpdate = function (taxNow) {
    VARIABLE_TAX.scope.taxNow = taxNow;
    VARIABLE_TAX.scope.showPopupUpdate = true;
  };

  VARIABLE_TAX.scope.closePopupUpdate = function () {
    VARIABLE_TAX.scope.taxNow = null;
    VARIABLE_TAX.scope.showPopupUpdate = false;
  };
}

function updateTax() {
    const tax = VARIABLE_TAX.scope.taxNow;
    tax.Name = document.getElementById("name").value;
    tax.Rate = document.getElementById("rate").value / 100;
    tax.Code = document.getElementById("code").value;
    tax.IsActive = document.getElementById("isActive").checked;
    tax.IsDefault = document.getElementById("isDefault").checked;
    tax.Description = document.getElementById("description").value;

    VARIABLE_TAX.http.post(`api/tax/update/${tax.id}`, tax).then((res) => {
      fetchTaxes();
    });
}

function handlePopupDeleteTax() {
  VARIABLE_TAX.scope.showPopupDelete = false;

  VARIABLE_TAX.scope.deleteTax = deleteTax;

  VARIABLE_TAX.scope.openPopupDelete = function (taxNow) {
    VARIABLE_TAX.scope.taxNow = taxNow;
    VARIABLE_TAX.scope.showPopupDelete = true;
  };

  VARIABLE_TAX.scope.closePopupDelete = function () {
    VARIABLE_TAX.scope.taxNow = null;
    VARIABLE_TAX.scope.showPopupDelete = false;
  };
}

function deleteTax() {
    VARIABLE_TAX.http.post(`api/tax/delete/${VARIABLE_TAX.scope.taxNow.id}`).then((res) => {
      fetchTaxes();
    });
}