using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace WebBanHang.Areas.DynamicAttribute.Model;

[Table("DynamicAttribute")]
public class AttributeModel 
{
    [Key]
    public int Id {set; get;}
    public string Name { set; get; }
    public string UserId { set; get; }

    [ForeignKey("UserId")]
    public IdentityUser User {set; get;}
    public List<AttributeValueModel>? AttributeValues {set; get;}
}