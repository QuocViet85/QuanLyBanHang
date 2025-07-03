using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Areas.Product.Model;

namespace WebBanHang.Areas.Tax.Model;

[Table("TaxProducts")]
public class TaxProductModel
{
    public int ProductId { set; get; }
    public int TaxId { set; get; }

    [ForeignKey("ProductId")]
    public ProductModel Product { set; get; }

    [ForeignKey("TaxId")]
    public TaxModel Tax { set; get; }
}