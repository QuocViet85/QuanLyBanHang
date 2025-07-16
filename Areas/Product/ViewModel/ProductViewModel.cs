using System.ComponentModel.DataAnnotations;
using WebBanHang.Areas.Category.Model;

namespace WebBanHang.Areas.Product.ViewModel;

public class ProductVM
{
    public int Id { set; get; }
    [Required(ErrorMessage = "Phải có tên sản phẩm")]
    public string Name { set; get; }
    public string Code { set; get; }
    public string Serial { set; get; }
    public string Unit { set; get; }
    public string? Description { set; get; }
    public int? Quantity { set; get; }

    public bool IsActive { set; get; }
    public decimal? PriceImport { set; get; }
    public decimal? PriceWholesale { set; get; }
    public decimal? PriceRetail { set; get; }
    //Tồn định mức
    public int? InventoryStandard { set; get; }
    public decimal? Discount { set; get; }
    public int? CategoryId { set; get; }

    public string? CategoryName { set; get; }

    public IFormFile? File { set; get; }
}