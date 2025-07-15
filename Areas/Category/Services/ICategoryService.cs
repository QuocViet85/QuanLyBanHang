using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.Category.ViewModel;

namespace WebBanHang.Areas.Category.Services;

public interface ICategoryService
{
    public Task<(List<CategoryVM> categoryVMs, int totalCategories)> GetCategories(int pageNumber, int limit, string userId, string searchByName);

    public Task Create(CategoryVM categoryVM, string userId);

    public Task Update(int id, CategoryVM categoryVM, string userId);

    public Task Delete(int[] ids, string userId);

    public CategoryVM GetCategoryVMFromCategoryModel(CategoryModel category);

    public CategoryModel CreateOrUpdateCategoryModelFromCategoryVM(CategoryVM categoryVM, string userId, CategoryModel categoryUpdate = null);
}