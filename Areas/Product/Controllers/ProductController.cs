using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
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
[Route("api/product")]
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

    public async Task<IActionResult> Index(int pageNumber, int limit)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            List<ProductModel> products;
            if (pageNumber > 0 && limit > 0)
            {
                products = await _dbContext.Products.Where(p => p.UserId == user.Id)
                                                    .Include(p => p.CategoryProducts)
                                                    .Include(p => p.AttributeProducts)
                                                    .Include(p => p.TaxProducts)
                                                    .Skip((pageNumber - 1) * limit)
                                                    .Take(limit)
                                                    .ToListAsync();
            }
            else
            {
                products = await _dbContext.Products.Where(p => p.UserId == user.Id)
                                                    .Include(p => p.CategoryProducts)
                                                    .Include(p => p.AttributeProducts)
                                                    .Include(p => p.TaxProducts)
                                                    .ToListAsync();
            }

            int totalProducts = await _dbContext.Products.CountAsync();

            List<ProductVM> productVMs = new List<ProductVM>();

            if (products?.Count > 0)
            {
                productVMs = products.Select(p => GetProductVMFromProductModel(p)).ToList();
            }
            return Ok(new
            {
                products = productVMs,
                totalProducts = totalProducts
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

                var productModel = new ProductModel()
                {
                    Name = productVM.Name,
                    Description = productVM.Description,
                    Quantity = productVM.Quantity,
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
        Console.WriteLine("NULL PR");
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                ProductModel productUpdate = await _dbContext.Products.Where(p => p.Id == id && p.UserId == user.Id).FirstOrDefaultAsync();

                if (productUpdate != null)
                {

                    productUpdate.Name = productVM.Name;
                    productUpdate.Description = productVM.Description;
                    productUpdate.Quantity = productVM.Quantity;
                    productUpdate.Price = productVM.Price;
                    productUpdate.Discount = productVM.Discount;
                    productUpdate.UpdatedAt = DateTime.Now;

                    _dbContext.Products.Update(productUpdate);
                    int result = await _dbContext.SaveChangesAsync();

                    await SetCategoryProducts(productUpdate.Id, productVM.CategoryIds);
                    await SetPrivateTax(productUpdate.Id, productVM.PrivateTaxIds);
                    await SetAttributeValue(productUpdate.Id, productVM.DynamicAttributes);

                    return Ok("Cập nhật sản phẩm thành công");
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
        catch { return BadRequest("Cập nhật sản phẩm thất bại"); }
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            ProductModel productDelete = await _dbContext.Products.Where(p => p.Id == id && p.UserId == user.Id).FirstOrDefaultAsync();
            if (productDelete != null)
            {
                await DeleteRelations(productDelete.Id);
                _dbContext.Products.Remove(productDelete);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new Exception();
            }
        }
        catch { return BadRequest("Xóa sản phẩm thất bại"); }
        return Ok();
    }

    private ProductVM GetProductVMFromProductModel(ProductModel product)
    {
        var productVM = new ProductVM();
        productVM.Id = product.Id;
        productVM.Name = product.Name;
        productVM.Description = product.Description;
        productVM.Quantity = product.Quantity;
        productVM.Price = product.Price;
        productVM.Discount = product.Discount;
        productVM.CreatedAt = product.CreatedAt.ToString("dd-MM-yyyy");
        productVM.UpdatedAt = product.UpdatedAt.ToString("dd-MM-yyyy");

        if (product.CategoryProducts != null)
        {
            foreach (var categoryProduct in product.CategoryProducts)
            {
                productVM.CategoryIds.Add(categoryProduct.CategoryId);
            }
        }

        if (product.AttributeProducts != null)
        {
            foreach (var attributeProduct in product.AttributeProducts)
            {
                productVM.DynamicAttributes.Add(attributeProduct.AttributeId, attributeProduct.Content);
            }
        }

        if (product.TaxProducts != null)
        {
            foreach (var taxProduct in product.TaxProducts)
            {
                productVM.PrivateTaxIds.Add(taxProduct.TaxId);
            }
        }
        
        return productVM;
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

    private async Task SetPrivateTax(int productId, List<int> taxIds)
    {
       await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM TaxProducts WHERE ProductId = {0}", productId);

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
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM DynamicAttributeValues WHERE ProductId = {0}", id);
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM OrderProducts WHERE ProductId = {0}", id);
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM TaxProducts WHERE ProductId = {0}", id);
    }
}


