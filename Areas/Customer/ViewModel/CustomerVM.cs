using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Areas.Customer.ViewModel;

public class CustomerVM
{
    public int Id { set; get; }
    [Display(Name = "Tên khách hàng")]
    [Required(ErrorMessage = "Khách hàng phải có tên")]
    public string Name { set; get; }

    [Display(Name = "Số điện thoại")]
    public int PhoneNumber { set; get; }
    
    [Display(Name = "Địa chỉ")]
    public string? Address { set; get; }
}