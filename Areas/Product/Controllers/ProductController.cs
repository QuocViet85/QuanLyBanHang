using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Areas.Product.Services;
using WebBanHang.Areas.Product.ViewModel;


namespace WebBanHang.Areas.Product.Controllers;

[Area("Product")]
[Route("api/product")]
[Authorize]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly UserManager<IdentityUser> _userManager;
    public ProductController(UserManager<IdentityUser> userManager, IProductService productService)
    {
        _productService = productService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int pageNumber, int limit, string searchByName, string searchByCode, string searchByCategory, string searchByUnit)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _productService.GetProducts(pageNumber, limit, user.Id, searchByName, searchByCode, searchByCategory, searchByUnit);
            return Ok(new
            {
                products = result.productVMs,
                totalProducts = result.totalProducts
            });
        }
        catch
        { throw; }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ProductVM productVM)
    {
        foreach (var key in ModelState.Keys)
        {
            var vals = ModelState[key];
            if (vals != null)
            {
                foreach (var val in vals.Errors)
                {
                    Console.WriteLine($"{key} - {val.ErrorMessage}");
                }
            }
        }
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                await _productService.Create(productVM, user.Id);

                return Ok("Tạo sản phẩm thành công");
            }
            else
            {
                return BadRequest("Thông tin nhập vào không hợp lệ");
            }
        }
        catch { throw; }

    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ProductVM productVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                await _productService.Update(id, productVM, user.Id);

                return Ok("Cập nhật sản phẩm thành công");
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
            await _productService.Delete(ids, user.Id);
        }
        catch { throw; }
        return Ok("Xóa sản phẩm thành công");
    }

    [HttpPost("active-unactive")]
    public async Task<IActionResult> ActiveOrUnactive([FromBody] int[] ids)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            await _productService.ActiveOrUnactive(ids, user.Id);
        }
        catch { throw; }
        return Ok("Thay đổi sử dụng sản phẩm thành công");
    }
}


