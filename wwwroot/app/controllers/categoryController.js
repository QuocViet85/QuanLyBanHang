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
  VARIABLE_CATEGORY.http.get(`api/category?pageNumber=${VARIABLE_CATEGORY.scope.paginate.pageNumber}&limit=${VARIABLE_CATEGORY.scope.paginate.limit}`).then(function (res) {
    VARIABLE_CATEGORY.scope.categories = res.data.categories;
    handlePaginateCategory(+res.data.totalCategories);
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
  console.log('CREATE CATEGORY');
  const category = {};
  category.Name = document.getElementById("name").value;
  category.Description = document.getElementById("description").value;

  VARIABLE_CATEGORY.http.post("api/category/create", category).then((res) => {
    fetchCategories();
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
    const category = VARIABLE_CATEGORY.scope.categoryNow;
    category.Name = document.getElementById("name").value;
    category.Description = document.getElementById("description").value;

    VARIABLE_CATEGORY.http.post(`api/category/update/${category.id}`, category).then((res) => {
      fetchCategories();
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
    VARIABLE_CATEGORY.http.post(`api/category/delete/${VARIABLE_CATEGORY.scope.categoryNow.id}`).then((res) => {
      fetchCategories();
    });
}