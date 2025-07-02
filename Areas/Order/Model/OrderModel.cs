using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using WebBanHang.Areas.Customer.Model;

namespace WebBanHang.Areas.Order.Model;

[Table("Order")]
public class OrderModel
{   
    [Key]
    public int Id {set; get;}
    public string? CustomerName {set; get;}
    public string UserId { set; get; }
    
    [ForeignKey("UserId")]
    public IdentityUser User { set; get; }
    public int? CustomerId { set; get; }

    [ForeignKey("CustomerId")]
    public CustomerModel? Customer {set; get;}
    public bool Completed {set; get;} = false;
    public DateTime CreatedAt {set; get;}
    public DateTime UpdatedAt {set; get;}
    public double Total {set; get;}
    public List<OrderProductModel> OrderProducts {set; get;}
    
}