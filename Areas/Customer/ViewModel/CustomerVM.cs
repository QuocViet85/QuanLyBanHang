using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Areas.Customer.ViewModel;

public class CustomerVM
{
    [Display(Name = "Tên khách hàng")]
    [Required(ErrorMessage = "Khách hàng phải có tên")]
    public string Name { set; get; }

    [Display(Name = "Số điện thoại")]
    public int PhoneNumber { set; get; }

    [Display(Name = "Email")]
    public string Email { set; get; }
    
    [Display(Name = "Địa chỉ")]
    public string Address { set; get; }
}