using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Category.Model;

namespace WebBanHang.Areas.Product.Model;

[Table("Products")]
public class ProductModel
{
    [Key]
    public int Id { set; get; }

    [Required]
    public string Code { set; get; }
    public string Serial { set; get; }
    public string Name { set; get; }
    public string Unit { set; get; }
    public string? Description { set; get; }
    public int? Quantity { set; get; }

    public bool IsActive { set; get; } = true;

    //giá nhập
    [Precision(18, 2)]
    public decimal PriceImport { set; get; }

    //giá buôn
    [Precision(18, 2)]
    public decimal PriceWholesale { set; get; }

    //giá lẻ
    [Precision(18, 2)]
    public decimal PriceRetail { set; get; }

    //Tồn định mức
    public int InventoryStandard { set; get; }

    [Precision(18, 2)]
    public decimal? Discount { set; get; }
    public string UserId { set; get; }

    [ForeignKey("UserId")]
    public IdentityUser User { set; get; }
    public int? CategoryId { set; get; }

    [ForeignKey("CategoryId")]
    public CategoryModel? Category { set; get; }
    public DateTime CreatedAt { set; get; }
    public DateTime UpdatedAt { set; get; }
    public List<ProductPhotoModel>? ProductPhotos { set; get; }

}