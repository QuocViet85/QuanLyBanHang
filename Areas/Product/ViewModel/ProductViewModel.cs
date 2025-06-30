using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Areas.Product.ViewModel;

public class ProductVM
{
    [Required(ErrorMessage = "Phải có tên sản phẩm")]
    [Display(Name = "Tên sản phẩm")]
    public string Name { set; get; }

    [Display(Name = "Mô tả")]
    public string Description { set; get; }

    [Display(Name = "Số lượng")]
    public int Quantity { set; get; } = 1;

    [Display(Name = "Đang bán")]
    public bool IsActive { set; get; } = true;

    [Display(Name = "Giảm giá")]
    public double Discount { set; get; }

    [Display(Name = "Danh mục")]
    public List<int>? CategoryIds { set; get; }
}