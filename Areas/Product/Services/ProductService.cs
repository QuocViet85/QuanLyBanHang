using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.DynamicAttribute.Model;
using WebBanHang.Areas.Product.Model;
using WebBanHang.Areas.Product.ViewModel;
using WebBanHang.Areas.Tax.Model;
using WebBanHang.Data;

namespace WebBanHang.Areas.Product.Services;

public class ProductService : IProductService
{
    private readonly ApplicationDbContext _dbContext;

    public ProductService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<(List<ProductVM> productVMs, int totalProducts)> GetProducts(int pageNumber, int limit, string userId)
    {
        List<ProductModel> products;
        if (pageNumber > 0 && limit > 0)
        {
            products = await _dbContext.Products.Where(p => p.UserId == userId)
                                                .Include(p => p.CategoryProducts)
                                                .Include(p => p.AttributeProducts)
                                                .Include(p => p.TaxProducts)
                                                .Skip((pageNumber - 1) * limit)
                                                .Take(limit)
                                                .ToListAsync();
        }
        else
        {
            products = await _dbContext.Products.Where(p => p.UserId == userId)
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

        return (productVMs, totalProducts);
    }

    public async Task Create(ProductVM productVM, string userId)
    {
        var productModel = new ProductModel()
        {
            Name = productVM.Name,
            Description = productVM.Description,
            Quantity = productVM.Quantity,
            UserId = userId,
            Price = productVM.Price,
            Discount = productVM.Discount,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now
        };

        await _dbContext.Products.AddAsync(productModel);
        await _dbContext.SaveChangesAsync();


        await SetCategoryProducts(productModel.Id, productVM.CategoryIds);
        await SetPrivateTax(productModel.Id, productVM.PrivateTaxIds);
        await SetDynamicAttributeValue(productModel.Id, productVM.DynamicAttributes);
    }

    public async Task Update(int id, ProductVM productVM, string userId)
    {
        ProductModel productUpdate = await _dbContext.Products.Where(p => p.Id == id && p.UserId == userId).FirstOrDefaultAsync();

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
            await SetDynamicAttributeValue(productUpdate.Id, productVM.DynamicAttributes);

        }
    }

    public async Task Delete(int id, string userId)
    {
        ProductModel productDelete = await _dbContext.Products.Where(p => p.Id == id && p.UserId == userId).FirstOrDefaultAsync();
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

    public async Task DeleteRelations(int id)
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM CategoryProducts WHERE ProductId = {0}", id);
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM DynamicAttributeValues WHERE ProductId = {0}", id);
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM TaxProducts WHERE ProductId = {0}", id);
    }

    public ProductVM GetProductVMFromProductModel(ProductModel product)
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

    public async Task SetDynamicAttributeValue(int productId, Dictionary<int, string> dynamicAttributes)
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
                    var attributeAndValues = new DynamicAttributeValueModel()
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

    public async Task SetCategoryProducts(int productId, List<int> categoryIds)
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

    public async Task SetPrivateTax(int productId, List<int> taxIds)
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
}