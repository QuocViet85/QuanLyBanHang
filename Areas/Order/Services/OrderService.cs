using Microsoft.EntityFrameworkCore;
using WebBanHang.Areas.Order.Model;
using WebBanHang.Areas.Order.ViewModel;
using WebBanHang.Areas.Tax.Model;
using WebBanHang.Data;

namespace WebBanHang.Areas.Order.Services;

public class OrderService : IOrderService
{
    private readonly ApplicationDbContext _dbContext;

    public OrderService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<(List<OrderVM> orderVMs, int totalOrders)> GetOrders(int pageNumber, int limit, string userId)
    {
        List<OrderModel> orders;

        if (pageNumber > 0 && limit > 0)
        {
            orders = await _dbContext.Orders
                            .Where(o => o.UserId == userId)
                            .Include(o => o.OrderDetails)
                            .Skip((pageNumber - 1) * limit)
                            .Take(limit)
                            .ToListAsync();
        }
        else
        {
            orders = await _dbContext.Orders
                            .Where(o => o.UserId == userId)
                            .Include(o => o.OrderDetails)
                            .ToListAsync();
        }

        int totalOrders = await _dbContext.Orders.CountAsync();

        List<OrderVM> orderVMs = new List<OrderVM>();

        if (orders?.Count > 0)
        {
            orderVMs = orders.Select(p => GetOrderVMFromOrderModel(p)).ToList();
        }

        return (orderVMs, totalOrders);
    }

    public async Task<OrderVM> GetOneOrder(int id, string userId)
    {
        var order = await _dbContext.Orders.Where(o => o.Id == id && o.UserId == userId)
                                                .Include(o => o.OrderDetails)
                                                .FirstOrDefaultAsync();

            if (order != null)
            {
                var orderVM = GetOrderVMFromOrderModel(order);
                return orderVM;
            }
            else
            {
                throw new Exception();
            }
    }

    public async Task Create(OrderVM orderVM, string userId)
    {
        var orderModel = new OrderModel()
        {
            Name = string.IsNullOrEmpty(orderVM.Name) ? $"DH - {DateTime.Now}" : orderVM.Name,
            Completed = orderVM.Completed,
            CustomerName = orderVM.CustomerName,
            CustomerPhoneNumber = orderVM.CustomerPhoneNumber,
            UserId = userId,
            CreatedAt = DateTime.Now,
        };

        List<TaxModel> taxDefaults = await GetTaxDefaults(userId);

        orderModel.DefaultTaxes = "";
        foreach (var tax in taxDefaults)
        {
            orderModel.DefaultTaxes += $"{tax.Name} - {tax.Rate * 100} %, ";
        }

        if (orderVM.ProductInOrders != null)
        {
            await _dbContext.Orders.AddAsync(orderModel);
            await _dbContext.SaveChangesAsync();

            orderVM.ProductInOrders = orderVM.ProductInOrders.Where((ProductInOrder po) =>
            {
                return po.ProductId > 0 && po.Quantity > 0;
            }).ToList();

            int result = await CalculateTotalPriceAndSaveOrder(orderModel, orderVM, userId);

            if (result <= 0)
            {
                throw new Exception();
            }
        }
        else
        {
            throw new Exception();
        }
    }

    public async Task CompletedOrder(int id, bool completed, string userId)
    {
        var order = await _dbContext.Orders.Where(o => o.Id == id && o.UserId == userId)
                                                    .Include(o => o.OrderDetails)
                                                    .FirstOrDefaultAsync();

        if (!order.Completed && completed) 
        {
            order.Completed = true;

            foreach (var orderDetail in order.OrderDetails)
            {
                var product = await _dbContext.Products.Where(p => p.Id == orderDetail.ProductId && p.UserId == userId).FirstOrDefaultAsync();

                if (product.Quantity < orderDetail.Quantity)
                {
                    throw new Exception($"Sản phẩm {product.Name} đã hết hàng nên không thể hoàn thành hóa đơn");
                }

                product.Quantity = product.Quantity - orderDetail.Quantity;
            }

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task Delete(int id, string userId)
    {
        OrderModel orderDelete = await _dbContext.Orders.Where(o => o.Id == id && o.UserId == userId).FirstOrDefaultAsync();
        if (orderDelete != null)
        {
            await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM OrderDetails WHERE OrderId = {0}", orderDelete.Id);
            _dbContext.Orders.Remove(orderDelete);
            int result = await _dbContext.SaveChangesAsync();
            if (result <= 0)
            {
                throw new Exception();
            }
        }
        else
        {
            throw new Exception();
        }
    }

    public async Task Destroy(int id, string userId)
    {
        var orderDestroy = await _dbContext.Orders.Include(o => o.OrderDetails).ThenInclude(op => op.Product).Where(o => o.Id == id && o.UserId == userId).FirstOrDefaultAsync();
        if (orderDestroy != null)
        {
            foreach (var orderDetail in orderDestroy.OrderDetails)
            {
                orderDetail.Product.Quantity += orderDetail.Quantity;
            }
            _dbContext.OrderDetails.RemoveRange(orderDestroy.OrderDetails);
            _dbContext.Orders.Remove(orderDestroy);
            int result = await _dbContext.SaveChangesAsync();

            if (result <= 0)
            {
                throw new Exception();
            }
        }
        else
        {
            throw new Exception();
        }
    }

    public OrderVM GetOrderVMFromOrderModel(OrderModel order)
    {
        OrderVM orderVM = new OrderVM()
        {
            Id = order.Id,
            Name = order.Name,
            CustomerName = order.CustomerName,
            CustomerPhoneNumber = order.CustomerPhoneNumber,
            Completed = order.Completed,
            OrderDetails = order.OrderDetails,
            DefaultTaxes = order.DefaultTaxes,
            TotalBeforeDefaultTax = order.TotalBeforeDefaultTax,
            TotalAfterTax = order.TotalAfterTax
        };

        if (orderVM.OrderDetails != null)
        {
            foreach (var orderDetail in orderVM.OrderDetails)
            {
                orderDetail.Order = null;
            }
        }
        

        orderVM.CreatedAt = order.CreatedAt.ToString("dd/MM/yyyy");

        return orderVM;
    }

    public async Task<List<TaxModel>> GetTaxDefaults(string userId)
    {
        return await _dbContext.Taxes.Where(t => t.IsDefault && t.IsActive && t.UserId == userId).ToListAsync();
    }

    public async Task<int> CalculateTotalPriceAndSaveOrder(OrderModel order, OrderVM orderVM, string userId)
    {
        decimal totalBeforeDefaultTax = 0;
        decimal totalAfterDefaultTax = 0;

        var taxDefaults = await GetTaxDefaults(userId);
        foreach (var productInOrder in orderVM.ProductInOrders)
        {
            var product = await _dbContext.Products.Include(p => p.TaxProducts).ThenInclude(t => t.Tax).Where(p => p.Id == productInOrder.ProductId && p.UserId == userId).FirstOrDefaultAsync();

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
}