using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Customer.ViewModel;
using WebBanHang.Areas.Customer.Model;
using WebBanHang.Data;

namespace WebBanHang.Areas.Customer.Controllers;

[Area("Customer")]
[Route("customer")]
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
        var user = await _userManager.GetUserAsync(User);

        var customers = await _dbContext.Customers.Where(c => c.UserId == user.Id).ToListAsync();

        return View(customers);
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var customer = await _dbContext.Customers.Where(c => c.Id == id && c.UserId == user.Id).FirstAsync();

        return View(customer);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(CustomerVM customerVM)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            var customerModel = new CustomerModel()
            {
                Name = customerVM.Name,
                PhoneNumber = customerVM.PhoneNumber,
                Email = customerVM.Email,
                Address = customerVM.Email,
                UserId = user.Id,
            };

            await _dbContext.Customers.AddAsync(customerModel);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Create");
        }
        return View();
    }

    [HttpGet("update/{id}")]
    public async Task<IActionResult> Update(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var customerUpdate = await _dbContext.Customers.Where(c => c.Id == id && c.UserId == user.Id).FirstOrDefaultAsync();

        var customerVMUpdate = new CustomerVM()
        {
            Name = customerUpdate.Name,
            PhoneNumber = customerUpdate.PhoneNumber,
            Email = customerUpdate.Email,
            Address = customerUpdate.Email,
        };

        return View(customerVMUpdate);
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, CustomerVM customerVM)
    {
        var user = await _userManager.GetUserAsync(User);

        try
        {
            var customerUpdate = await _dbContext.Customers.Where(c => c.Id == id && c.UserId == user.Id).FirstAsync();

            if (customerUpdate != null)
            {
                customerUpdate.Name = customerVM.Name;
                customerUpdate.PhoneNumber = customerVM.PhoneNumber;
                customerUpdate.Email = customerVM.Email;
                customerUpdate.Address = customerVM.Address;

                _dbContext.Customers.Update(customerUpdate);
                int result = await _dbContext.SaveChangesAsync();

                return RedirectToAction("Update", new { id = id });
            }
            return Redirect("Index");
        }
        catch
        {
            return Redirect("Index");
        } 
    }
    
    [HttpGet("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        try
        {
            var customerDelete = await _dbContext.Customers.Where(c => c.Id == id && c.UserId == user.Id).FirstAsync();
            if (customerDelete != null)
            {
                _dbContext.Customers.Remove(customerDelete);
                int result = await _dbContext.SaveChangesAsync();
            }   
        }
        catch{}
        return RedirectToAction("Index");
    }
}