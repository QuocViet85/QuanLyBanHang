using System.ComponentModel.DataAnnotations;
using WebBanHang.Areas.Order.Model;

namespace WebBanHang.Areas.Order.ViewModel;

public class OrderVM
{
    public int Id { set; get; }

    [Display(Name = "Tên đơn hàng")]
    public string? Name { set; get; }

    [Display(Name = "Tên khách hàng")]
    public string? CustomerName { set; get; }

    public int? CustomerPhoneNumber { set; get; }

    [Display(Name = "Hoàn thành")]
    public bool Completed { set; get; }
    public List<ProductInOrder>? ProductInOrders { set; get; }

    public List<OrderDetailModel>? OrderDetails { set; get; }
    public string? DefaultTaxes { set; get; }

    public decimal? TotalBeforeDefaultTax { set; get; }
    public decimal? TotalAfterTax { set; get; }
    public string? CreatedAt { set; get; }
}

public class ProductInOrder
{
    public int ProductId { set; get; }
    public int Quantity { set; get; }
}