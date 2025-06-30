using Microsoft.AspNetCore.Mvc;
using WebBanHang.Areas.Product.ViewModel;

namespace WebBanHang.Areas.Product;

[Area("Product")]
[Route("product")]
public class ProductController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    public IActionResult Create(ProductVM productVM)
    {
        return View();
    }
}