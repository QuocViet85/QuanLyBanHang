using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using WebBanHang.Areas.Category.Model;
using WebBanHang.Areas.DynamicAttribute.Model;
using WebBanHang.Areas.Order.Model;
using WebBanHang.Areas.Tax.Model;

namespace WebBanHang.Areas.Product.Model;

[Table("Products")]
public class ProductModel
{
    [Key]
    public int Id { set; get; }

    public string Name { set; get; }
    public string? Description { set; get; }
    public int Quantity { set; get; }
    public bool IsActive { set; get; } = true;
    public string UserId { set; get; }
    public decimal Price { set; get; }
    public decimal Discount { set; get; } = 0;

    [ForeignKey("UserId")]
    public IdentityUser User { set; get; }
    public DateTime CreatedAt { set; get; }
    public DateTime UpdatedAt { set; get; }

    public List<CategoryProductModel>? CategoryProducts { set; get; }
    public List<AttributeValueModel>? AttributeProducts { set; get; }
    public List<ProductPhotoModel>? ProductPhotos { set; get; }
    public List<OrderProductModel>? OrderProducts { set; get; }
    public List<TaxProductModel>? TaxProducts { set; get; }

}