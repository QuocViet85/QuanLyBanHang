const VARIABLE_CATEGORY = {};

app.controller("categoryController", [
  "$scope",
  "$http",
  function ($scope, $http) {
    console.log($scope);
    VARIABLE_CATEGORY.scope = $scope;
    VARIABLE_CATEGORY.http = $http;
    $scope.categories = [];
    $scope.categoryNow = null;

    $scope.categoryIdChecks = [];

    $scope.searchByName = '';

    $scope.message = {}
    $scope.message.success = false;
    $scope.message.error = false;

    $scope.fetchCategories = fetchCategories;
    
    handlePaginateCategory();
    fetchCategories();

    handlePopupCategory();
  },
]);

function handlePaginateCategory(totalCategories) {
    if (!totalCategories) {
      VARIABLE_CATEGORY.scope.paginate = {};
      VARIABLE_CATEGORY.scope.paginate.pageNumber = 1;
      VARIABLE_CATEGORY.scope.paginate.limit = 10;

      VARIABLE_CATEGORY.scope.paginate.minusPageNumber = () => {
        if (VARIABLE_CATEGORY.scope.paginate.pageNumber > 1) {
          VARIABLE_CATEGORY.scope.paginate.pageNumber -= 1;
        }else {
          VARIABLE_CATEGORY.scope.paginate.pageNumber = 1;
        }
        fetchCategories();
      }

      VARIABLE_CATEGORY.scope.paginate.plusPageNumber = () => {
        VARIABLE_CATEGORY.scope.paginate.pageNumber += 1;
        fetchCategories();
      }
    }else {
      VARIABLE_CATEGORY.scope.paginate.totalPages = Math.ceil(totalCategories / VARIABLE_CATEGORY.scope.paginate.limit);
    }
}

function fetchCategories() {
  VARIABLE_CATEGORY.http.get(`api/category?pageNumber=${VARIABLE_CATEGORY.scope.paginate.pageNumber}&limit=${VARIABLE_CATEGORY.scope.paginate.limit}&searchByName=${VARIABLE_CATEGORY.scope.searchByName}`).then(function (res) {

    console.log(`api/category?pageNumber=${VARIABLE_CATEGORY.scope.paginate.pageNumber}&limit=${VARIABLE_CATEGORY.scope.paginate.limit}&searchByName=${VARIABLE_CATEGORY.scope.searchByName}`)
    if (!res.data.categories) {
      throw "Không tìm thấy danh mục sản phẩm";
    }
    VARIABLE_CATEGORY.scope.categories = res.data.categories;
    handlePaginateCategory(+res.data.totalCategories);
  })
  .catch(err => {
    showMessage(VARIABLE_CATEGORY.scope, err, false);
  });
}

function handlePopupCategory() {
  setTemplatePopupCategory();
  handlePopupDescriptionCategory();
  handlePopupCreateCategory();
  handlePopupUpdateCategory();
  handlePopupDeleteCategory();
}

function setTemplatePopupCategory() {
  VARIABLE_CATEGORY.scope.templateDescription =
    window.location.origin + "/app/templates/category/showDescription.html";
  VARIABLE_CATEGORY.scope.templateCreate =
    window.location.origin + "/app/templates/category/showCreate.html";
    VARIABLE_CATEGORY.scope.templateUpdate =
    window.location.origin + "/app/templates/category/showUpdate.html";
    VARIABLE_CATEGORY.scope.templateDelete =
    window.location.origin + "/app/templates/category/showDelete.html";
    VARIABLE_CATEGORY.scope.templateMessage = 
    window.location.origin + "/app/templates/showMessage.html";
}

function handlePopupDescriptionCategory() {
  VARIABLE_CATEGORY.scope.description = null;
  VARIABLE_CATEGORY.scope.showPopupDescription = false;

  VARIABLE_CATEGORY.scope.openPopupDescription = function (categoryNow) {
    VARIABLE_CATEGORY.scope.categoryNow = categoryNow;
    VARIABLE_CATEGORY.scope.showPopupDescription = true;
  };

  VARIABLE_CATEGORY.scope.closePopupDescription = function () {
    VARIABLE_CATEGORY.scope.description = null;
    VARIABLE_CATEGORY.scope.showPopupDescription = false;
  };
}

