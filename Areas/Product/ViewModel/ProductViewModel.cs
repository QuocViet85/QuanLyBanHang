using System.ComponentModel.DataAnnotations;
using WebBanHang.Areas.Category.Model;

namespace WebBanHang.Areas.Product.ViewModel;

public class ProductVM
{
    public int Id { set; get; }
    [Required(ErrorMessage = "Phải có tên sản phẩm")]
    [Display(Name = "Tên sản phẩm")]
    public string Name { set; get; }

    [Display(Name = "Mô tả")]
    public string Description { set; get; }

    [Display(Name = "Số lượng")]
    public int Quantity { set; get; } = 1;

    [Display(Name = "Đang bán")]
    public bool IsActive { set; get; } = true;

    [Display(Name = "Giá")]
    public decimal Price { set; get; }

    [Display(Name = "Giảm giá")]
    public decimal Discount { set; get; }

    [Display(Name = "Danh mục")]
    public List<int>? CategoryIds { set; get; }

    public Dictionary<int, string> DynamicAttributes { set; get; }
}