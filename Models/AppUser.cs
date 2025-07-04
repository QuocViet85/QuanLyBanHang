using Microsoft.AspNetCore.Identity;
using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.Customer.Controllers;
using WebBanHang.Areas.Customer.Model;
using WebBanHang.Areas.DynamicAttribute.Model;
using WebBanHang.Areas.Order.Model;
using WebBanHang.Areas.Product.Model;

namespace WebBanHang.Models;

public class AppUser : IdentityUser
{
    public List<ProductModel> Products { set; get; }
    public List<OrderModel> Orders { set; get; }
    public List<CategoryModel> Categories { set; get; }
    public List<AttributeModel> Attributes { set; get; }
    public List<CustomerModel> Customer { set; get; }

}