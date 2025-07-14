using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.Product.Controllers;
using WebBanHang.Areas.Product.Model;
using WebBanHang.Areas.Product.ViewModel;
using WebBanHang.Data;

namespace WebBanHang.Areas.Product.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _dbContext;

    public ProductService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<(List<ProductVM> productVMs, int totalProducts)> GetProducts(int pageNumber, int limit, string userId, string searchByName, string searchByCode, string searchByCategory)
    {
        IQueryable<ProductModel> queryProduct;
        List<ProductModel> products;
        if (pageNumber > 0 && limit > 0)
        {
            queryProduct = _dbContext.Products.Where(p => p.UserId == userId)
                                                .Skip((pageNumber - 1) * limit)
                                                .Take(limit);
        }
        else
        {
            queryProduct = _dbContext.Products.Where(p => p.UserId == userId)
                                                .Include(p => p.Category);
        }

        if (!string.IsNullOrEmpty(searchByName))
        {
            queryProduct = queryProduct.Where(p => p.Name.Contains(searchByName));
        }

        if (!string.IsNullOrEmpty(searchByCode))
        {
            queryProduct = queryProduct.Where(p => p.Code.Contains(searchByCode));
        }

        if (!string.IsNullOrEmpty(searchByCategory))
        {
            queryProduct = queryProduct.Where(p => p.Category.Name.Contains(searchByCategory));
        }

        products = await queryProduct.ToListAsync();

        int totalProducts = await _dbContext.Products.CountAsync();

        List<ProductVM> productVMs = new List<ProductVM>();

        if (products?.Count > 0)
        {
            productVMs = products.Select(p => GetProductVMFromProductModel(p)).ToList();
        }

        return (productVMs, totalProducts);
    }

    public async Task Create(ProductVM productVM, string userId)
    {
        bool existCode = await _dbContext.Products.AnyAsync(p => p.Code == productVM.Code && p.UserId == userId);
        if (existCode)
        {
            throw new Exception("Đã có sản phẩm có mã code này");
        }

        bool existSerial = await _dbContext.Products.AnyAsync(p => p.Serial == productVM.Serial && p.UserId == userId);
        if (existSerial)
        {
            throw new Exception("Đã có sản phẩm có số serial này");
        }

        var productModel = CreateOrUpdateProductModelFromProductVM(productVM, userId);

        await _dbContext.Products.AddAsync(productModel);
        await _dbContext.SaveChangesAsync();

    }

    public async Task Update(int id, ProductVM productVM, string userId)
    {
        ProductModel productUpdate = await _dbContext.Products.Where(p => p.Id == id && p.UserId == userId).FirstOrDefaultAsync();
        if (productUpdate != null)
        {
            bool existCode = await _dbContext.Products.AnyAsync(p => p.Code == productVM.Code && p.Id != productUpdate.Id && p.UserId == userId);
            if (existCode)
            {
                throw new Exception("Đã có sản phẩm có mã code này");
            }

            bool existSerial = await _dbContext.Products.AnyAsync(p => p.Serial == productVM.Serial && p.Id != productUpdate.Id && p.UserId == userId);
            if (existSerial)
            {
                throw new Exception("Đã có sản phẩm có số serial này");
            }
            CreateOrUpdateProductModelFromProductVM(productVM, userId, productUpdate);

            _dbContext.Products.Update(productUpdate);
            int result = await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Không tìm thấy sản phẩm");
        }
    }

    public async Task Delete(int id, string userId)
    {
        ProductModel productDelete = await _dbContext.Products.Where(p => p.Id == id && p.UserId == userId).FirstOrDefaultAsync();
        if (productDelete != null)
        {
            _dbContext.Products.Remove(productDelete);
            await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Không tìm thấy sản phẩm");
        }
    }

    public ProductVM GetProductVMFromProductModel(ProductModel product)
    {
        var productVM = new ProductVM();
        productVM.Id = product.Id;
        productVM.Name = product.Name;
        productVM.Code = product.Code;
        productVM.Serial = product.Serial;
        productVM.Unit = product.Unit;
        productVM.Description = product.Description;
        productVM.Quantity = product.Quantity;
        productVM.PriceImport = product.PriceImport;
        productVM.PriceWholesale = product.PriceWholesale;
        productVM.PriceRetail = product.PriceWholesale;
        productVM.InventoryStandard = product.InventoryStandard;
        productVM.Discount = product.Discount;
        productVM.CategoryId = product.CategoryId;

        return productVM;
    }

    public ProductModel CreateOrUpdateProductModelFromProductVM(ProductVM productVM, string userId, ProductModel productUpdate = null)
    {
        if (productUpdate == null)
        {
            var product = new ProductModel();

            product.Name = productVM.Name;
            product.Code = productVM.Code;
            product.Serial = productVM.Serial;
            product.Unit = productVM.Unit;
            product.Description = productVM.Description;
            product.Quantity = productVM.Quantity;
            product.PriceImport = productVM.PriceImport;
            product.PriceWholesale = productVM.PriceWholesale;
            product.PriceRetail = productVM.PriceWholesale;
            product.InventoryStandard = productVM.InventoryStandard;
            product.Discount = productVM.Discount;
            product.CategoryId = productVM.CategoryId;
            product.UserId = userId;
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;

            return product;
        }
        else
        {
            productUpdate.Name = productVM.Name;
            productUpdate.Code = productVM.Code;
            productUpdate.Serial = productVM.Serial;
            productUpdate.Unit = productVM.Unit;
            productUpdate.Description = productVM.Description;
            productUpdate.Quantity = productVM.Quantity;
            productUpdate.PriceImport = productVM.PriceImport;
            productUpdate.PriceWholesale = productVM.PriceWholesale;
            productUpdate.PriceRetail = productVM.PriceWholesale;
            productUpdate.InventoryStandard = productVM.InventoryStandard;
            productUpdate.Discount = productVM.Discount;
            productUpdate.CategoryId = productVM.CategoryId;
            productUpdate.UpdatedAt = DateTime.Now;

            return productUpdate;
        }
    }
}