using WebBanHang.Areas.Product.Model;
using WebBanHang.Areas.Product.ViewModel;

namespace WebBanHang.Areas.Product.Services;

public interface IProductService
{
    public Task<(List<ProductVM> productVMs, int totalProducts)> GetProducts(int pageNumber, int limit, string userId, string searchByName, string searchByCode, string searchByCategory, string searchByUnit);

    public Task Create(ProductVM productVM, string userId);

    public Task Update(int id, ProductVM productVM, string userId);

    public Task Delete(int[] ids, string userId);

    public Task ActiveOrUnactive(int[] ids, string userId);

    public ProductVM GetProductVMFromProductModel(ProductModel product);

    public ProductModel CreateOrUpdateProductModelFromProductVM(ProductVM productVM, string userId, ProductModel productUpdate = null);

    public Task HandleUploadFile(int productId, ProductVM productVM, string userId);
}