using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Areas.Order.ViewModel;

public class OrderVM
{
    [Display(Name = "Tên khách hàng")]
    public string CustomerName { set; get; }

    [Display(Name = "Khách hàng")]
    public int CustomerId { set; get; }

    [Display(Name = "Hoàn thành")]
    public bool Completed { set; get; }
    public List<ProductInOrder>? ProductInOrders { set; get; }
}

public class ProductInOrder
{
    public int ProductId { set; get; }
    public int Quantity { set; get; }
}