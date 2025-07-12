using WebBanHang.Areas.Order.Model;
using WebBanHang.Areas.Order.ViewModel;
using WebBanHang.Areas.Tax.Model;

namespace WebBanHang.Areas.Order.Services;

public interface IOrderService
{
    public Task<(List<OrderVM> orderVMs, int totalOrders)> GetOrders(int pageNumber, int limit, string userId);

    public Task<OrderVM> GetOneOrder(int id, string userId);

    public Task Create(OrderVM orderVM, string userId);

    public Task CompletedOrder(int id, bool completed, string userId);

    public Task Delete(int id, string userId);

    public Task Destroy(int id, string userId);

    public OrderVM GetOrderVMFromOrderModel(OrderModel order);

    public Task<int> CalculateTotalPriceAndSaveOrder(OrderModel order, OrderVM orderVM, string userId);

    public Task<List<TaxModel>> GetTaxDefaults(string userId);
}