function handlePopupCreateCategory() {
  VARIABLE_CATEGORY.scope.showPopupCreate = false;

  VARIABLE_CATEGORY.scope.createCategory = createCategory;

  VARIABLE_CATEGORY.scope.openPopupCreate = function () {
    VARIABLE_CATEGORY.scope.showPopupCreate = true;
  };

  VARIABLE_CATEGORY.scope.closePopupCreate = function () {
    VARIABLE_CATEGORY.scope.showPopupCreate = false;
  };
}

function createCategory() {
  const category = {};
  category.Name = document.getElementById("nameCategory").value;
  category.Code = document.getElementById('codeCategory').value;
  category.Description = document.getElementById("descriptionCategory").value;

  VARIABLE_CATEGORY.http.post("api/category/create", category).then((res) => {
    fetchCategories();
    showMessage(VARIABLE_CATEGORY.scope, res.data, true);
  })
  .catch(err => {
    showMessage(VARIABLE_CATEGORY.scope, err.data, false);
  });
}

function handlePopupUpdateCategory() {
  VARIABLE_CATEGORY.scope.showPopupUpdate = false;

  VARIABLE_CATEGORY.scope.updateCategory = updateCategory;

  VARIABLE_CATEGORY.scope.openPopupUpdate = function (categoryNow) {
    VARIABLE_CATEGORY.scope.categoryNow = categoryNow;
    VARIABLE_CATEGORY.scope.showPopupUpdate = true;
  };

  VARIABLE_CATEGORY.scope.closePopupUpdate = function () {
    VARIABLE_CATEGORY.scope.categoryNow = null;
    VARIABLE_CATEGORY.scope.showPopupUpdate = false;
  };
}

function updateCategory() {
    const category = {}
    category.Name = document.getElementById("nameCategory").value;
    category.Code = document.getElementById('codeCategory').value;
    category.Description = document.getElementById("descriptionCategory").value;

    VARIABLE_CATEGORY.http.post(`api/category/update/${VARIABLE_CATEGORY.scope.categoryNow.id}`, category).then((res) => {
      fetchCategories();
      showMessage(VARIABLE_CATEGORY.scope, res.data, true);
    })
    .catch(err => {
      showMessage(VARIABLE_CATEGORY.scope, err.data, false);
    });
}

function handlePopupDeleteCategory() {
  VARIABLE_CATEGORY.scope.showPopupDelete = false;

  VARIABLE_CATEGORY.scope.deleteCategory = deleteCategory;

  VARIABLE_CATEGORY.scope.openPopupDelete = function (categoryNow) {
    VARIABLE_CATEGORY.scope.categoryNow = categoryNow;
    VARIABLE_CATEGORY.scope.showPopupDelete = true;
  };

  VARIABLE_CATEGORY.scope.closePopupDelete = function () {
    VARIABLE_CATEGORY.scope.categoryNow = null;
    VARIABLE_CATEGORY.scope.showPopupDelete = false;
  };
}

function deleteCategory() {
    VARIABLE_CATEGORY.http.post(`api/category/delete`, VARIABLE_CATEGORY.scope.categoryIdChecks).then((res) => {
      fetchCategories();
      showMessage(VARIABLE_CATEGORY.scope, res.data, true);
    })
    .catch(err => {
      showMessage(VARIABLE_CATEGORY.scope, err.data, false);
    });
}

function checkAllCategories() {
  const checkBoxAllCategory = document.getElementById('checkBoxAllCategory');

  const checkBoxCategories = document.getElementsByClassName('checkboxCategory');

  VARIABLE_CATEGORY.scope.categoryIdChecks = [];

  if (checkBoxAllCategory.checked) {
      for (const checkBoxCategory of checkBoxCategories) {
      checkBoxCategory.checked = checkBoxAllCategory.checked;
      VARIABLE_CATEGORY.scope.categoryIdChecks.push(parseInt(checkBoxCategory.id.slice('5')));
    }
  }else {
    for (const checkBoxCategory of checkBoxCategories) {
      checkBoxCategory.checked = checkBoxAllCategory.checked;
    }
  }
}

function setCategoryCheck(element) {
  const categoryId = parseInt(element.id.slice('5'));
  if (element.checked) {
    VARIABLE_CATEGORY.scope.categoryIdChecks.push(categoryId);
  }else {
    const index = VARIABLE_CATEGORY.scope.categoryIdChecks.indexOf(categoryId);
    if (index !== -1) {
      VARIABLE_CATEGORY.scope.categoryIdChecks.splice(index);
    }
  }
}