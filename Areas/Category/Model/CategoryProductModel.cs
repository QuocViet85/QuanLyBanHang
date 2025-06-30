using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.Product.Model;

namespace WebBanHang.Areas.Category.Model;

[Table("CategoryProduct")]
public class CategoryProductModel
{
    public int ProductId { set; get; }
    public int CategoryId { set; get; }

    [ForeignKey("ProductId")]
    public ProductModel Product { set; get; }

    [ForeignKey("CategoryId")]
    public CategoryModel Category { set; get; }
}