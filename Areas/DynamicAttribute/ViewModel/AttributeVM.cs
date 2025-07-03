using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Areas.DynamicAttribute.ViewModel;

public class AttributeVM
{
    public int Id { set; get; }
    [Required(ErrorMessage = "Thuộc tính sản phẩm phải có tên")]
    [Display(Name = "Thuộc tính sản phẩm")]
    public string Name { set; get; }
}