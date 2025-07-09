using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client.Extensibility;
using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.Category.ViewModel;
using WebBanHang.Data;

namespace WebBanHang.Areas.Category.Controllers;

[Area("Category")]
[Route("api/category")]
[Authorize]
public class CategoryController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;

    public CategoryController(ApplicationDbContext dbContext, UserManager<IdentityUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int pageNumber, int limit)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            List<CategoryModel> categories;

            if (pageNumber > 0 && limit > 0)
            {
                categories = await _dbContext.Categories.Where(c => c.UserId == user.Id)
                                                        .Skip((pageNumber - 1) * limit)
                                                        .Take(limit)
                                                        .ToListAsync(); 
            }
            else
            {
                categories = await _dbContext.Categories.Where(c => c.UserId == user.Id).ToListAsync();
            }

            int totalCategories = await _dbContext.Categories.CountAsync();

            List<CategoryVM> categoryVMs = new List<CategoryVM>();

            if (categories?.Count > 0)
            {
                categoryVMs = categories.Select(c => GetCategoryVMFromCategoryModel(c)).ToList();
            }
            return Ok(new
            {
                categories = categoryVMs,
                totalCategories = totalCategories
            });
        }
        catch { throw new Exception("Lấy danh mục sản phẩm thất bại"); }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CategoryVM categoryVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var categoryModel = new CategoryModel()
                {
                    Name = categoryVM.Name,
                    Description = categoryVM.Description,
                    UserId = user.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                await _dbContext.Categories.AddAsync(categoryModel);
                await _dbContext.SaveChangesAsync();

                return Ok("Tạo danh mục sản phẩm thành công");
            }
            else
            {
                return BadRequest("Thông tin nhập vào không hợp lệ");
            }
        }
        catch { return BadRequest("Tạo danh mục sản phẩm thất bại"); }
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryVM categoryVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                CategoryModel categoryUpdate = await _dbContext.Categories.Where(c => c.Id == id && c.UserId == user.Id).FirstOrDefaultAsync();

                if (categoryUpdate != null)
                {
                    categoryUpdate.Name = categoryVM.Name;
                    categoryUpdate.Description = categoryVM.Description;
                    categoryUpdate.UpdatedAt = DateTime.Now;

                    _dbContext.Categories.Update(categoryUpdate);
                    int result = await _dbContext.SaveChangesAsync();

                    return Ok("Cập nhật danh mục sản phẩm thành công");
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                return BadRequest("Thông tin nhập vào không hợp lệ");
            }
            
        }
        catch { return BadRequest("Cập nhật danh mục sản phẩm thất bại"); }
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            CategoryModel categoryDelete = await _dbContext.Categories.Where(c => c.Id == id && c.UserId == user.Id).FirstOrDefaultAsync();
            if (categoryDelete != null)
            {
                _dbContext.Categories.Remove(categoryDelete);
                int result = await _dbContext.SaveChangesAsync();

                return Ok("Xóa danh mục sản phẩm thành công");
            }
            else
            {
                throw new Exception();
            }
        }
        catch { return BadRequest("Xóa danh mục sản phẩm thất bại"); }
    }

    private CategoryVM GetCategoryVMFromCategoryModel(CategoryModel category)
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