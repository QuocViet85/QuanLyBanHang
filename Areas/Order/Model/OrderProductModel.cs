using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Areas.Product.Model;

namespace WebBanHang.Areas.Order.Model;

[Table("OrderProducts")]
public class OrderProductModel
{
    public int ProductId {set; get;}
    public int OrderId {set; get;}
    public int Quantity {set; get;}
    public decimal Discount { set; get; } = 0;
    public decimal PriceBeforeTax { set; get; }
    public decimal PriceAfterTax { set; get; }
    public string? Taxes { set; get; }

    [ForeignKey("ProductId")]
    public ProductModel Product {set; get;}
    
    [ForeignKey("OrderId")]
    public OrderModel Order {set; get;}
}