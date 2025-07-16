using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
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

    private readonly string[] ExtensionFileUploads = { ".jpg", ".jpeg", ".png", ".gif" };
    private readonly int LimitSize = 5097152;
    public async Task<(List<ProductVM> productVMs, int totalProducts)> GetProducts(int pageNumber, int limit, string userId, string searchByName, string searchByCode, string searchByCategory, string searchByUnit)
    {
        IQueryable<ProductModel> queryProduct = _dbContext.Products.Where(p => p.UserId == userId).Include(p => p.Category);
        List<ProductModel> products;
        if (pageNumber > 0 && limit > 0)
        {
            queryProduct = queryProduct.Skip((pageNumber - 1) * limit).Take(limit);
        }
        else if (limit < 0)
        {
            throw new Exception("Lỗi phân trang, không thể giới hạn số trang là số âm");
        }

        if (!string.IsNullOrEmpty(searchByName))
        {
            queryProduct = queryProduct.Where(p => p.Name.Contains(searchByName));
        }

        if (!string.IsNullOrEmpty(searchByCode))
        {
            queryProduct = queryProduct.Where(p => p.Code.Contains(searchByCode));
        }

        if (!string.IsNullOrEmpty(searchByUnit))
        {
            queryProduct = queryProduct.Where(p => p.Unit.Contains(searchByUnit));
        }

        if (!string.IsNullOrEmpty(searchByCategory))
        {
            queryProduct = queryProduct.Where(p => p != null && EF.Functions.Like(p.Category.Name, $"%{searchByCategory}%"));
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

        await HandleUploadFile(productModel.Id, productVM, userId);

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

            await HandleUploadFile(productUpdate.Id, productVM, userId);
        }
        else
        {
            throw new Exception("Không tìm thấy sản phẩm");
        }
    }

    public async Task Delete(int[] ids, string userId)
    {
        if (ids != null)
        {
            foreach (var id in ids)
            {
                bool exist = await _dbContext.Products.AnyAsync(p => p.Id == id && p.UserId == userId);
                if (exist)
                {
                    await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM Products WHERE id = {0}", id);
                }
            }
        }
        else
        {
            throw new Exception("Chưa chọn sản phẩm để xóa");
        }
    }

    public async Task ActiveOrUnactive(int[] ids, string userId)
    {
        if (ids != null)
        {
            var products = await _dbContext.Products.Where(p => ids.Any(id => id == p.Id) && p.UserId == userId).ToListAsync();

            foreach (var product in products)
            {
                product.IsActive = !product.IsActive;
            }

            await _dbContext.SaveChangesAsync();
        }
        else
        {
            throw new Exception("Chưa chọn sản phẩm để xóa");
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
        productVM.IsActive = product.IsActive;
        if (product.Category != null)
        {
            productVM.CategoryName = product.Category.Name;
        }


        return productVM;
    }

    public ProductModel CreateOrUpdateProductModelFromProductVM(ProductVM productVM, string userId, ProductModel productUpdate = null)
    {
        ProductModel product;
        if (productUpdate == null)
        {
            product = new ProductModel();

            product.UserId = userId;
            product.CreatedAt = DateTime.Now;
            product.UpdatedAt = DateTime.Now;
        }
        else
        {
            product = productUpdate;
            productUpdate.UpdatedAt = DateTime.Now;
        }

        product.Name = productVM.Name;
        product.Code = productVM.Code;
        product.Serial = productVM.Serial;
        product.Unit = productVM.Unit;
        product.Description = productVM.Description;
        product.Quantity = productVM.Quantity;
        product.PriceImport = productVM.PriceImport != null ? (int)productVM.PriceImport : 0;
        product.PriceWholesale = productVM.PriceWholesale != null ? (int)productVM.PriceWholesale : 0;
        product.PriceRetail = productVM.PriceRetail != null ? (int)productVM.PriceRetail : 0;
        product.InventoryStandard = productVM.InventoryStandard != null ? (int)productVM.InventoryStandard : 0;
        product.Discount = productVM.Discount;
        product.CategoryId = productVM.CategoryId;
        product.IsActive = productVM.IsActive;

        return product;
    }

    public async Task HandleUploadFile(int productId, ProductVM productVM, string userId)
    {
        if (productVM.File != null)
        {
            var extensionFileUpload = Path.GetExtension(productVM.File.FileName);

            if (ExtensionFileUploads.Contains(extensionFileUpload))
            {
                if (productVM.File.Length <= LimitSize)
                {
                    string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + extensionFileUpload;

                    string pathFile = Path.Combine("wwwroot", "images", "products", fileName);

                    using (FileStream fileStream = new FileStream(pathFile, FileMode.Create))
                    {
                        await productVM.File.CopyToAsync(fileStream);
                    }

                    var productPhoto = new ProductPhotoModel()
                    {
                        FileName = fileName,
                        ProductId = productId
                    };

                    await _dbContext.ProductPhotos.AddAsync(productPhoto);

                    var productPhotoOlds = await _dbContext.ProductPhotos.Where(pp => pp.ProductId == productId).ToListAsync();

                    if (productPhotoOlds.Count > 0)
                    {
                        foreach (var productPhotoOld in productPhotoOlds)
                        {
                            string pathOldFile = Path.Combine("wwwroot", "images", "products", productPhotoOld.FileName);
                            File.Delete(pathOldFile);
                        }

                        _dbContext.RemoveRange(productPhotoOlds);
                    }

                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Tạo sản phẩm thành công nhưng tải ảnh lên thất bại vì dung lượng file quá lớn. Chỉ tải lên file có dung lượng tối đa là 5 MB");
                }
            }
            else
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("Tạo sản phẩm thành công nhưng tải ảnh lên thất bại vì định dạng file không phù hợp. Chỉ upload file có các đuôi sau: ");

                foreach (var extension in ExtensionFileUploads)
                {
                    builder.Append($"{extension}, ");
                }

                string error = builder.ToString();

                throw new Exception(error);
            }
        }
    }
}