const VARIABLE_DYNAMIC_ATTRIBUTE = {};
app.controller("dynamicAttributeController", [
  "$scope",
  "$http",
  function ($scope, $http) {
    VARIABLE_DYNAMIC_ATTRIBUTE.scope = $scope;
    VARIABLE_DYNAMIC_ATTRIBUTE.http = $http;

    $scope.dynamicAttributes = [];
    $scope.dynamicAttributeNow = null;

    handlePaginateDynamicAttribute();
    fetchDynamicAttributes();
    handlePopupDynamicAttribute();
  },
]);

function handlePaginateDynamicAttribute(totalDynamicAttributes) {
    if (!totalDynamicAttributes) {
      VARIABLE_DYNAMIC_ATTRIBUTE.scope.paginate = {};
      VARIABLE_DYNAMIC_ATTRIBUTE.scope.paginate.pageNumber = 1;
      VARIABLE_DYNAMIC_ATTRIBUTE.scope.paginate.limit = 10;

      VARIABLE_DYNAMIC_ATTRIBUTE.scope.paginate.minusPageNumber = () => {
        if (VARIABLE_DYNAMIC_ATTRIBUTE.scope.paginate.pageNumber > 1) {
          VARIABLE_DYNAMIC_ATTRIBUTE.scope.paginate.pageNumber -= 1;
        }else {
          VARIABLE_DYNAMIC_ATTRIBUTE.scope.paginate.pageNumber = 1;
        }
        fetchDynamicAttributes();
      }

      VARIABLE_DYNAMIC_ATTRIBUTE.scope.paginate.plusPageNumber = () => {
        VARIABLE_DYNAMIC_ATTRIBUTE.scope.paginate.pageNumber += 1;
        fetchDynamicAttributes();
      }
    }else {
      VARIABLE_DYNAMIC_ATTRIBUTE.scope.paginate.totalPages = Math.ceil(totalDynamicAttributes / VARIABLE_DYNAMIC_ATTRIBUTE.scope.paginate.limit);
    }
}

function fetchDynamicAttributes() {
  VARIABLE_DYNAMIC_ATTRIBUTE.http.get(`api/dynamicattribute?pageNumber=${VARIABLE_DYNAMIC_ATTRIBUTE.scope.paginate.pageNumber}&limit=${VARIABLE_DYNAMIC_ATTRIBUTE.scope.paginate.limit}`).then(function (res) {
    VARIABLE_DYNAMIC_ATTRIBUTE.scope.dynamicAttributes = res.data.dynamicAttributes;
    handlePaginateDynamicAttribute(+res.data.totalDynamicAttributes);
  });
}

function handlePopupDynamicAttribute() {
  setTemplatePopupDynamicAttribute();
  handlePopupCreateDynamicAttribute();
  handlePopupUpdateDynamicAttribute();
  handlePopupDeleteDynamicAttribute();
}

function setTemplatePopupDynamicAttribute() {
  VARIABLE_DYNAMIC_ATTRIBUTE.scope.templateCreate =
    window.location.origin + "/app/templates/dynamicAttribute/showCreate.html";
  VARIABLE_DYNAMIC_ATTRIBUTE.scope.templateUpdate =
    window.location.origin + "/app/templates/dynamicAttribute/showUpdate.html";
    VARIABLE_DYNAMIC_ATTRIBUTE.scope.templateDelete =
    window.location.origin + "/app/templates/dynamicAttribute/showDelete.html";
}

function handlePopupCreateDynamicAttribute() {
  VARIABLE_DYNAMIC_ATTRIBUTE.scope.showPopupCreate = false;

  VARIABLE_DYNAMIC_ATTRIBUTE.scope.createDynamicAttribute = createDynamicAttribute;

  VARIABLE_DYNAMIC_ATTRIBUTE.scope.openPopupCreate = function () {
    VARIABLE_DYNAMIC_ATTRIBUTE.scope.showPopupCreate = true;
  };

  VARIABLE_DYNAMIC_ATTRIBUTE.scope.closePopupCreate = function () {
    VARIABLE_DYNAMIC_ATTRIBUTE.scope.showPopupCreate = false;
  };
}

function createDynamicAttribute() {
  const dynamicAttribute = {};
  dynamicAttribute.Name = document.getElementById("name").value;

  VARIABLE_DYNAMIC_ATTRIBUTE.http.post("api/dynamicattribute/create", dynamicAttribute).then((res) => {
    fetchDynamicAttributes();
  });
}

function handlePopupUpdateDynamicAttribute() {
  VARIABLE_DYNAMIC_ATTRIBUTE.scope.showPopupUpdate = false;

  VARIABLE_DYNAMIC_ATTRIBUTE.scope.updateDynamicAttribute = updateDynamicAttribute;

  VARIABLE_DYNAMIC_ATTRIBUTE.scope.openPopupUpdate = function (dynamicAttributeNow) {
    VARIABLE_DYNAMIC_ATTRIBUTE.scope.dynamicAttributeNow = dynamicAttributeNow;
    VARIABLE_DYNAMIC_ATTRIBUTE.scope.showPopupUpdate = true;
  };

  VARIABLE_DYNAMIC_ATTRIBUTE.scope.closePopupUpdate = function () {
    VARIABLE_DYNAMIC_ATTRIBUTE.scope.dynamicAttributeNow = null;
    VARIABLE_DYNAMIC_ATTRIBUTE.scope.showPopupUpdate = false;
  };
}

function updateDynamicAttribute() {
    const dynamicAttribute = VARIABLE_DYNAMIC_ATTRIBUTE.scope.dynamicAttributeNow;
    dynamicAttribute.Name = document.getElementById("name").value;

    VARIABLE_DYNAMIC_ATTRIBUTE.http.post(`api/dynamicAttribute/update/${dynamicAttribute.id}`, dynamicAttribute).then((res) => {
      fetchDynamicAttributes();
    });
}

function handlePopupDeleteDynamicAttribute() {
  VARIABLE_DYNAMIC_ATTRIBUTE.scope.showPopupDelete = false;

  VARIABLE_DYNAMIC_ATTRIBUTE.scope.deleteDynamicAttribute = deleteDynamicAttribute;

  VARIABLE_DYNAMIC_ATTRIBUTE.scope.openPopupDelete = function (dynamicAttributeNow) {
    VARIABLE_DYNAMIC_ATTRIBUTE.scope.dynamicAttributeNow = dynamicAttributeNow;
    VARIABLE_DYNAMIC_ATTRIBUTE.scope.showPopupDelete = true;
  };

  VARIABLE_DYNAMIC_ATTRIBUTE.scope.closePopupDelete = function () {
    VARIABLE_DYNAMIC_ATTRIBUTE.scope.dynamicAttributeNow = null;
    VARIABLE_DYNAMIC_ATTRIBUTE.scope.showPopupDelete = false;
  };
}

function deleteDynamicAttribute() {
    VARIABLE_DYNAMIC_ATTRIBUTE.http.post(`api/dynamicAttribute/delete/${VARIABLE_DYNAMIC_ATTRIBUTE.scope.dynamicAttributeNow.id}`).then((res) => {
      fetchDynamicAttributes();
    });
}