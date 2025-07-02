using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.Category.ViewModel;
using WebBanHang.Data;

namespace WebBanHang.Areas.Category.Controllers;

[Area("Category")]
[Route("category")]
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
        var user = await _userManager.GetUserAsync(User);

        var categories = await _dbContext.Categories.Where(c => c.UserId == user.Id).ToListAsync();

        return View(categories);
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var category = await _dbContext.Categories.Where(c => c.Id == id && c.UserId == user.Id).FirstAsync();

        return View(category);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CategoryVM categoryVM)
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

            return RedirectToAction("Create");
        }
        return View();
    }

    [HttpGet("update/{id}")]
    public async Task<IActionResult> Update(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var categoryUpdate = await _dbContext.Categories.Where(c => c.Id == id && c.UserId == user.Id).FirstOrDefaultAsync();

        var categoryVMUpdate = new CategoryVM()
        {
            Name = categoryUpdate.Name,
            Description = categoryUpdate.Description,
        };

        return View(categoryVMUpdate);
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, CategoryVM categoryVM)
    {
        var user = await _userManager.GetUserAsync(User);

        try
        {
            CategoryModel categoryUpdate = await _dbContext.Categories.Where(c => c.Id == id && c.UserId == user.Id).FirstAsync();

            if (categoryUpdate != null)
            {
                categoryUpdate.Name = categoryVM.Name;
                categoryUpdate.Description = categoryVM.Description;
                categoryUpdate.UpdatedAt = DateTime.Now;

                _dbContext.Categories.Update(categoryUpdate);
                int result = await _dbContext.SaveChangesAsync();

                return RedirectToAction("Update", new { id = id });
            }
            return Redirect("Index");
        }
        catch
        {
            return Redirect("Index");
        }
    }

    [HttpGet("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        try
        {
            CategoryModel categoryDelete = await _dbContext.Categories.Where(c => c.Id == id && c.UserId == user.Id).FirstAsync();
            if (categoryDelete != null)
            {
                _dbContext.Categories.Remove(categoryDelete);
                int result = await _dbContext.SaveChangesAsync();
            }
        }
        catch { }
        return RedirectToAction("Index");
    }
}