using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Areas.Product.Model;

namespace WebBanHang.Areas.DynamicAttribute.Model;

[Table("DynamicAttributeValues")]
public class DynamicAttributeValueModel
{
    public string Content { set; get; }
    public int ProductId { set; get; }

    [ForeignKey("ProductId")]
    public ProductModel Product { set; get; }
    public int AttributeId { set; get; }

    [ForeignKey("AttributeId")]
    public DynamicAttributeModel Attribute { set; get; }
}