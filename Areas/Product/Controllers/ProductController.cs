using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.DynamicAttribute.Model;
using WebBanHang.Areas.Product.Model;
using WebBanHang.Areas.Product.ViewModel;
using WebBanHang.Areas.Tax.Model;
using WebBanHang.Data;

namespace WebBanHang.Areas.Product.Controllers;

[Area("Product")]
[Route("product")]
[Authorize]
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
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var products = await _dbContext.Products.Where(p => p.UserId == user.Id).ToListAsync();

            List<ProductVM> productVMs = new List<ProductVM>();

            if (products.Count > 0)
            {
                productVMs = products.Select(p => GetProductVMFromProductModel(p)).ToList();
            }
            return View(productVMs);
        }
        catch
        { }
        return null;
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var product = await _dbContext.Products.Where(p => p.Id == id && p.UserId == user.Id).FirstOrDefaultAsync();

            if (product != null)
            {
                var productVM = GetProductVMFromProductModel(product);
                return View(productVM);
            }
        }
        catch { }

        return null;
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        try
        {
            var categories = await GetCategories();
            ViewData["categories"] = categories;

            var attributes = await GetAttributes();
            ViewData["attributes"] = attributes;

            var privateTaxes = await GetPrivateTaxes();
            ViewData["privateTaxes"] = privateTaxes;

            return View();
        }
        catch { }
        return null;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(ProductVM productVM)
    {
        try
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


                await SetCategoryProducts(productModel.Id, productVM.CategoryIds);
                await SetPrivateTax(productModel.Id, productVM.PrivateTaxIds);
                await SetAttributeValue(productModel.Id, productVM.DynamicAttributes);
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

            var productUpdate = await _dbContext.Products.Where(p => p.Id == id && p.UserId == user.Id).FirstOrDefaultAsync();

            if (productUpdate != null)
            {
                var productVMUpdate = GetProductVMFromProductModel(productUpdate);

                var categories = await GetCategories();
                ViewData["categories"] = categories;

                var attributes = await GetAttributes();
                ViewData["attributes"] = attributes;

                var privateTaxes = await GetPrivateTaxes();
                ViewData["privateTaxes"] = privateTaxes;

                return View(productVMUpdate);
            }
        }
        catch { }
        return null;
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, ProductVM productVM)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            ProductModel productUpdate = await _dbContext.Products.Where(p => p.Id == id && p.UserId == user.Id).FirstOrDefaultAsync();

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
            }
            await SetCategoryProducts(productUpdate.Id, productVM.CategoryIds);
            await SetPrivateTax(productUpdate.Id, productVM.PrivateTaxIds);
            await SetAttributeValue(productUpdate.Id, productVM.DynamicAttributes);
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
            ProductModel productDelete = await _dbContext.Products.Where(p => p.Id == id && p.UserId == user.Id).FirstOrDefaultAsync();
            if (productDelete != null)
            {
                _dbContext.Products.Remove(productDelete);
                await _dbContext.SaveChangesAsync();
                await DeleteRelations(productDelete.Id);
            }
        }
        catch { }
        return RedirectToAction("Index");
    }

    private ProductVM GetProductVMFromProductModel(ProductModel product)
    {
        return new ProductVM()
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Quantity = product.Quantity,
            IsActive = product.IsActive,
            Price = product.Price,
            Discount = product.Discount,

        };
    }

    private async Task<List<CategoryModel>> GetCategories()
    {
        var user = await _userManager.GetUserAsync(User);
        var categories = await _dbContext.Categories.Include(c => c.CategoryProducts).Where(c => c.UserId == user.Id).ToListAsync();

        return categories;
    }

    private async Task SetCategoryProducts(int productId, List<int> categoryIds)
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM CategoryProducts WHERE ProductId = {0}", productId);

        if (categoryIds != null)
        {
            foreach (var categoryId in categoryIds)
            {
                var categoryProduct = new CategoryProductModel()
                {
                    ProductId = productId,
                    CategoryId = categoryId
                };

                await _dbContext.CategoryProducts.AddAsync(categoryProduct);
            }
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task<List<TaxModel>> GetPrivateTaxes()
    {
        var user = await _userManager.GetUserAsync(User);
        return await _dbContext.Taxes.Include(t => t.TaxProducts).Where(t => !t.IsDefault && t.UserId == user.Id).ToListAsync();
    }

    private async Task SetPrivateTax(int productId, List<int> taxIds)
    {
       // await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM TaxProducts WHERE ProductId = {0}", productId);

        if (taxIds != null)
        {
            foreach (var taxId in taxIds)
            {
                var taxProduct = new TaxProductModel()
                {
                    ProductId = productId,
                    TaxId = taxId
                };

                await _dbContext.TaxProducts.AddAsync(taxProduct);
            }
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task<List<AttributeModel>> GetAttributes()
    {
        var user = await _userManager.GetUserAsync(User);
        var attributes = await _dbContext.Attributes.Include(c => c.AttributeValues).Where(c => c.UserId == user.Id).ToListAsync();

        return attributes;
    }

    private async Task SetAttributeValue(int productId, Dictionary<int, string> dynamicAttributes)
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM DynamicAttributeValues WHERE ProductId = {0}", productId);

        if (dynamicAttributes != null)
        {
            var attributeIds = dynamicAttributes.Keys;

            foreach (var attributeId in attributeIds)
            {
                //attributeValue == null thì Product không có Attribute và Attribute không có Value nào tương ứng với Product => Không đưa dữ liệu vào database để không bị thừa dữ liệu
                if (!string.IsNullOrEmpty(dynamicAttributes[attributeId]))
                {
                    var attributeAndValues = new AttributeValueModel()
                    {
                        Content = dynamicAttributes[attributeId],
                        ProductId = productId,
                        AttributeId = attributeId
                    };
                    await _dbContext.AttributeValues.AddAsync(attributeAndValues);
                }
            }
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task DeleteRelations(int id)
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM CategoryProducts WHERE ProductId = {0}", id);
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM AttributeValues WHERE ProductId = {0}", id);
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM OrderProducts WHERE ProductId = {0}", id);
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM TaxProducts WHERE ProductId = {0}", id);
    }
}


