using Microsoft.AspNetCore.Identity;
using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.DynamicAttribute.Model;
using WebBanHang.Areas.Order.Model;
using WebBanHang.Areas.Product.Model;

namespace WebBanHang.Models;

public class AppUser : IdentityUser
{
    public List<ProductModel> Products { set; get; }
    public List<OrderModel> Orders { set; get; }
    public List<CategoryModel> Categories { set; get; }
    public List<DynamicAttributeModel> Attributes { set; get; }

}