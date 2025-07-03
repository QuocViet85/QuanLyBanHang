using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Areas.Product.Model;

namespace WebBanHang.Areas.DynamicAttribute.Model;

[Table("DynamicAttributeValues")]
public class AttributeValueModel 
{   
    public string Content {set; get;}
    public int ProductId { set; get; }

    [ForeignKey("ProductId")]
    public ProductModel Product { set; get; }
    public int AttributeId { set; get; }

    [ForeignKey("AttributeId")]
    public AttributeModel Attribute {set; get;}
}