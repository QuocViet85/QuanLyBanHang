using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;

namespace WebBanHang.Areas.Tax.Model;

[Table("Taxes")]
public class TaxModel
{
    [Key]
    public int Id { set; get; }
    public string Name { set; get; }
    public string? Code { set; get; }
    public decimal Rate { set; get; }
    public string? Description { set; get; }
    public bool IsActive { set; get; } = true;
    public bool IsDefault { set; get; }
    public string UserId { set; get; }

    [ForeignKey("UserId")]
    public IdentityUser User { set; get; }
    public List<TaxProductModel> TaxProducts { set; get; }

}