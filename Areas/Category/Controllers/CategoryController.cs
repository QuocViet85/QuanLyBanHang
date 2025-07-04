using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.Category.ViewModel;
using WebBanHang.Data;

namespace WebBanHang.Areas.Category.Controllers;

[Area("Category")]
[Route("category")]
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

    public async Task<IActionResult> Index()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var categories = await _dbContext.Categories.Where(c => c.UserId == user.Id).ToListAsync();

            List<CategoryVM> categoryVMs = new List<CategoryVM>();

            if (categories.Count > 0)
            {
                categoryVMs = categories.Select(c => GetCategoryVMFromCategoryModel(c)).ToList();
            }
            return View(categoryVMs);
        }
        catch { }

        return null;
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var category = await _dbContext.Categories.Where(c => c.Id == id && c.UserId == user.Id).FirstOrDefaultAsync();

            if (category != null)
            {
                var categoryVM = GetCategoryVMFromCategoryModel(category);
                return View(categoryVM);
            }
        }
        catch { }

        return null;
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CategoryVM categoryVM)
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
            }
        }
        catch { }
        return RedirectToAction("Create");
    }

    [HttpGet("update/{id}")]
    public async Task<IActionResult> Update(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var categoryUpdate = await _dbContext.Categories.Where(c => c.Id == id && c.UserId == user.Id).FirstOrDefaultAsync();

            if (categoryUpdate != null)
            {
                var categoryVMUpdate = GetCategoryVMFromCategoryModel(categoryUpdate);
                return View(categoryVMUpdate);
            }
        }
        catch { }

        return null;
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, CategoryVM categoryVM)
    {
        try
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
            }
        }
        catch { }
        return RedirectToAction("Update", new { id = id });
    }

    [HttpGet("delete/{id}")]
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
            }
        }
        catch { }
        return RedirectToAction("Index");
    }

    private CategoryVM GetCategoryVMFromCategoryModel(CategoryModel category)
    {
        return new CategoryVM()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description
        };
    }
}