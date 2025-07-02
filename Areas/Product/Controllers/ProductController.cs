using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Product.Model;
using WebBanHang.Areas.Product.ViewModel;
using WebBanHang.Data;

namespace WebBanHang.Areas.Product.Controllers;

[Area("Product")]
[Route("product")]
public class ProductController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;
    public ProductController(UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var products = await _dbContext.Products.Where(p => p.UserId == user.Id).ToListAsync();

        return View(products);
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var product = await _dbContext.Products.Where(p => p.Id == id && p.UserId == user.Id).FirstAsync();

        return View(product);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(ProductVM productVM)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            var productModel = new ProductModel()
            {
                Name = productVM.Name,
                Description = productVM.Description,
                Quantity = productVM.Quantity,
                IsActive = productVM.IsActive,
                UserId = user.Id,
                Price = productVM.Price,
                Discount = productVM.Discount,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _dbContext.Products.AddAsync(productModel);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Create");
        }
        return View();
    }

    [HttpGet("update/{id}")]
    public async Task<IActionResult> Update(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var productUpdate = await _dbContext.Products.Where(p => p.Id == id && p.UserId == user.Id).FirstOrDefaultAsync();

        var productVMUpdate = new ProductVM()
        {
            Name = productUpdate.Name,
            Description = productUpdate.Description,
            Quantity = productUpdate.Quantity,
            IsActive = productUpdate.IsActive,
            Price = productUpdate.Price,
            Discount = productUpdate.Discount,
        };

        return View(productVMUpdate);
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, ProductVM productVM)
    {
        var user = await _userManager.GetUserAsync(User);

        try
        {
            ProductModel productUpdate = await _dbContext.Products.Where(p => p.Id == id && p.UserId == user.Id).FirstAsync();

            if (productUpdate != null)
            {
                productUpdate.Name = productVM.Name;
                productUpdate.Description = productVM.Description;
                productUpdate.Quantity = productVM.Quantity;
                productUpdate.IsActive = productVM.IsActive;
                productUpdate.Price = productVM.Price;
                productUpdate.Discount = productVM.Discount;
                productUpdate.UpdatedAt = DateTime.Now;

                _dbContext.Products.Update(productUpdate);
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
            ProductModel productDelete = await _dbContext.Products.Where(p => p.Id == id && p.UserId == user.Id).FirstAsync();
            if (productDelete != null)
            {
                _dbContext.Products.Remove(productDelete);
                int result = await _dbContext.SaveChangesAsync();
            }   
        }
        catch{}
        return RedirectToAction("Index");
    }
}