using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Areas.Category.Services;
using WebBanHang.Areas.Category.ViewModel;

namespace WebBanHang.Areas.Category.Controllers;

[Area("Category")]
[Route("api/category")]
[Authorize]
public class CategoryController : Controller
{
    private readonly ICategoryService _categoryService;
    private readonly UserManager<IdentityUser> _userManager;


    public CategoryController(ICategoryService categoryService, UserManager<IdentityUser> userManager)
    {
        _categoryService = categoryService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int pageNumber, int limit, string searchByName)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var result = await _categoryService.GetCategories(pageNumber, limit, user.Id, searchByName);
            return Ok(new
            {
                categories = result.categoryVMs,
                totalCategories = result.totalCategories
            });
        }
        catch { throw; }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CategoryVM categoryVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                await _categoryService.Create(categoryVM, user.Id);

                return Ok("Tạo danh mục sản phẩm thành công");
            }
            else
            {
                return BadRequest("Thông tin nhập vào không hợp lệ");
            }
        }
        catch { throw; }
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryVM categoryVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                await _categoryService.Update(id, categoryVM, user.Id);

                return Ok("Cập nhật danh mục sản phẩm thành công");
            }
            else
            {
                return BadRequest("Thông tin nhập vào không hợp lệ");
            }
            
        }
        catch { throw; }
    }

    [HttpPost("delete")]
    public async Task<IActionResult> Delete([FromBody] int[] ids)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            await _categoryService.Delete(ids, user.Id);

            return Ok("Xóa danh mục sản phẩm thành công");
        }
        catch { throw; }
    }
}