using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Areas.Category.ViewModel;

public class CategoryVM
{
    public int Id { set; get; }
    [Display(Name = "Tên chuyên mục")]
    [Required(ErrorMessage = "Chuyên mục phải có tên")]
    public string Name { set; get; }

    [Display(Name = "Mô tả")]
    public string Description { set; get; }
}