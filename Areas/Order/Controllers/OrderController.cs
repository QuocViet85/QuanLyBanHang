using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Order.ViewModel;
using WebBanHang.Data;
using WebBanHang.Areas.Order.Model;
using WebBanHang.Areas.Customer.Model;
using WebBanHang.Areas.Tax.Model;
using WebBanHang.Areas.Product.Model;
using Microsoft.AspNetCore.Authorization;
using System.Runtime.CompilerServices;

namespace WebBanHang.Areas.Order.Controllers;

[Area("Order")]
[Route("api/order")]
[Authorize]
public class OrderController : Controller
{
    private readonly ApplicationDbContext _dbContext;
    private readonly UserManager<IdentityUser> _userManager;
    public OrderController(UserManager<IdentityUser> userManager, ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(int pageNumber, int limit)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            List<OrderModel> orders;

            if (pageNumber > 0 && limit > 0)
            {
                orders = await _dbContext.Orders
                                .Where(o => o.UserId == user.Id)
                                .Include(o => o.Customer)
                                .Include(o => o.OrderDetails)
                                .Skip((pageNumber - 1) * limit)
                                .Take(limit)
                                .ToListAsync();
            }
            else
            {
                orders = await _dbContext.Orders
                                .Where(o => o.UserId == user.Id)
                                .Include(o => o.Customer)
                                .Include(o => o.OrderDetails)
                                .ToListAsync();
            }

            int totalOrders = await _dbContext.Orders.CountAsync();

            List<OrderVM> orderVMs = new List<OrderVM>();

            if (orders?.Count > 0)
            {
                orderVMs = orders.Select(p => GetOrderVMFromOrderModel(p)).ToList();
            }

            return Ok(new
            {
                orders = orderVMs,
                totalOrders = totalOrders
            });
        }
        catch { return BadRequest("Lấy hóa đơn thất bại"); }
    }

    [HttpGet("detail/{id}")]
    public async Task<IActionResult> Detail(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);

            var order = await _dbContext.Orders.Where(o => o.Id == id && o.UserId == user.Id)
                                                .Include(o => o.Customer)
                                                .Include(o => o.OrderDetails)
                                                .FirstOrDefaultAsync();

            if (order != null)
            {
                var orderVM = GetOrderVMFromOrderModel(order);
                return Ok(order);
            }
            else
            {
                throw new Exception();
            }
        }
        catch { return BadRequest("Lấy hóa đơn thất bại"); }
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create()
    {
        try
        {
            ViewData["customers"] = await GetCustomers();
            ViewData["products"] = await GetProducts();
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
                    var customer = await GetCustomerById(orderVM.CustomerId);
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

                if (orderVM.ProductInOrders != null)
                {
                    await _dbContext.Orders.AddAsync(orderModel);
                    await _dbContext.SaveChangesAsync();

                    orderVM.ProductInOrders = orderVM.ProductInOrders.Where((ProductInOrder po) =>
                    {
                        return po.ProductId > 0 && po.Quantity > 0;
                    }).ToList();

                    int result = await CalculateTotalPriceAndSaveOrder(orderModel, orderVM);

                    if (result > 0)
                    {
                        return Ok("Tạo hóa đơn thành công");
                    }
                    else
                    {
                        throw new Exception();
                    }
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
        catch
        { return BadRequest("Tạo hóa đơn thất bại"); }
    }

    [HttpPost("completed-order/{id}")]
    public async Task<IActionResult> CompletedOrder(int id, [FromBody] bool completed)
    {
        try
        {
            if (completed)
            {
                var user = await _userManager.GetUserAsync(User);
                var order = await _dbContext.Orders.Where(o => o.Id == id && o.UserId == user.Id)
                                                    .Include(o => o.OrderDetails)
                                                    .FirstOrDefaultAsync();

                if (!order.Completed)
                {
                    order.Completed = true;
                    
                    foreach (var orderDetail in order.OrderDetails)
                    {
                        var product = await _dbContext.Products.Where(p => p.Id == orderDetail.ProductId && p.UserId == user.Id).FirstOrDefaultAsync();

                        if (product.Quantity < orderDetail.Quantity)
                        {
                            return BadRequest($"Sản phẩm {product.Name} đã hết hàng nên không thể hoàn thành hóa đơn");
                        }

                        product.Quantity = product.Quantity - orderDetail.Quantity;
                    }

                    await _dbContext.SaveChangesAsync();
                    return Ok("Hoàn thành hóa đơn thành công");
                }
                else
                {
                    throw new Exception();
                }
                
            }
            else
            {
                throw new Exception();
            }
        }
        catch
        {
            return BadRequest("Hoàn thành đơn thất bại");
        }
    }

    // [HttpGet("update/{id}")]
    // public async Task<IActionResult> Update(int id)
    // {
    //     try
    //     {
    //         var user = await _userManager.GetUserAsync(User);

    //         var orderUpdate = await _dbContext.Orders.Include(o => o.Customer).Where(o => o.Id == id && o.UserId == user.Id).FirstOrDefaultAsync();

    //         if (orderUpdate != null)
    //         {
    //             var orderVMUpdate = GetOrderVMFromOrderModel(orderUpdate);

    //             if (orderUpdate.Customer != null) orderVMUpdate.CustomerId = orderUpdate.Customer.Id;

    //             ViewData["customers"] = await GetCustomers();

    //             return View(orderVMUpdate);
    //         }
    //     }
    //     catch { }

    //     return null;
    // }

    // [HttpPost("update/{id}")]
    // public async Task<IActionResult> Update(int id, OrderVM orderVM)
    // {
    //     try
    //     {
    //         if (ModelState.IsValid)
    //         {
    //             var user = await _userManager.GetUserAsync(User);
    //             var orderUpdate = await _dbContext.Orders.Where(o => o.Id == id && o.UserId == user.Id).FirstOrDefaultAsync();

    //             if (orderUpdate != null)
    //             {
    //                 orderUpdate.CustomerName = orderVM.CustomerName;
    //                 orderUpdate.Completed = orderVM.Completed;
    //                 orderUpdate.UpdatedAt = DateTime.Now;

    //                 if (orderVM.CustomerId != 0)
    //                 {
    //                     var customer = await GetCustomerById(orderVM.CustomerId);
    //                     if (customer != null)
    //                     {
    //                         orderUpdate.CustomerId = customer.Id;
    //                     }
    //                     else
    //                     {
    //                         orderUpdate.CustomerName = orderVM.CustomerName;
    //                     }
    //                 }
    //                 else
    //                 {
    //                     orderUpdate.CustomerName = orderVM.CustomerName;
    //                 }

    //                 _dbContext.Orders.Update(orderUpdate);
    //                 int result = await _dbContext.SaveChangesAsync();

    //                 return Ok("Cập nhật hóa đơn thành công");
    //             }
    //             else
    //             {
    //                 throw new Exception();
    //             }
    //         }
    //         else
    //         {
    //             return BadRequest("Thông tin nhập vào không hợp lệ");
    //         }

    //     }
    //     catch
    //     { return BadRequest("Cập nhật hóa đơn thất bại"); }
    // }

    //Xóa hóa đơn
    [HttpPost("delete/{id}")]
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
                if (result > 0)
                {
                    await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM OrderDetails WHERE OrderId = {0}", orderDelete.Id);

                    return Ok("Xóa hóa đơn thành công");
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                throw new Exception();
            }
        }
        catch { return BadRequest("Xóa hóa đơn thất bại"); }
    }

    //Hủy hóa đơn (xóa hóa đơn và hiệu lực của hóa đơn), hồi phục lại số sản phẩm
    [HttpPost("destroy/{id}")]
    public async Task<IActionResult> Destroy(int id)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            var orderDestroy = await _dbContext.Orders.Include(o => o.OrderDetails).ThenInclude(op => op.Product).Where(o => o.Id == id && o.UserId == user.Id).FirstOrDefaultAsync();
            if (orderDestroy != null)
            {
                foreach (var orderDetail in orderDestroy.OrderDetails)
                {
                    orderDetail.Product.Quantity += orderDetail.Quantity;
                }
                _dbContext.OrderDetails.RemoveRange(orderDestroy.OrderDetails);
                _dbContext.Orders.Remove(orderDestroy);
                int result = await _dbContext.SaveChangesAsync();

                if (result > 0)
                {
                    return Ok("Xóa hóa đơn thành công");
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                throw new Exception();
            }
        }
        catch { return BadRequest("Hủy hóa đơn thất bại"); }
    }

    private OrderVM GetOrderVMFromOrderModel(OrderModel order)
    {
        OrderVM orderVM = new OrderVM()
        {
            Id = order.Id,
            Name = order.Name,
            CustomerName = order.CustomerName,
            CustomerId = order.CustomerId,
            Completed = order.Completed,
            OrderDetails = order.OrderDetails,
            DefaultTaxes = order.DefaultTaxes,
            TotalBeforeDefautTax = order.TotalBeforeDefaultTax,
            TotalAfterTax = order.TotalAfterTax
        };

        if (orderVM.OrderDetails != null)
        {
            foreach (var orderDetail in orderVM.OrderDetails)
            {
                orderDetail.Order = null;
            }
        }

        if (order.Customer != null)
        {
            orderVM.CustomerName = order.Customer.Name;
        }

        orderVM.CreatedAt = order.CreatedAt.ToString("dd/MM/yyyy");
        orderVM.UpdatedAt = order.UpdatedAt.ToString("dd/MM/yyyy");

        return orderVM;
    }

    private async Task<List<CustomerModel>> GetCustomers()
    {
        var user = await _userManager.GetUserAsync(User);
        return await _dbContext.Customers.Where(c => c.UserId == user.Id).ToListAsync();
    }

    private async Task<CustomerModel> GetCustomerById(int? id)
    {
        if (id.HasValue)
        {
            var user = await _userManager.GetUserAsync(User);
            return await _dbContext.Customers.Where(c => c.UserId == user.Id && c.Id == id).FirstOrDefaultAsync();
        }
        return null;
    }

    private async Task<List<ProductModel>> GetProducts()
    {
        var user = await _userManager.GetUserAsync(User);
        return await _dbContext.Products.Where(p => p.UserId == user.Id).ToListAsync();
    }

    private async Task<int> CalculateTotalPriceAndSaveOrder(OrderModel order, OrderVM orderVM)
    {
        var user = await _userManager.GetUserAsync(User);
        decimal totalBeforeDefaultTax = 0;
        decimal totalAfterDefaultTax = 0;

        var taxDefaults = await GetTaxDefaults();
        foreach (var productInOrder in orderVM.ProductInOrders)
        {
            var product = await _dbContext.Products.Include(p => p.TaxProducts).ThenInclude(t => t.Tax).Where(p => p.Id == productInOrder.ProductId && p.UserId == user.Id).FirstOrDefaultAsync();

            if (product != null && product.Quantity >= productInOrder.Quantity)
            {
                decimal priceBeforePrivateTax = (int)productInOrder.Quantity * (product.Price - product.Discount);

                //Giá từng sản phẩm sau thuế riêng
                decimal priceAfterPrivateTax = priceBeforePrivateTax;
                string privateTaxes = "";

                if (product.TaxProducts != null)
                {
                    foreach (var taxProduct in product.TaxProducts)
                    {
                        if (taxProduct.Tax != null && taxProduct.Tax.IsActive)
                        {
                            priceAfterPrivateTax *= (1 + taxProduct.Tax.Rate);
                            privateTaxes += $"{taxProduct.Tax.Name} - {taxProduct.Tax.Rate * 100} %, ";
                        }
                    }
                }

                priceAfterPrivateTax = Math.Round(priceAfterPrivateTax);

                var orderDetail = new OrderDetailModel()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    OrderId = order.Id,
                    Price = product.Price,
                    PrivateTaxes = privateTaxes,
                    Discount = product.Discount,
                    Quantity = product.Quantity,
                    PriceBeforePrivateTax = priceBeforePrivateTax,
                    PriceAfterPrivateTax = priceAfterPrivateTax
                };
                await _dbContext.OrderDetails.AddAsync(orderDetail);

                totalBeforeDefaultTax += priceAfterPrivateTax;

                if (orderVM.Completed)
                {
                    product.Quantity = product.Quantity - (int)productInOrder.Quantity;
                }
            }
            else
            {
                _dbContext.Orders.Remove(order);
                await _dbContext.SaveChangesAsync();

                return 0;
            }
        }

        //Tính tổng tiền sau thuế chung
        totalAfterDefaultTax = totalBeforeDefaultTax;

        string defaultTaxes = "";
        foreach (var tax in taxDefaults)
        {
            totalAfterDefaultTax *= (1 + tax.Rate);
            defaultTaxes += $"{tax.Name} - {tax.Rate * 100} %, ";
        }

        order.TotalBeforeDefaultTax = totalBeforeDefaultTax;
        totalAfterDefaultTax = Math.Round(totalAfterDefaultTax);
        order.TotalAfterTax = totalAfterDefaultTax;
        order.DefaultTaxes = defaultTaxes;
        
        await _dbContext.SaveChangesAsync();

        return 1;
    }

    private async Task<List<TaxModel>> GetTaxDefaults()
    {
        var user = await _userManager.GetUserAsync(User);
        return await _dbContext.Taxes.Where(t => t.IsDefault && t.IsActive && t.UserId == user.Id).ToListAsync();
    }
}