using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Order.ViewModel;
using WebBanHang.Data;
using WebBanHang.Areas.Order.Model;

namespace WebBanHang.Areas.Order.Controllers;

[Area("Order")]
[Route("Order")]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;
    public OrderController(UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var orders = await _dbContext.Orders.Where(o => o.UserId == user.Id).ToListAsync();

        return View(orders);
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var order = await _dbContext.Orders.Where(o => o.Id == id && o.UserId == user.Id).FirstAsync();

        return View(order);
    }

    [HttpGet("create")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(OrderVM orderVM)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.GetUserAsync(User);

            var orderModel = new OrderModel()
            {
                CustomerName = orderVM.CustomerName,
                Completed = orderVM.Completed,
                UserId = user.Id,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _dbContext.Orders.AddAsync(orderModel);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Create");
        }
        return View();
    }

    [HttpGet("update/{id}")]
    public async Task<IActionResult> Update(int id)
    {
        var user = await _userManager.GetUserAsync(User);

        var orderUpdate = await _dbContext.Orders.Where(o => o.Id == id && o.UserId == user.Id).FirstOrDefaultAsync();

        var orderVMUpdate = new OrderVM()
        {
            CustomerName = orderUpdate.CustomerName,
            Completed = orderUpdate.Completed,
        };

        return View(orderVMUpdate);
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, OrderVM OrderVM)
    {
        var user = await _userManager.GetUserAsync(User);

        try
        {
            var orderUpdate = await _dbContext.Orders.Where(o => o.Id == id && o.UserId == user.Id).FirstAsync();

            if (orderUpdate != null)
            {
                orderUpdate.CustomerName = OrderVM.CustomerName;
                orderUpdate.Completed = OrderVM.Completed;
                orderUpdate.UpdatedAt = DateTime.Now;

                _dbContext.Orders.Update(orderUpdate);
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
            OrderModel OrderDelete = await _dbContext.Orders.Where(o => o.Id == id && o.UserId == user.Id).FirstAsync();
            if (OrderDelete != null)
            {
                _dbContext.Orders.Remove(OrderDelete);
                int result = await _dbContext.SaveChangesAsync();
            }   
        }
        catch{}
        return RedirectToAction("Index");
    }
}