using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Order.ViewModel;
using WebBanHang.Data;
using WebBanHang.Areas.Order.Model;
using WebBanHang.Areas.Customer.Model;
using WebBanHang.Areas.Tax.Model;
using WebBanHang.Areas.Product.Model;

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
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var orders = await _dbContext.Orders.Include(o => o.Customer).Where(o => o.UserId == user.Id).ToListAsync();

            List<OrderVM> orderVMs = new List<OrderVM>();

            if (orderVMs.Count > 0)
            {
                orderVMs = orders.Select(p => GetOrderVMFromOrderModel(p)).ToList();
            }

            return View(orderVMs);
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

            var order = await _dbContext.Orders.Where(o => o.Id == id && o.UserId == user.Id).FirstOrDefaultAsync();

            if (order != null)
            {
                var orderVM = GetOrderVMFromOrderModel(order);
                return View(order);
            }
            return null;
        }
        catch { }

        return null;
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        try
        {
            ViewData["customers"] = await GetCustomers();
            return View();
        }
        catch { }

        return null;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(OrderVM orderVM)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                var orderModel = new OrderModel()
                {
                    Name = string.IsNullOrEmpty(orderVM.Name) ? $"DH - {DateTime.Now}" : orderVM.Name,
                    Completed = orderVM.Completed,
                    UserId = user.Id,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                if (orderVM.CustomerId != 0)
                {
                    var customer = await GetCustomerById((int)orderVM.CustomerId);
                    if (customer != null)
                    {
                        orderModel.CustomerId = customer.Id;
                    }
                    else
                    {
                        orderModel.CustomerName = orderVM.CustomerName;
                    }
                }
                else
                {
                    orderModel.CustomerName = orderVM.CustomerName;
                }
                await _dbContext.Orders.AddAsync(orderModel);
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

            var orderUpdate = await _dbContext.Orders.Include(o => o.Customer).Where(o => o.Id == id && o.UserId == user.Id).FirstOrDefaultAsync();

            if (orderUpdate != null)
            {
                var orderVMUpdate = GetOrderVMFromOrderModel(orderUpdate);

                if (orderUpdate.Customer != null) orderVMUpdate.CustomerId = orderUpdate.Customer.Id;

                ViewData["customers"] = await GetCustomers();

                return View(orderVMUpdate);
            }
        }
        catch { }

        return null;
    }

    [HttpPost("update/{id}")]
    public async Task<IActionResult> Update(int id, OrderVM orderVM)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var orderUpdate = await _dbContext.Orders.Where(o => o.Id == id && o.UserId == user.Id).FirstOrDefaultAsync();

            if (orderUpdate != null)
            {
                orderUpdate.CustomerName = orderVM.CustomerName;
                orderUpdate.Completed = orderVM.Completed;
                orderUpdate.UpdatedAt = DateTime.Now;

                if (orderVM.CustomerId != 0)
                {
                    var customer = await GetCustomerById((int)orderVM.CustomerId);
                    if (customer != null)
                    {
                        orderUpdate.CustomerId = customer.Id;
                    }
                    else
                    {
                        orderUpdate.CustomerName = orderVM.CustomerName;
                    }
                }
                else
                {
                    orderUpdate.CustomerName = orderVM.CustomerName;
                }

                _dbContext.Orders.Update(orderUpdate);
                int result = await _dbContext.SaveChangesAsync();

                return RedirectToAction("Update", new { id = id });
            }
        }
        catch
        { }
        return RedirectToAction("Update", new { id = id });
    }

    [HttpGet("delete/{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            OrderModel orderDelete = await _dbContext.Orders.Where(o => o.Id == id && o.UserId == user.Id).FirstOrDefaultAsync();
            if (orderDelete != null)
            {
                _dbContext.Orders.Remove(orderDelete);
                int result = await _dbContext.SaveChangesAsync();
            }
        }
        catch { }
        return RedirectToAction("Index");
    }

    [HttpGet("destroy/{id}")]
    public async Task<IActionResult> Destroy(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var orderDestroy = await _dbContext.Orders.Include(o => o.OrderProducts).ThenInclude(op => op.Product).Where(o => o.Id == id && o.UserId == user.Id).FirstOrDefaultAsync();
            if (orderDestroy != null)
            {
                foreach (var orderProduct in orderDestroy.OrderProducts)
                {
                    orderProduct.Product.Quantity += orderProduct.Quantity;
                }
                _dbContext.OrderProducts.RemoveRange(orderDestroy.OrderProducts);
                _dbContext.Orders.Remove(orderDestroy);
                int result = await _dbContext.SaveChangesAsync();
            }
        }
        catch { }
        return RedirectToAction("Index");
    }

    private OrderVM GetOrderVMFromOrderModel(OrderModel order)
    {
        return new OrderVM()
        {
            Id = order.Id,
            Name = order.Name,
            CustomerName = order.CustomerName,
            CustomerId = order.CustomerId,
            Completed = order.Completed
        };
    }

    private async Task<List<CustomerModel>> GetCustomers()
    {
        var user = await _userManager.GetUserAsync(User);
        return await _dbContext.Customers.Where(c => c.UserId == user.Id).ToListAsync();
    }

    private async Task<CustomerModel> GetCustomerById(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        return await _dbContext.Customers.Where(c => c.UserId == user.Id && c.Id == id).FirstOrDefaultAsync();
    }

    private async Task CalculateTotal(OrderModel order, OrderVM orderVM)
    {
        decimal totalBeforeTax = 0;
        decimal TotalAfterTax = 0;

        if (orderVM.ProductInOrders != null)
        {
            var taxDefaults = await GetTaxDefaults();
            foreach (var productInOrder in orderVM.ProductInOrders)
            {
                var product = await _dbContext.Products.Include(p => p.TaxProducts).ThenInclude(t => t.Tax).Where(p => p.Id == productInOrder.ProductId).FirstOrDefaultAsync();

                if (product != null && product.Quantity >= productInOrder.Quantity)
                {
                    //Giá trước thuế
                    decimal priceBeforeTax = productInOrder.Quantity * (product.Price - product.Price * product.Discount);
                    totalBeforeTax += priceBeforeTax;

                    //Giá sau thuế
                    // Thuế mặc định
                    decimal priceAfterTax = priceBeforeTax;
                    string taxes = "";
                    foreach (var taxDefault in taxDefaults)
                    {
                        priceAfterTax += priceBeforeTax * taxDefault.Rate;
                        taxes += $"{taxDefault.Name} - {taxDefault.Rate * 100} %, ";
                    }

                    // Thuế riêng của sản phẩm
                    if (product.TaxProducts != null)
                    {
                        foreach (var taxProduct in product.TaxProducts)
                        {
                            if (taxProduct.Tax != null)
                            {
                                priceAfterTax += priceBeforeTax * taxProduct.Tax.Rate;
                                taxes += $"{taxProduct.Tax.Name} - {taxProduct.Tax.Rate * 100} %, ";
                            }
                        }
                    }

                    var orderProduct = new OrderProductModel()
                    {
                        ProductId = product.Id,
                        OrderId = order.Id,
                        Taxes = taxes,
                        Discount = product.Discount,
                        PriceBeforeTax = priceBeforeTax,
                        PriceAfterTax = priceAfterTax
                    };
                    await _dbContext.AddAsync(orderProduct);
                    product.Quantity = product.Quantity - productInOrder.Quantity;

                    totalBeforeTax += priceBeforeTax;
                    TotalAfterTax += priceAfterTax;
                }
            }
            order.TotalBeforeTax = totalBeforeTax;
            order.TotalAfterTax = TotalAfterTax;
            await _dbContext.SaveChangesAsync();
        }
    }

    private async Task<List<TaxModel>> GetTaxDefaults()
    {
        return await _dbContext.Taxes.Where(t => t.IsDefault).ToListAsync();
    }
}