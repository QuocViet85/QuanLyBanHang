using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using WebBanHang.Areas.Order.Model;

namespace WebBanHang.Areas.Customer.Model;

[Table("Customers")]
public class CustomerModel
{
    public int Id {set; get;}
    public string Name {set; get;}
    public int PhoneNumber {set; get;}
    public string? Address {set; get;}
    public string UserId {set; get;}

    [ForeignKey("UserId")]
    public IdentityUser User {set; get;}
    public List<OrderModel>? Orders {set; get;}

}