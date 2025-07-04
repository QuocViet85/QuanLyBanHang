using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Customer.ViewModel;
using WebBanHang.Areas.Customer.Model;
using WebBanHang.Data;
using Microsoft.AspNetCore.Authorization;

namespace WebBanHang.Areas.Customer.Controllers;

[Area("Customer")]
[Route("customer")]
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

    public async Task<IActionResult> Index()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var customers = await _dbContext.Customers.Where(c => c.UserId == user.Id).ToListAsync();

            List<CustomerVM> customerVMs = new List<CustomerVM>();

            if (customers.Count > 0)
            {
                customerVMs = customers.Select(c => GetCustomerVMFromCustomerModel(c)).ToList();
            }
            return View(customerVMs);
        }
        catch { }
        return null;
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var customer = await _dbContext.Customers.Where(c => c.Id == id && c.UserId == user.Id).FirstOrDefaultAsync();

            if (customer != null)
            {
                var customerVM = GetCustomerVMFromCustomerModel(customer);
                return View(customerVM);
            }
        }
        catch { }
        return null;
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CustomerVM customerVM)
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
            }
        }
        catch { }
        return RedirectToAction("Create");
    }

    [HttpGet("update/{id}")]
    public async Task<IActionResult> Update(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var customerUpdate = await _dbContext.Customers.Where(c => c.Id == id && c.UserId == user.Id).FirstOrDefaultAsync();

            if (customerUpdate != null)
            {
                var customerVMUpdate = GetCustomerVMFromCustomerModel(customerUpdate);
                return View(customerVMUpdate);
            }
        }
        catch { }

        return null;
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, CustomerVM customerVM)
    {
        try
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
            }
        }
        catch { }

        return RedirectToAction("Update", new { id = id });
    }

    [HttpGet("delete/{id}")]
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
            }
        }
        catch { }
        return RedirectToAction("Index");
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