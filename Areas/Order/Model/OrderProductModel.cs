using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Areas.Product.Model;

namespace WebBanHang.Order.Model;

[Table("OrderProduct")]
public class OrderProductModel
{
    public int ProductId {set; get;}
    public int OrderId {set; get;}
    public int Quantity {set; get;}

    [ForeignKey("ProductId")]
    public ProductModel Product {set; get;}
    
    [ForeignKey("OrderId")]
    public OrderModel Order {set; get;}
}