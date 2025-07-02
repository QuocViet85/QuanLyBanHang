using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebBanHang.Areas.Category.Model;

[Table("Categories")]
public class CategoryModel
{
    [Key]
    public int Id { set; get; }
    public string Name { set; get; }
    public string Description { set; get; }
    public string UserId { set; get; }

    [ForeignKey("UserId")]
    public IdentityUser User { set; get; }
    public List<CategoryProductModel>? CategoryProducts { set; get; }
    public DateTime CreatedAt { set; get; }
    public DateTime UpdatedAt { set; get; }
}