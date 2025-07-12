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
        var categoryModel = new CategoryModel()
        {
            Name = categoryVM.Name,
            Description = categoryVM.Description,
            UserId = userId,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _dbContext.Categories.AddAsync(categoryModel);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Update(int id, CategoryVM categoryVM, string userId)
    {
        CategoryModel categoryUpdate = await _dbContext.Categories.Where(c => c.Id == id && c.UserId == userId).FirstOrDefaultAsync();

        if (categoryUpdate != null)
        {
            categoryUpdate.Name = categoryVM.Name;
            categoryUpdate.Description = categoryVM.Description;
            categoryUpdate.UpdatedAt = DateTime.Now;

            _dbContext.Categories.Update(categoryUpdate);
            int result = await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception();
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
            throw new Exception();
        }
    }

    public CategoryVM GetCategoryVMFromCategoryModel(CategoryModel category)
    {
        return new CategoryVM()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
            UpdatedAt = category.UpdatedAt
        };
    }
}