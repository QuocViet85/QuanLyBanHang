using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.Category.ViewModel;

namespace WebBanHang.Areas.Category.Services;

public interface ICategoryService
{
    public Task<(List<CategoryVM> categoryVMs, int totalCategories)> GetCategories(int pageNumber, int limit, string userId);

    public Task Create(CategoryVM categoryVM, string userId);

    public Task Update(int id, CategoryVM categoryVM, string userId);

    public Task Delete(int id, string userId);

    public CategoryVM GetCategoryVMFromCategoryModel(CategoryModel category);
}