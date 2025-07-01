using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Areas.Product.Model;

namespace WebBanHang.Areas.DynamicAttribute.Model;

[Table("AttributeProduct")]
public class AttributeProductModel
{
    public int ProductId { set; get; }
    public int AttributeId { set; get; }
    public ProductModel Product { set; get; }

    [ForeignKey("AttributeId")]
    public AttributeModel Attribute { set; get; }
}