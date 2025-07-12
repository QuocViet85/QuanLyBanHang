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

    public async Task<IActionResult> Index(int pageNumber, int limit)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var result = await _productService.GetProducts(pageNumber, limit, user.Id);
            return Ok(new
            {
                products = result.productVMs,
                totalProducts = result.totalProducts
            });
        }
        catch
        { return BadRequest("Lấy sản phẩm thất bại"); }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ProductVM productVM)
    {
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
        catch { return BadRequest("Tạo sản phẩm thất bại"); }

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
        catch { return BadRequest("Cập nhật sản phẩm thất bại"); }
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            await _productService.Delete(id, user.Id);
        }
        catch { return BadRequest("Xóa sản phẩm thất bại"); }
        return Ok();
    }
}


