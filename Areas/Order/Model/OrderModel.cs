using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebBanHang.Areas.Order.Model;

[Table("Orders")]
public class OrderModel
{
    [Key]
    public int Id { set; get; }
    public string? Name { set; get; }
    public string? CustomerName { set; get; }

    public int? CustomerPhoneNumber { set; get; }
    public string UserId { set; get; }

    [ForeignKey("UserId")]
    public IdentityUser User { set; get; }
    public bool Completed { set; get; } = true;

    public string? DefaultTaxes { set; get; }
    public DateTime CreatedAt { set; get; }

    [Precision(18, 2)] //phải chỉ rõ độ chính xác của kiểu decimal trong database
    public decimal TotalBeforeDefaultTax { set; get; }

    [Precision(18, 2)]
    public decimal TotalAfterTax { set; get; }
    public List<OrderDetailModel> OrderDetails { set; get; }

}