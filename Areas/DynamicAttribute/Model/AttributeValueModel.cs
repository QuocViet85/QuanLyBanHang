using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Areas.DynamicAttribute.Model;

[Table("DynamicAttributeValues")]
public class AttributeValueModel 
{   
    [Key]
    public int Id {set; get;}
    public string Content {set; get;}
    public int AttributeId {set; get;}

    [ForeignKey("AttributeId")]
    public AttributeModel Attribute {set; get;}
}