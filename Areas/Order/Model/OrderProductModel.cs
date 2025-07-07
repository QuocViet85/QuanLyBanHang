using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Product.Model;

namespace WebBanHang.Areas.Order.Model;

[Table("OrderProducts")]
public class OrderProductModel
{
    public int ProductId {set; get;}
    public int OrderId {set; get;}
    public int Quantity {set; get;}

    [Precision(18, 2)]
    public decimal Discount { set; get; } = 0;

    [Precision(18, 2)]
    public decimal PriceBeforeTax { set; get; }

    [Precision(18, 2)]
    public decimal PriceAfterTax { set; get; }
    public string? Taxes { set; get; }

    [ForeignKey("ProductId")]
    public ProductModel Product {set; get;}
    
    [ForeignKey("OrderId")]
    public OrderModel Order {set; get;}
}