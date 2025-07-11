using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Product.Model;

namespace WebBanHang.Areas.Order.Model;

[Table("OrderDetails")]
public class OrderDetailModel
{  
    [Key]
    public int Id { set; get; }
    public int ProductId { set; get; }
    public string ProductName { set; get; }
    public int OrderId { set; get; }
    public int Quantity {set; get;}
    
    [Precision(18, 2)]
    public decimal Price { set; get; } = 0;

    [Precision(18, 2)]
    public decimal Discount { set; get; } = 0;

    [Precision(18, 2)]
    public decimal PriceBeforePrivateTax { set; get; }

    [Precision(18, 2)]
    public decimal PriceAfterPrivateTax { set; get; }
    public string? PrivateTaxes { set; get; }

    [ForeignKey("ProductId")]
    public ProductModel Product {set; get;}
    
    [ForeignKey("OrderId")]
    public OrderModel Order {set; get;}
}