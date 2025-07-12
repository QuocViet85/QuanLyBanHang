using WebBanHang.Areas.Product.Model;
using WebBanHang.Areas.Product.ViewModel;

namespace WebBanHang.Areas.Product.Services;

public interface IProductService
{
    public Task<(List<ProductVM> productVMs, int totalProducts)> GetProducts(int pageNumber, int limit, string userId);

    public Task Create(ProductVM productVM, string userId);

    public Task Update(int id, ProductVM productVM, string userId);

    public Task Delete(int id, string userId);

    public ProductVM GetProductVMFromProductModel(ProductModel product);

    public Task SetCategoryProducts(int productId, List<int> categoryIds);

    public Task SetPrivateTax(int productId, List<int> taxIds);

    public Task SetDynamicAttributeValue(int productId, Dictionary<int, string> dynamicAttributes);

    public Task DeleteRelations(int id);
}