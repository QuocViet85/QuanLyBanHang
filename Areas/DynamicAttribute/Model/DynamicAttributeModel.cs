using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebBanHang.Areas.DynamicAttribute.Model;

[Table("DynamicAttributes")]
public class DynamicAttributeModel
{
    [Key]
    public int Id { set; get; }
    public string Name { set; get; }
    public string UserId { set; get; }

    [ForeignKey("UserId")]
    public IdentityUser User { set; get; }
    public List<DynamicAttributeValueModel>? AttributeValues { set; get; }
}