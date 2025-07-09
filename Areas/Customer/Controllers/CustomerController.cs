using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Customer.ViewModel;
using WebBanHang.Areas.Customer.Model;
using WebBanHang.Data;
using Microsoft.AspNetCore.Authorization;

namespace WebBanHang.Areas.Customer.Controllers;

[Area("Customer")]
[Route("api/customer")]
[Authorize]
public class CustomerController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;
    public CustomerController(UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int pageNumber, int limit)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            List<CustomerModel> customers;

            if (pageNumber > 0 && limit > 0)
            {
                customers = await _dbContext.Customers
                                            .Skip((pageNumber - 1) * limit)
                                            .Take(limit)
                                            .Where(c => c.UserId == user.Id).ToListAsync();
            }
            else
            {
                customers = await _dbContext.Customers.Where(c => c.UserId == user.Id).ToListAsync();
            }

            int totalCustomers = await _dbContext.Taxes.CountAsync();

            List<CustomerVM> customerVMs = new List<CustomerVM>();

            if (customers?.Count > 0)
            {
                customerVMs = customers.Select(c => GetCustomerVMFromCustomerModel(c)).ToList();
            }
            return Ok(new
            {
                customers = customerVMs,
                totalCustomers = totalCustomers
            });
        }
        catch { return BadRequest("Lấy khách hàng thất bại"); }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CustomerVM customerVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var customerModel = new CustomerModel()
                {
                    Name = customerVM.Name,
                    PhoneNumber = customerVM.PhoneNumber,
                    Address = customerVM.Address,
                    UserId = user.Id,
                };

                await _dbContext.Customers.AddAsync(customerModel);
                await _dbContext.SaveChangesAsync();

                return Ok("Tạo khách hàng thành công");
            }
            else
            {
                return BadRequest("Thông tin nhập vào không hợp lệ");
            }
        }
        catch { return BadRequest("Tạo khách hàng thất bại"); }
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CustomerVM customerVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var customerUpdate = await _dbContext.Customers.Where(c => c.Id == id && c.UserId == user.Id).FirstOrDefaultAsync();

                if (customerUpdate != null)
                {
                    customerUpdate.Name = customerVM.Name;
                    customerUpdate.PhoneNumber = customerVM.PhoneNumber;
                    customerUpdate.Address = customerVM.Address;

                    _dbContext.Customers.Update(customerUpdate);
                    int result = await _dbContext.SaveChangesAsync();

                    return Ok("Cập nhật khách hàng thành công");
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                return BadRequest("Thông tin nhập vào không hợp lệ");
            }
        }
        catch { return BadRequest("Cập nhật khách hàng thất bại"); }
    }

    [HttpPost("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var customerDelete = await _dbContext.Customers.Where(c => c.Id == id && c.UserId == user.Id).FirstOrDefaultAsync();
            if (customerDelete != null)
            {
                _dbContext.Customers.Remove(customerDelete);
                int result = await _dbContext.SaveChangesAsync();

                return Ok("Xóa khách hàng thành công");
            }
            else
            {
                throw new Exception();
            }
        }
        catch { return BadRequest("Xóa khách hàng thất bại"); }
    }

    private CustomerVM GetCustomerVMFromCustomerModel(CustomerModel customer)
    {
        return new CustomerVM()
        {
            Id = customer.Id,
            Name = customer.Name,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address
        };
    }
}