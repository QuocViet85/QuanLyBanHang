using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Areas.Category.ViewModel;

public class CategoryVM
{
    public int Id { set; get; }

    [Required(ErrorMessage = "Phải có tên danh mục sản phẩm")]
    public string Name { set; get; }
    public string Code { set; get; }
    public string? Description { set; get; }

}