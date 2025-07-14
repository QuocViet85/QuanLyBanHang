using Microsoft.AspNetCore.Identity;
using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.Product.Model;

namespace WebBanHang.Models;

public class AppUser : IdentityUser
{
    public List<ProductModel> Products { set; get; }
    public List<CategoryModel> Categories { set; get; }

}