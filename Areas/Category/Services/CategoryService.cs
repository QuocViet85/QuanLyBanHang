using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.Category.ViewModel;
using WebBanHang.Data;

namespace WebBanHang.Areas.Category.Services;

public class CategoryService : ICategoryService
{
    private readonly ApplicationDbContext _dbContext;

    public CategoryService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(List<CategoryVM> categoryVMs, int totalCategories)> GetCategories(int pageNumber, int limit, string userId)
    {
        List<CategoryModel> categories;

        if (pageNumber > 0 && limit > 0)
        {
            categories = await _dbContext.Categories.Where(c => c.UserId == userId)
                                                    .Skip((pageNumber - 1) * limit)
                                                    .Take(limit)
                                                    .ToListAsync();
        }
        else
        {
            categories = await _dbContext.Categories.Where(c => c.UserId == userId).ToListAsync();
        }

        int totalCategories = await _dbContext.Categories.CountAsync();

        List<CategoryVM> categoryVMs = new List<CategoryVM>();

        if (categories?.Count > 0)
        {
            categoryVMs = categories.Select(c => GetCategoryVMFromCategoryModel(c)).ToList();
        }

        return (categoryVMs, totalCategories);
    }
    public async Task Create(CategoryVM categoryVM, string userId)
    {
        bool existCode = await _dbContext.Categories.AnyAsync(c => c.Code == categoryVM.Code && c.UserId == userId);

        if (existCode)
        {
            throw new Exception("Đã có danh mục có mã code này");
        }

        var categoryModel = CreateOrUpdateCategoryModelFromCategoryVM(categoryVM, userId);

        await _dbContext.Categories.AddAsync(categoryModel);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(int id, CategoryVM categoryVM, string userId)
    {
        CategoryModel categoryUpdate = await _dbContext.Categories.Where(c => c.Id == id && c.UserId == userId).FirstOrDefaultAsync();

        if (categoryUpdate != null)
        {
            bool existCode = await _dbContext.Categories.AnyAsync(c => c.Code == categoryVM.Code && c.Id != categoryUpdate.Id && c.UserId == userId);

            if (existCode)
            {
                throw new Exception("Đã có danh mục có mã code này");
            }

            categoryUpdate = CreateOrUpdateCategoryModelFromCategoryVM(categoryVM, userId, categoryUpdate);

            _dbContext.Categories.Update(categoryUpdate);
            int result = await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Không tìm thấy danh mục sản phẩm");
        }
    }

    public async Task Delete(int id, string userId)
    {
        CategoryModel categoryDelete = await _dbContext.Categories.Where(c => c.Id == id && c.UserId == userId).FirstOrDefaultAsync();
        if (categoryDelete != null)
        {
            _dbContext.Categories.Remove(categoryDelete);
            int result = await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Không tìm thấy danh mục sản phẩm");
        }
    }

    public CategoryVM GetCategoryVMFromCategoryModel(CategoryModel category)
    {
        return new CategoryVM()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            Code = category.Code
        };
    }
    
    public CategoryModel CreateOrUpdateCategoryModelFromCategoryVM(CategoryVM categoryVM, string userId, CategoryModel categoryUpdate = null)
    {
        if (categoryUpdate == null)
        {
            var category = new CategoryModel();

            category.Name = categoryVM.Name;
            category.Description = categoryVM.Description;
            category.Code = categoryVM.Code;
            category.UserId = userId;
            category.CreatedAt = DateTime.Now;
            category.UpdatedAt = DateTime.Now;

            return category;
        }
        else
        {
            categoryUpdate.Name = categoryVM.Name;
            categoryUpdate.Description = categoryVM.Description;
            categoryUpdate.Code = categoryVM.Code;

            categoryUpdate.UpdatedAt = DateTime.Now;

            return categoryUpdate;
        }
    }
}