using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebBanHang.Areas.Product.Services;
using WebBanHang.Areas.Product.ViewModel;


namespace WebBanHang.Areas.Product.Controllers;

[Area("Product")]
[Route("api/product")]
// [Authorize]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    private readonly UserManager<IdentityUser> _userManager;

    public string userDemo = "da1c87e4-0df8-4000-8bac-79b48d2082c4";
    public ProductController(UserManager<IdentityUser> userManager, IProductService productService)
    {
        _productService = productService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int pageNumber, int limit, string searchByName, string searchByCode, string searchByCategory)
    {
        try
        {
            //var user = await _userManager.GetUserAsync(User);

            var result = await _productService.GetProducts(pageNumber, limit, userDemo, searchByName, searchByCode, searchByCategory);
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
        try
        {
            if (ModelState.IsValid)
            {
                //var user = await _userManager.GetUserAsync(User);

                await _productService.Create(productVM, userDemo);

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
                //var user = await _userManager.GetUserAsync(User);

                await _productService.Update(id, productVM, userDemo);

                return Ok("Cập nhật sản phẩm thành công");
            }
            else
            {
                return BadRequest("Thông tin nhập vào không hợp lệ");
            }

        }
        catch { throw; }
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            //var user = await _userManager.GetUserAsync(User);
            await _productService.Delete(id, userDemo);
        }
        catch { return BadRequest("Xóa sản phẩm thất bại"); }
        return Ok("Xóa sản phẩm thành công");
    }
}


