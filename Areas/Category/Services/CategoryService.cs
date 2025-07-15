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

    public async Task<(List<CategoryVM> categoryVMs, int totalCategories)> GetCategories(int pageNumber, int limit, string userId, string searchByName)
    {
        List<CategoryModel> categories;

        IQueryable<CategoryModel> queryCategories = _dbContext.Categories.Where(c => c.UserId == userId);

        if (pageNumber > 0 && limit > 0)
        {
            queryCategories = queryCategories.Skip((pageNumber - 1) * limit).Take(limit);
        }
        else if (limit < 0)
        {
            throw new Exception("Lỗi phân trang");
        }

        if (!string.IsNullOrEmpty(searchByName))
        {
            queryCategories = queryCategories.Where(c => c.Name.Contains(searchByName));
        }

        categories = await queryCategories.ToListAsync();

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

    public async Task Delete(int[] ids, string userId)
    {
        if (ids != null)
        {
            foreach (var id in ids)
            {
                bool exist = await _dbContext.Products.AnyAsync(p => p.Id == id && p.UserId == userId);
                if (exist)
                {
                    await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Categories WHERE id = {0}", id);
                }
            }
        }
        else
        {
            throw new Exception("Chưa chọn sản phẩm để xóa");
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