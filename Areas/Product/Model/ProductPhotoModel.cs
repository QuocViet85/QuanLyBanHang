using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Areas.Product.Model;

[Table("ProductPhotos")]
public class ProductPhotoModel
{
    [Key]
    public int Id {set; get;}
    public string FileName {set; get;}
    public int ProductId {set; get;}
    
    [ForeignKey("ProductId")]
    public ProductModel Product {set; get;}
}